using App.Domain.Clicker;
using NUnit.Framework;

namespace App.Tests.Domain
{
    public sealed class CurrencyCollectorTests
    {
        [Test]
        public void TryCollect_SpendsEnergy_AndAddsReward()
        {
            var wallet = new Wallet(0);
            var energy = new Energy(1000, 1000);
            var collector = new CurrencyCollector(wallet, energy);

            var collected = collector.TryCollect(new CollectRule(reward: 1, energyCost: 1));

            Assert.That(collected, Is.True);
            Assert.That(wallet.Balance, Is.EqualTo(1));
            Assert.That(energy.Current, Is.EqualTo(999));
        }

        [Test]
        public void TryCollect_WithoutEnergy_GivesNothing()
        {
            var wallet = new Wallet(0);
            var energy = new Energy(1000, 0);
            var collector = new CurrencyCollector(wallet, energy);

            var collected = collector.TryCollect(new CollectRule(reward: 1, energyCost: 1));

            Assert.That(collected, Is.False);
            Assert.That(wallet.Balance, Is.Zero);
        }
    }
}
