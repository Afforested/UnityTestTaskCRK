using System.IO;
using App.Infrastructure.Dogs;
using App.Infrastructure.Dogs.Dto;
using App.Infrastructure.Weather;
using App.Infrastructure.Weather.Dto;
using Newtonsoft.Json;
using NUnit.Framework;

namespace App.Tests.Infrastructure
{
    public sealed class MapperTests
    {
        private const string ForecastJson = @"{
  ""type"": ""Feature"",
  ""properties"": {
    ""units"": ""us"",
    ""periods"": [
      {
        ""number"": 2,
        ""name"": ""Tuesday"",
        ""isDaytime"": true,
        ""temperature"": 68,
        ""temperatureUnit"": ""F"",
        ""temperatureTrend"": null,
        ""probabilityOfPrecipitation"": { ""unitCode"": ""wmoUnit:percent"", ""value"": 57 },
        ""icon"": ""https://api.weather.gov/icons/land/day/rain_showers,60/bkn?size=medium"",
        ""shortForecast"": ""Rain Showers Likely""
      },
      {
        ""number"": 1,
        ""name"": ""Overnight"",
        ""isDaytime"": false,
        ""temperature"": 53,
        ""temperatureUnit"": ""F"",
        ""temperatureTrend"": null,
        ""icon"": ""https://api.weather.gov/icons/land/night/rain_showers,50?size=medium"",
        ""shortForecast"": ""Chance Rain Showers""
      }
    ]
  }
}";

        private const string BreedsJson = @"{
  ""data"": [
    { ""id"": ""036feed0"", ""type"": ""breed"", ""attributes"": { ""name"": ""Affenpinscher"", ""description"": ""Small and playful."", ""hypoallergenic"": true } },
    { ""id"": ""dd9362cc"", ""type"": ""breed"", ""attributes"": { ""name"": ""Afghan Hound"", ""description"": ""Elegant."" } }
  ],
  ""meta"": { ""pagination"": { ""current"": 1, ""next"": 2, ""last"": 29, ""records"": 283 } }
}";

        private const string BreedJson = @"{
  ""data"": {
    ""id"": ""036feed0"",
    ""type"": ""breed"",
    ""attributes"": { ""name"": ""Affenpinscher"", ""description"": ""The Affenpinscher is a small and playful breed."" },
    ""relationships"": { ""group"": { ""data"": { ""id"": ""f56dc4b1"", ""type"": ""group"" } } }
  }
}";

        [Test]
        public void Forecast_IsOrderedByPeriodNumber_AndMapped()
        {
            var response = JsonConvert.DeserializeObject<ForecastResponseDto>(ForecastJson);

            var periods = WeatherForecastMapper.Map(response, "large");

            Assert.That(periods, Has.Count.EqualTo(2));
            Assert.That(periods[0].PeriodName, Is.EqualTo("Overnight"));
            Assert.That(periods[0].Temperature, Is.EqualTo(53));
            Assert.That(periods[0].TemperatureUnit, Is.EqualTo("F"));
            Assert.That(periods[0].ShortForecast, Is.EqualTo("Chance Rain Showers"));
            Assert.That(periods[0].IconUrl, Is.EqualTo("https://api.weather.gov/icons/land/night/rain_showers,50?size=large"));
        }

        [Test]
        public void Forecast_WithoutPeriods_Throws()
        {
            var response = JsonConvert.DeserializeObject<ForecastResponseDto>(@"{ ""properties"": {} }");

            Assert.Throws<InvalidDataException>(() => WeatherForecastMapper.Map(response, "large"));
        }

        [TestCase("https://x/icon?size=medium", "large", "https://x/icon?size=large")]
        [TestCase("https://x/icon", "small", "https://x/icon?size=small")]
        [TestCase("https://x/icon?a=1&size=medium", "large", "https://x/icon?a=1&size=large")]
        [TestCase("https://x/icon?size=medium", "", "https://x/icon?size=medium")]
        public void IconSize_IsReplacedOrAdded(string url, string size, string expected) =>
            Assert.That(WeatherForecastMapper.WithIconSize(url, size), Is.EqualTo(expected));

        [Test]
        public void Breeds_AreMappedInResponseOrder()
        {
            var response = JsonConvert.DeserializeObject<BreedListResponseDto>(BreedsJson);

            var breeds = BreedMapper.ToBreeds(response);

            Assert.That(breeds, Has.Count.EqualTo(2));
            Assert.That(breeds[0].Id, Is.EqualTo("036feed0"));
            Assert.That(breeds[0].Name, Is.EqualTo("Affenpinscher"));
            Assert.That(breeds[1].Name, Is.EqualTo("Afghan Hound"));
        }

        [Test]
        public void BreedDetails_AreMapped()
        {
            var response = JsonConvert.DeserializeObject<BreedResponseDto>(BreedJson);

            var details = BreedMapper.ToDetails(response);

            Assert.That(details.Id, Is.EqualTo("036feed0"));
            Assert.That(details.Name, Is.EqualTo("Affenpinscher"));
            Assert.That(details.Description, Is.EqualTo("The Affenpinscher is a small and playful breed."));
        }

        [Test]
        public void BreedDetails_WithoutData_Throw()
        {
            var response = JsonConvert.DeserializeObject<BreedResponseDto>(@"{ ""errors"": [] }");

            Assert.Throws<InvalidDataException>(() => BreedMapper.ToDetails(response));
        }
    }
}
