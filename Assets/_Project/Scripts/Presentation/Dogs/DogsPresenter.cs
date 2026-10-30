using System;
using System.Linq;
using System.Threading;
using App.Application.Dogs;
using App.Domain.Dogs;
using App.Presentation.Common;
using App.Presentation.Navigation;
using App.Presentation.Popup;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace App.Presentation.Dogs
{
   
    public sealed class DogsPresenter : ITabPresenter, IInitializable, IDisposable
    {
        private const string LogTag = nameof(DogsPresenter);

        private readonly IDogsView _view;
        private readonly IGetBreedsUseCase _getBreeds;
        private readonly IGetBreedDetailsUseCase _getBreedDetails;
        private readonly IPopupService _popup;
        private readonly ILogger _logger;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private CancellationTokenSource _openTabCancellation;
        private CancellationTokenSource _breedDetailsCancellation;
        private string _loadingBreedId;

        public DogsPresenter(
            IDogsView view,
            IGetBreedsUseCase getBreeds,
            IGetBreedDetailsUseCase getBreedDetails,
            IPopupService popup,
            ILogger logger)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _getBreeds = getBreeds ?? throw new ArgumentNullException(nameof(getBreeds));
            _getBreedDetails = getBreedDetails ?? throw new ArgumentNullException(nameof(getBreedDetails));
            _popup = popup ?? throw new ArgumentNullException(nameof(popup));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TabId Id => TabId.Dogs;

        public void Initialize()
        {
            _view.BreedClicked.Subscribe(OnBreedClicked).AddTo(_disposables);
            _view.RetryClicked.Subscribe(_ => LoadBreeds()).AddTo(_disposables);
        }

        public void Open()
        {
            _view.Show();
            _openTabCancellation = new CancellationTokenSource();
            LoadBreeds();
        }

        public void Close()
        {
            CancelBreedDetails();
            CancellationSourceUtility.CancelAndDispose(ref _openTabCancellation);
            _view.Hide();
        }

        public void Dispose()
        {
            _disposables.Dispose();
            CancellationSourceUtility.CancelAndDispose(ref _breedDetailsCancellation);
            CancellationSourceUtility.CancelAndDispose(ref _openTabCancellation);
        }

        private void LoadBreeds()
        {
            if (_openTabCancellation == null)
                return;

            LoadBreedsAsync(_openTabCancellation.Token).Forget();
        }

        private async UniTaskVoid LoadBreedsAsync(CancellationToken cancellationToken)
        {
            _view.ShowLoading();
            try
            {
                var breeds = await _getBreeds.ExecuteAsync(cancellationToken);
                var items = breeds
                    .Select((breed, index) => new BreedListItem(breed.Id, $"{index + 1} - {breed.Name}"))
                    .ToList();
                _view.ShowBreeds(items);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _logger.Log(LogType.Warning, LogTag, $"Breeds failed to load: {exception.Message}");
                _view.ShowError("Не удалось загрузить список пород.");
            }
        }

        private void OnBreedClicked(string breedId)
        {
            if (_openTabCancellation == null || breedId == _loadingBreedId)
                return;

            CancelBreedDetails();

            _loadingBreedId = breedId;
            _view.SetBreedLoading(breedId, true);
            _breedDetailsCancellation = CancellationTokenSource.CreateLinkedTokenSource(_openTabCancellation.Token);
            LoadBreedDetailsAsync(breedId, _breedDetailsCancellation.Token).Forget();
        }

        private async UniTaskVoid LoadBreedDetailsAsync(string breedId, CancellationToken cancellationToken)
        {
            BreedDetails details;
            try
            {
                details = await _getBreedDetails.ExecuteAsync(breedId, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception exception)
            {
                _logger.Log(LogType.Warning, LogTag, $"Breed {breedId} failed to load: {exception.Message}");
                CompleteBreedDetails(breedId);
                ShowPopup("Ошибка", "Не удалось загрузить информацию о породе. Попробуй ещё раз.");
                return;
            }

            CompleteBreedDetails(breedId);
            ShowPopup(details.Name, details.Description);
        }

        private void CompleteBreedDetails(string breedId)
        {
            if (_loadingBreedId != breedId)
                return;

            _view.SetBreedLoading(breedId, false);
            _loadingBreedId = null;
            CancellationSourceUtility.CancelAndDispose(ref _breedDetailsCancellation);
        }

        private void CancelBreedDetails()
        {
            if (_loadingBreedId != null)
            {
                _view.SetBreedLoading(_loadingBreedId, false);
                _loadingBreedId = null;
            }

            CancellationSourceUtility.CancelAndDispose(ref _breedDetailsCancellation);
        }

        private void ShowPopup(string title, string message)
        {
            var cancellationToken = _openTabCancellation?.Token ?? CancellationToken.None;
            _popup.ShowAsync(title, message, cancellationToken).Forget();
        }
    }
}
