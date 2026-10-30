using System.Collections.Generic;
using System.Threading;
using App.Domain.Weather;
using Cysharp.Threading.Tasks;

namespace App.Application.Weather
{
    public interface IWeatherGateway
    {
        UniTask<IReadOnlyList<WeatherForecast>> GetForecastAsync(CancellationToken cancellationToken);
    }
}
