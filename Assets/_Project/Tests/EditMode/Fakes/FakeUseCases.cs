using System.Collections.Generic;
using System.Threading;
using App.Application.Dogs;
using App.Application.Images;
using App.Application.Weather;
using App.Domain.Dogs;
using App.Domain.Weather;
using Cysharp.Threading.Tasks;

namespace App.Tests.Fakes
{
    public sealed class FakeGetBreedsUseCase : IGetBreedsUseCase
    {
        public List<PendingCall<IReadOnlyList<Breed>>> Calls { get; } = new List<PendingCall<IReadOnlyList<Breed>>>();

        public PendingCall<IReadOnlyList<Breed>> LastCall => Calls[Calls.Count - 1];

        public UniTask<IReadOnlyList<Breed>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var call = new PendingCall<IReadOnlyList<Breed>>(null, cancellationToken);
            Calls.Add(call);
            return call.Task;
        }
    }

    public sealed class FakeGetBreedDetailsUseCase : IGetBreedDetailsUseCase
    {
        public List<PendingCall<BreedDetails>> Calls { get; } = new List<PendingCall<BreedDetails>>();

        public PendingCall<BreedDetails> LastCall => Calls[Calls.Count - 1];

        public UniTask<BreedDetails> ExecuteAsync(string breedId, CancellationToken cancellationToken)
        {
            var call = new PendingCall<BreedDetails>(breedId, cancellationToken);
            Calls.Add(call);
            return call.Task;
        }
    }

    public sealed class FakeGetTodayForecastUseCase : IGetTodayForecastUseCase
    {
        public List<PendingCall<WeatherForecast>> Calls { get; } = new List<PendingCall<WeatherForecast>>();

        public PendingCall<WeatherForecast> LastCall => Calls[Calls.Count - 1];

        public UniTask<WeatherForecast> ExecuteAsync(CancellationToken cancellationToken)
        {
            var call = new PendingCall<WeatherForecast>(null, cancellationToken);
            Calls.Add(call);
            return call.Task;
        }
    }

    public sealed class FakeLoadImageUseCase : ILoadImageUseCase
    {
        public UniTask<byte[]> ExecuteAsync(string url, CancellationToken cancellationToken) =>
            UniTask.FromException<byte[]>(new System.InvalidOperationException("Images are not loaded in tests."));
    }
}
