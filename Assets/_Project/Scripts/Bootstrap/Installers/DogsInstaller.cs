using App.Application.Dogs;
using App.Infrastructure.Dogs;
using App.Presentation.Dogs;
using UnityEngine;
using Zenject;

namespace App.Bootstrap.Installers
{
    public sealed class DogsInstaller : MonoInstaller
    {
        [SerializeField] private DogsView _view;
        [SerializeField] private BreedItemView _breedItemPrefab;
        [SerializeField] private RectTransform _listContent;
        [SerializeField, Min(0)] private int _breedItemPoolSize = 10;

        public override void InstallBindings()
        {
            Container.Bind<IDogBreedsGateway>().To<DogBreedsGateway>().AsSingle();
            Container.Bind<IGetBreedsUseCase>().To<GetBreedsUseCase>().AsSingle();
            Container.Bind<IGetBreedDetailsUseCase>().To<GetBreedDetailsUseCase>().AsSingle();

            Container.Bind<IDogsView>().FromInstance(_view).AsSingle();
            Container.BindInterfacesTo<DogsPresenter>().AsSingle();
            Container.BindMemoryPool<BreedItemView, BreedItemView.Pool>()
                .WithInitialSize(_breedItemPoolSize)
                .FromComponentInNewPrefab(_breedItemPrefab)
                .UnderTransform(_listContent);
        }
    }
}
