using App.Application.Weather;
using App.Infrastructure.Weather;
using App.Presentation.Common;
using App.Presentation.Weather;
using UnityEngine;
using Zenject;

namespace App.Bootstrap.Installers
{
    public sealed class WeatherInstaller : MonoInstaller
    {
        [SerializeField] private WeatherView _view;

        public override void InstallBindings()
        {
            Container.Bind<IWeatherGateway>().To<WeatherGateway>().AsSingle();
            Container.Bind<IGetTodayForecastUseCase>().To<GetTodayForecastUseCase>().AsSingle();

            Container.BindInterfacesAndSelfTo<RemoteSpriteCache>().AsSingle();
            Container.Bind<IWeatherView>().FromInstance(_view).AsSingle();
            Container.BindInterfacesTo<WeatherPresenter>().AsSingle();
        }
    }
}
