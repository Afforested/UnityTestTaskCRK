using System;

namespace App.Infrastructure.Weather
{
    public sealed class WeatherApiSettings
    {
        public WeatherApiSettings(string forecastUrl, string iconSize)
        {
            if (string.IsNullOrWhiteSpace(forecastUrl))
                throw new ArgumentException("Forecast url is required.", nameof(forecastUrl));

            ForecastUrl = forecastUrl;
            IconSize = iconSize ?? string.Empty;
        }

        public string ForecastUrl { get; }

        public string IconSize { get; }
    }
}
