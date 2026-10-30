using System;
using System.Collections.Generic;
using System.Threading;
using App.Application.Weather;
using App.Domain.Weather;
using App.Infrastructure.Network;
using App.Infrastructure.Network.Http;
using App.Infrastructure.Weather.Dto;
using Cysharp.Threading.Tasks;

namespace App.Infrastructure.Weather
{
    public sealed class WeatherGateway : IWeatherGateway
    {
        private readonly IRequestQueue _queue;
        private readonly WeatherApiSettings _apiSettings;
        private readonly NetworkSettings _networkSettings;

        public WeatherGateway(IRequestQueue queue, WeatherApiSettings apiSettings, NetworkSettings networkSettings)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
            _networkSettings = networkSettings ?? throw new ArgumentNullException(nameof(networkSettings));
        }

        public async UniTask<IReadOnlyList<WeatherForecast>> GetForecastAsync(CancellationToken cancellationToken)
        {
            var request = new JsonWebRequest<ForecastResponseDto>(_apiSettings.ForecastUrl, _networkSettings, "Weather forecast");
            var response = await _queue.Enqueue(request, cancellationToken);
            return WeatherForecastMapper.Map(response, _apiSettings.IconSize);
        }
    }
}
