using System.Threading;
using App.Domain.Weather;
using Cysharp.Threading.Tasks;

namespace App.Application.Weather
{
    public interface IGetTodayForecastUseCase
    {
        UniTask<WeatherForecast> ExecuteAsync(CancellationToken cancellationToken);
    }
}
