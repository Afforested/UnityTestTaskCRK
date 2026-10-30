using App.Application.Weather;
using App.Domain.Weather;
using App.Presentation.Common;
using App.Presentation.Weather;
using App.Tests.Fakes;
using NUnit.Framework;

namespace App.Tests.Presentation
{
    public sealed class WeatherPresenterTests
    {
        private FakeWeatherView _view;
        private FakeGetTodayForecastUseCase _getForecast;
        private RemoteSpriteCache _icons;
        private WeatherPresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            _view = new FakeWeatherView();
            _getForecast = new FakeGetTodayForecastUseCase();
            _icons = new RemoteSpriteCache(new FakeLoadImageUseCase());
            _presenter = new WeatherPresenter(_view, _getForecast, _icons, new WeatherSettings(5f), TestLogger.Create());
        }

        [TearDown]
        public void TearDown()
        {
            _presenter.Dispose();
            _icons.Dispose();
        }

        [Test]
        public void Open_RequestsForecastRightAway_AndShowsLoader()
        {
            _presenter.Open();

            Assert.That(_view.IsVisible, Is.True);
            Assert.That(_view.IsLoadingShown, Is.True);
            Assert.That(_getForecast.Calls, Has.Count.EqualTo(1));
        }

        [Test]
        public void Forecast_IsShownAsTodayWithTemperatureAndUnit()
        {
            _presenter.Open();

            _getForecast.LastCall.Complete(new WeatherForecast("This Afternoon", 61, "F", "Sunny", string.Empty));

            Assert.That(_view.Title, Is.EqualTo("Сегодня - 61F"));
            Assert.That(_view.Description, Is.EqualTo("Sunny"));
        }

        [Test]
        public void Close_CancelsRequestOfTheTab()
        {
            _presenter.Open();
            var call = _getForecast.LastCall;

            _presenter.Close();

            Assert.That(call.IsCancelled, Is.True);
            Assert.That(_view.IsVisible, Is.False);
        }

        [Test]
        public void ReopeningTab_RequestsForecastAgain()
        {
            _presenter.Open();
            _presenter.Close();

            _presenter.Open();

            Assert.That(_getForecast.Calls, Has.Count.EqualTo(2));
            Assert.That(_getForecast.LastCall.IsCancelled, Is.False);
        }
    }
}
