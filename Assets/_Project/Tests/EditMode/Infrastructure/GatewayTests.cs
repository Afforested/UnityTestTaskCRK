using System.Collections.Generic;
using System.Threading;
using App.Infrastructure.Dogs;
using App.Infrastructure.Dogs.Dto;
using App.Infrastructure.Network;
using App.Infrastructure.Network.Http;
using App.Infrastructure.Weather;
using App.Infrastructure.Weather.Dto;
using App.Tests.Fakes;
using NUnit.Framework;

namespace App.Tests.Infrastructure
{
    public sealed class GatewayTests
    {
        private static readonly NetworkSettings Network = new NetworkSettings(10, "test", logRequests: false);

        [Test]
        public void DogBreedsGateway_RequestsBreedsPageThroughQueue()
        {
            var response = new BreedListResponseDto
            {
                Data = new List<BreedDataDto>
                {
                    new BreedDataDto { Id = "a", Attributes = new BreedAttributesDto { Name = "Akita" } },
                },
            };
            var queue = new RecordingRequestQueue(response);
            var gateway = new DogBreedsGateway(queue, new DogApiSettings("https://dogapi.dog/api/v2/"), Network);

            var breeds = gateway.GetBreedsAsync(10, CancellationToken.None).GetAwaiter().GetResult();

            Assert.That(queue.Requests, Has.Count.EqualTo(1));
            var request = (JsonWebRequest<BreedListResponseDto>)queue.Requests[0];
            Assert.That(request.Url, Is.EqualTo("https://dogapi.dog/api/v2/breeds?page%5Bsize%5D=10"));
            Assert.That(breeds[0].Name, Is.EqualTo("Akita"));
        }

        [Test]
        public void DogBreedsGateway_RequestsBreedDetailsByIdThroughQueue()
        {
            var response = new BreedResponseDto
            {
                Data = new BreedDataDto
                {
                    Id = "id-1",
                    Attributes = new BreedAttributesDto { Name = "Akita", Description = "Loyal." },
                },
            };
            var queue = new RecordingRequestQueue(response);
            var gateway = new DogBreedsGateway(queue, new DogApiSettings("https://dogapi.dog/api/v2"), Network);

            var details = gateway.GetBreedDetailsAsync("id-1", CancellationToken.None).GetAwaiter().GetResult();

            var request = (JsonWebRequest<BreedResponseDto>)queue.Requests[0];
            Assert.That(request.Url, Is.EqualTo("https://dogapi.dog/api/v2/breeds/id-1"));
            Assert.That(details.Description, Is.EqualTo("Loyal."));
        }

        [Test]
        public void WeatherGateway_RequestsForecastThroughQueue()
        {
            var response = new ForecastResponseDto
            {
                Properties = new ForecastPropertiesDto
                {
                    Periods = new List<ForecastPeriodDto>
                    {
                        new ForecastPeriodDto { Number = 1, Name = "Today", Temperature = 61, TemperatureUnit = "F" },
                    },
                },
            };
            var queue = new RecordingRequestQueue(response);
            const string url = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
            var gateway = new WeatherGateway(queue, new WeatherApiSettings(url, "large"), Network);

            var periods = gateway.GetForecastAsync(CancellationToken.None).GetAwaiter().GetResult();

            Assert.That(((JsonWebRequest<ForecastResponseDto>)queue.Requests[0]).Url, Is.EqualTo(url));
            Assert.That(periods[0].Temperature, Is.EqualTo(61));
        }
    }
}
