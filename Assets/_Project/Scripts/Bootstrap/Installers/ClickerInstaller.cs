using App.Application.Clicker;
using App.Domain.Clicker;
using App.Presentation.Clicker;
using UnityEngine;
using Zenject;

namespace App.Bootstrap.Installers
{
    public sealed class ClickerInstaller : MonoInstaller
    {
        [SerializeField] private ClickerView _view;
        [SerializeField] private FlyingCurrencyView _flyingCurrencyPrefab;
        [SerializeField] private RectTransform _flyingCurrencyRoot;
        [SerializeField, Min(0)] private int _flyingCurrencyPoolSize = 12;

        public override void InstallBindings()
        {
            // Domain
            Container.Bind<Wallet>()
                .FromMethod(context => new Wallet(context.Container.Resolve<ClickerSettings>().StartBalance))
                .AsSingle();
            Container.Bind<Energy>()
                .FromMethod(context => Energy.Full(context.Container.Resolve<ClickerSettings>().MaxEnergy))
                .AsSingle();
            Container.Bind<CurrencyCollector>().AsSingle();

            // Application
            Container.BindInterfacesTo<ClickerState>().AsSingle();
            Container.BindInterfacesTo<CollectCurrencyUseCase>().AsSingle();
            Container.BindInterfacesTo<AutoCollectService>().AsSingle();
            Container.BindInterfacesTo<EnergyRegenerationService>().AsSingle();

            // Presentation
            Container.Bind<IClickerView>().FromInstance(_view).AsSingle();
            Container.BindInterfacesTo<ClickerPresenter>().AsSingle();
            Container.BindMemoryPool<FlyingCurrencyView, FlyingCurrencyView.Pool>()
                .WithInitialSize(_flyingCurrencyPoolSize)
                .FromComponentInNewPrefab(_flyingCurrencyPrefab)
                .UnderTransform(_flyingCurrencyRoot);
        }
    }
}
