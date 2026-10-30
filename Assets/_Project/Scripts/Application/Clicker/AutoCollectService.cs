using System;
using App.Application.Common;
using App.Domain.Common;
using UniRx;

namespace App.Application.Clicker
{
    public sealed class AutoCollectService : IAutoCollectService, IUpdatable, IDisposable
    {
        private readonly ICollectCurrencyUseCase _collectCurrency;
        private readonly PeriodicTimer _timer;
        private readonly ReactiveProperty<float> _progress = new ReactiveProperty<float>(0f);

        public AutoCollectService(ICollectCurrencyUseCase collectCurrency, ClickerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _collectCurrency = collectCurrency ?? throw new ArgumentNullException(nameof(collectCurrency));
            _timer = new PeriodicTimer(settings.AutoCollectIntervalSeconds);
        }

        public IReadOnlyReactiveProperty<float> Progress => _progress;

        public void Tick(float deltaTime)
        {
            var completed = _timer.Advance(deltaTime);
            for (var i = 0; i < completed; i++)
                _collectCurrency.Execute(CollectSource.Auto);

            _progress.Value = _timer.Progress;
        }

        public void Dispose() => _progress.Dispose();
    }
}
