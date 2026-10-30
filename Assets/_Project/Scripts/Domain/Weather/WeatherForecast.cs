using System;

namespace App.Domain.Weather
{
    public sealed class WeatherForecast
    {
        public WeatherForecast(string periodName, int temperature, string temperatureUnit, string shortForecast, string iconUrl)
        {
            if (string.IsNullOrWhiteSpace(temperatureUnit))
                throw new ArgumentException("Temperature unit is required.", nameof(temperatureUnit));

            PeriodName = periodName ?? string.Empty;
            Temperature = temperature;
            TemperatureUnit = temperatureUnit;
            ShortForecast = shortForecast ?? string.Empty;
            IconUrl = iconUrl ?? string.Empty;
        }

        public string PeriodName { get; }

        public int Temperature { get; }

        public string TemperatureUnit { get; }

        public string ShortForecast { get; }

        public string IconUrl { get; }

        public bool HasIcon => IconUrl.Length > 0;
    }
}
