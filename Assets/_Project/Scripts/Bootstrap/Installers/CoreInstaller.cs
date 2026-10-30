using App.Application.Images;
using App.Infrastructure.Images;
using App.Infrastructure.Network;
using App.Presentation.Navigation;
using App.Presentation.Popup;
using UnityEngine;
using Zenject;

namespace App.Bootstrap.Installers
{
    public sealed class CoreInstaller : MonoInstaller
    {
        [SerializeField] private TabBarView _tabBarView;
        [SerializeField] private InfoPopupView _popupView;

        public override void InstallBindings()
        {
            Container.Bind<ILogger>().FromInstance(Debug.unityLogger).AsSingle();
            Container.BindInterfacesTo<RequestQueue>().AsSingle();
            Container.BindInterfacesTo<UpdatableTicker>().AsSingle();

            Container.Bind<IImageGateway>().To<ImageGateway>().AsSingle();
            Container.Bind<ILoadImageUseCase>().To<LoadImageUseCase>().AsSingle();

            Container.Bind<ITabBarView>().FromInstance(_tabBarView).AsSingle();
            Container.BindInterfacesTo<TabNavigationPresenter>().AsSingle();
            Container.BindExecutionOrder<TabNavigationPresenter>(100);

            Container.Bind<IPopupView>().FromInstance(_popupView).AsSingle();
            Container.BindInterfacesTo<PopupPresenter>().AsSingle();
        }
    }
}
