using System.Collections.Generic;
using Newtonsoft.Json;

namespace App.Infrastructure.Weather.Dto
{

    internal sealed class ForecastResponseDto
    {
        [JsonProperty("properties")]
        public ForecastPropertiesDto Properties { get; set; }
    }

    internal sealed class ForecastPropertiesDto
    {
        [JsonProperty("periods")]
        public List<ForecastPeriodDto> Periods { get; set; }
    }

    internal sealed class ForecastPeriodDto
    {
        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("temperature")]
        public int? Temperature { get; set; }

        [JsonProperty("temperatureUnit")]
        public string TemperatureUnit { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("shortForecast")]
        public string ShortForecast { get; set; }
    }
}
