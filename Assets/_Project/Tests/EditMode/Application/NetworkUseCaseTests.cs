using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Application.Dogs;
using App.Application.Weather;
using App.Domain.Dogs;
using App.Domain.Weather;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace App.Tests.Application
{
    public sealed class NetworkUseCaseTests
    {
        [Test]
        public void GetBreeds_AsksGatewayForConfiguredCount_AndTrimsExtraItems()
        {
            var gateway = new StubDogBreedsGateway(Enumerable.Range(1, 12).Select(i => new Breed("id" + i, "Breed " + i)).ToList());
            var useCase = new GetBreedsUseCase(gateway, new DogsSettings(10));

            var breeds = useCase.ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();

            Assert.That(gateway.RequestedCount, Is.EqualTo(10));
            Assert.That(breeds, Has.Count.EqualTo(10));
        }

        [Test]
        public void GetTodayForecast_ReturnsFirstPeriod()
        {
            var gateway = new StubWeatherGateway(new[]
            {
                new WeatherForecast("Tonight", 53, "F", "Clear", string.Empty),
                new WeatherForecast("Tuesday", 68, "F", "Sunny", string.Empty),
            });
            var useCase = new GetTodayForecastUseCase(gateway);

            var forecast = useCase.ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();

            Assert.That(forecast.Temperature, Is.EqualTo(53));
        }

        [Test]
        public void GetTodayForecast_WithoutPeriods_Throws()
        {
            var useCase = new GetTodayForecastUseCase(new StubWeatherGateway(Array.Empty<WeatherForecast>()));

            Assert.Throws<InvalidOperationException>(() => useCase.ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult());
        }

        private sealed class StubDogBreedsGateway : IDogBreedsGateway
        {
            private readonly IReadOnlyList<Breed> _breeds;

            public StubDogBreedsGateway(IReadOnlyList<Breed> breeds) => _breeds = breeds;

            public int RequestedCount { get; private set; }

            public UniTask<IReadOnlyList<Breed>> GetBreedsAsync(int count, CancellationToken cancellationToken)
            {
                RequestedCount = count;
                return UniTask.FromResult(_breeds);
            }

            public UniTask<BreedDetails> GetBreedDetailsAsync(string breedId, CancellationToken cancellationToken) =>
                throw new NotSupportedException();
        }

        private sealed class StubWeatherGateway : IWeatherGateway
        {
            private readonly IReadOnlyList<WeatherForecast> _periods;

            public StubWeatherGateway(IReadOnlyList<WeatherForecast> periods) => _periods = periods;

            public UniTask<IReadOnlyList<WeatherForecast>> GetForecastAsync(CancellationToken cancellationToken) =>
                UniTask.FromResult(_periods);
        }
    }
}
