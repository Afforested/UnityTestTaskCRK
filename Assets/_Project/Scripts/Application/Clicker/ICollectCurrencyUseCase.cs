using System;

namespace App.Application.Clicker
{
    public interface ICollectCurrencyUseCase
    {
        IObservable<CollectResult> Results { get; }

        CollectResult Execute(CollectSource source);
    }
}
