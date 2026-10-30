using System;
using App.Domain.Clicker;
using UniRx;

namespace App.Application.Clicker
{
   
    public sealed class CollectCurrencyUseCase : ICollectCurrencyUseCase, IDisposable
    {
        private readonly CurrencyCollector _collector;
        private readonly ClickerSettings _settings;
        private readonly Subject<CollectResult> _results = new Subject<CollectResult>();

        public CollectCurrencyUseCase(CurrencyCollector collector, ClickerSettings settings)
        {
            _collector = collector ?? throw new ArgumentNullException(nameof(collector));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IObservable<CollectResult> Results => _results;

        public CollectResult Execute(CollectSource source)
        {
            var rule = GetRule(source);
            var isCollected = _collector.TryCollect(rule);
            var result = new CollectResult(source, isCollected, isCollected ? rule.Reward : 0);

            _results.OnNext(result);
            return result;
        }

        public void Dispose() => _results.Dispose();

        private CollectRule GetRule(CollectSource source)
        {
            switch (source)
            {
                case CollectSource.Manual:
                    return _settings.ManualCollect;
                case CollectSource.Auto:
                    return _settings.AutoCollect;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }
    }
}
