using System;
using App.Application.Common;
using App.Domain.Clicker;
using App.Domain.Common;
using UniRx;

namespace App.Application.Clicker
{
    public sealed class EnergyRegenerationService : IEnergyRegenerationService, IUpdatable, IDisposable
    {
        private readonly Energy _energy;
        private readonly PeriodicTimer _timer;
        private readonly ReactiveProperty<float> _secondsUntilRestore;

        public EnergyRegenerationService(Energy energy, ClickerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _energy = energy ?? throw new ArgumentNullException(nameof(energy));
            _timer = new PeriodicTimer(settings.EnergyRegenIntervalSeconds);
            _secondsUntilRestore = new ReactiveProperty<float>(_timer.Remaining);
            RestoreAmount = settings.EnergyRegenAmount;
        }

        public int RestoreAmount { get; }

        public IReadOnlyReactiveProperty<float> SecondsUntilRestore => _secondsUntilRestore;

        public void Tick(float deltaTime)
        {
            var completed = _timer.Advance(deltaTime);
            for (var i = 0; i < completed; i++)
                _energy.Restore(RestoreAmount);

            _secondsUntilRestore.Value = _timer.Remaining;
        }

        public void Dispose() => _secondsUntilRestore.Dispose();
    }
}
