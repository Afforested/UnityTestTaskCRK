using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Domain.Weather;
using App.Infrastructure.Weather.Dto;

namespace App.Infrastructure.Weather
{
    internal static class WeatherForecastMapper
    {
        private const string SizeParameter = "size=";

        public static IReadOnlyList<WeatherForecast> Map(ForecastResponseDto response, string iconSize)
        {
            var periods = response?.Properties?.Periods;
            if (periods == null)
                throw new InvalidDataException("The forecast response has no periods.");

            return periods
                .Where(period => period?.Temperature != null && !string.IsNullOrWhiteSpace(period.TemperatureUnit))
                .OrderBy(period => period.Number)
                .Select(period => new WeatherForecast(
                    period.Name,
                    period.Temperature.Value,
                    period.TemperatureUnit,
                    period.ShortForecast,
                    WithIconSize(period.Icon, iconSize)))
                .ToList();
        }

        public static string WithIconSize(string iconUrl, string size)
        {
            if (string.IsNullOrEmpty(iconUrl) || string.IsNullOrEmpty(size))
                return iconUrl;

            var queryStart = iconUrl.IndexOf('?');
            var path = queryStart < 0 ? iconUrl : iconUrl.Substring(0, queryStart);
            var query = queryStart < 0 ? string.Empty : iconUrl.Substring(queryStart + 1);

            var parameters = query
                .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(parameter => !parameter.StartsWith(SizeParameter, StringComparison.OrdinalIgnoreCase))
                .Append(SizeParameter + size);

            return path + "?" + string.Join("&", parameters);
        }
    }
}
