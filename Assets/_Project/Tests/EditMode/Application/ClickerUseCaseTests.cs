using System.Collections.Generic;
using App.Application.Clicker;
using App.Domain.Clicker;
using App.Tests.Fakes;
using NUnit.Framework;
using UniRx;

namespace App.Tests.Application
{
    public sealed class ClickerUseCaseTests
    {
        private Wallet _wallet;
        private Energy _energy;

        private CollectCurrencyUseCase CreateCollect(ClickerSettings settings, int? energy = null)
        {
            _wallet = new Wallet(settings.StartBalance);
            _energy = energy.HasValue ? new Energy(settings.MaxEnergy, energy.Value) : Energy.Full(settings.MaxEnergy);
            return new CollectCurrencyUseCase(new CurrencyCollector(_wallet, _energy), settings);
        }

        [Test]
        public void ManualCollect_UsesTapRule()
        {
            var collect = CreateCollect(TestSettings.Clicker(tapReward: 2, tapEnergyCost: 3));

            var result = collect.Execute(CollectSource.Manual);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Reward, Is.EqualTo(2));
            Assert.That(_wallet.Balance, Is.EqualTo(2));
            Assert.That(_energy.Current, Is.EqualTo(997));
        }

        [Test]
        public void AutoCollect_UsesAutoCollectRule()
        {
            var collect = CreateCollect(TestSettings.Clicker(autoReward: 5, autoEnergyCost: 4));

            collect.Execute(CollectSource.Auto);

            Assert.That(_wallet.Balance, Is.EqualTo(5));
            Assert.That(_energy.Current, Is.EqualTo(996));
        }

        [Test]
        public void Collect_WithoutEnergy_FailsAndReportsIt()
        {
            var collect = CreateCollect(TestSettings.Clicker(), energy: 0);
            var results = new List<CollectResult>();
            collect.Results.Subscribe(results.Add);

            var result = collect.Execute(CollectSource.Manual);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Reward, Is.Zero);
            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(_wallet.Balance, Is.Zero);
        }

        [Test]
        public void AutoCollectService_PressesButtonEveryInterval()
        {
            var settings = TestSettings.Clicker(autoCollectInterval: 3f);
            var collect = CreateCollect(settings);
            var autoCollect = new AutoCollectService(collect, settings);
            var sources = new List<CollectSource>();
            collect.Results.Subscribe(result => sources.Add(result.Source));

            autoCollect.Tick(2.9f);
            Assert.That(sources, Is.Empty);

            autoCollect.Tick(0.2f);
            Assert.That(sources, Is.EqualTo(new[] { CollectSource.Auto }));
            Assert.That(_wallet.Balance, Is.EqualTo(1));
            Assert.That(_energy.Current, Is.EqualTo(999));

            autoCollect.Tick(6f);
            Assert.That(sources, Has.Count.EqualTo(3));
        }

        [Test]
        public void AutoCollectService_ReportsProgressTowardsNextCollect()
        {
            var settings = TestSettings.Clicker(autoCollectInterval: 4f);
            var autoCollect = new AutoCollectService(CreateCollect(settings), settings);

            autoCollect.Tick(1f);

            Assert.That(autoCollect.Progress.Value, Is.EqualTo(0.25f).Within(1e-4f));
        }

        [Test]
        public void EnergyRegeneration_RestoresAmountEveryInterval_UpToMax()
        {
            var settings = TestSettings.Clicker(maxEnergy: 1000, energyRegenAmount: 10, energyRegenInterval: 10f);
            var energy = new Energy(settings.MaxEnergy, 985);
            var regeneration = new EnergyRegenerationService(energy, settings);

            regeneration.Tick(9.9f);
            Assert.That(energy.Current, Is.EqualTo(985));

            regeneration.Tick(0.1f);
            Assert.That(energy.Current, Is.EqualTo(995));

            regeneration.Tick(10f);
            Assert.That(energy.Current, Is.EqualTo(1000));
        }

        [Test]
        public void ClickerState_MirrorsWalletAndEnergy()
        {
            var collect = CreateCollect(TestSettings.Clicker());
            using (var state = new ClickerState(_wallet, _energy))
            {
                collect.Execute(CollectSource.Manual);

                Assert.That(state.Balance.Value, Is.EqualTo(1));
                Assert.That(state.CurrentEnergy.Value, Is.EqualTo(999));
                Assert.That(state.MaxEnergy, Is.EqualTo(1000));
            }
        }
    }
}
