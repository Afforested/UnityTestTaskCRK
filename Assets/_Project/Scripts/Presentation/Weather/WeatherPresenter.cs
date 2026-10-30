using System;
using System.Threading;
using App.Application.Weather;
using App.Domain.Weather;
using App.Presentation.Common;
using App.Presentation.Navigation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Presentation.Weather
{
  
    public sealed class WeatherPresenter : ITabPresenter, IDisposable
    {
        private const string LogTag = nameof(WeatherPresenter);
        private const string TodayLabel = "Сегодня";

        private readonly IWeatherView _view;
        private readonly IGetTodayForecastUseCase _getTodayForecast;
        private readonly RemoteSpriteCache _icons;
        private readonly WeatherSettings _settings;
        private readonly ILogger _logger;

        private CancellationTokenSource _openTabCancellation;
        private bool _hasForecast;

        public WeatherPresenter(
            IWeatherView view,
            IGetTodayForecastUseCase getTodayForecast,
            RemoteSpriteCache icons,
            WeatherSettings settings,
            ILogger logger)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _getTodayForecast = getTodayForecast ?? throw new ArgumentNullException(nameof(getTodayForecast));
            _icons = icons ?? throw new ArgumentNullException(nameof(icons));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TabId Id => TabId.Weather;

        public void Open()
        {
            _view.Show();
            if (!_hasForecast)
                _view.ShowLoading();

            _openTabCancellation = new CancellationTokenSource();
            PollAsync(_openTabCancellation.Token).Forget();
        }

        public void Close()
        {
            CancellationSourceUtility.CancelAndDispose(ref _openTabCancellation);
            _view.Hide();
        }

        public void Dispose() => CancellationSourceUtility.CancelAndDispose(ref _openTabCancellation);

        private async UniTaskVoid PollAsync(CancellationToken cancellationToken)
        {
            var interval = TimeSpan.FromSeconds(_settings.PollIntervalSeconds);
            var refresh = UniTask.CompletedTask;

            while (true)
            {
              
                if (refresh.Status.IsCompleted())
                    refresh = RefreshAsync(cancellationToken).Preserve();

                var isCancelled = await UniTask.Delay(interval, DelayType.Realtime, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
                if (isCancelled)
                    return;
            }
        }

        private async UniTask RefreshAsync(CancellationToken cancellationToken)
        {
            try
            {
                var forecast = await _getTodayForecast.ExecuteAsync(cancellationToken);
                var icon = forecast.HasIcon ? await LoadIconAsync(forecast.IconUrl, cancellationToken) : null;

                _hasForecast = true;
                _view.ShowForecast(icon, FormatTitle(forecast), forecast.ShortForecast, $"Обновлено в {DateTime.Now:HH:mm:ss}");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _logger.Log(LogType.Warning, LogTag, $"Forecast update failed: {exception.Message}");
                _view.ShowError("Не удалось обновить погоду. Повторим через несколько секунд.");
            }
        }

        private async UniTask<Sprite> LoadIconAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                return await _icons.GetAsync(url, cancellationToken);
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                _logger.Log(LogType.Warning, LogTag, $"Weather icon failed to load: {exception.Message}");
                return null;
            }
        }

        private static string FormatTitle(WeatherForecast forecast) =>
            $"{TodayLabel} - {forecast.Temperature}{forecast.TemperatureUnit}";
    }
}
