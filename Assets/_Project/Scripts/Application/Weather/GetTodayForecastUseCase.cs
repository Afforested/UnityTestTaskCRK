using System;
using System.Threading;
using App.Domain.Weather;
using Cysharp.Threading.Tasks;

namespace App.Application.Weather
{
    public sealed class GetTodayForecastUseCase : IGetTodayForecastUseCase
    {
        private readonly IWeatherGateway _gateway;

        public GetTodayForecastUseCase(IWeatherGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        public async UniTask<WeatherForecast> ExecuteAsync(CancellationToken cancellationToken)
        {
            var periods = await _gateway.GetForecastAsync(cancellationToken);
            if (periods == null || periods.Count == 0)
                throw new InvalidOperationException("The weather service returned no forecast periods.");

            return periods[0];
        }
    }
}
