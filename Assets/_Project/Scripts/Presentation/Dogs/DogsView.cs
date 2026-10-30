using System;
using System.Collections.Generic;
using App.Presentation.Common;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Presentation.Dogs
{
    public sealed class DogsView : ScreenView, IDogsView
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _loader;
        [SerializeField] private GameObject _errorPanel;
        [SerializeField] private TMP_Text _errorText;
        [SerializeField] private Button _retryButton;
        [SerializeField, Min(0f)] private float _appearStagger = 0.04f;

        private readonly Subject<string> _breedClicked = new Subject<string>();
        private readonly List<BreedItemView> _items = new List<BreedItemView>();
        private readonly CompositeDisposable _itemSubscriptions = new CompositeDisposable();

        private BreedItemView.Pool _itemPool;

        [Inject]
        public void Construct(BreedItemView.Pool itemPool) => _itemPool = itemPool;

        public IObservable<string> BreedClicked => _breedClicked;

        public IObservable<Unit> RetryClicked => _retryButton.OnClickAsObservable();

        public void ShowLoading()
        {
            ClearItems();
            _errorPanel.SetActive(false);
            _loader.SetActive(true);
        }

        public void ShowBreeds(IReadOnlyList<BreedListItem> breeds)
        {
            ClearItems();
            _errorPanel.SetActive(false);
            _loader.SetActive(false);

            for (var i = 0; i < breeds.Count; i++)
            {
                var item = _itemPool.Spawn();
                item.transform.SetAsLastSibling();
                item.Setup(breeds[i].BreedId, breeds[i].Title);
                item.Clicked.Subscribe(_ => _breedClicked.OnNext(item.BreedId)).AddTo(_itemSubscriptions);
                item.PlayAppear(i * _appearStagger);
                _items.Add(item);
            }

            _scrollRect.verticalNormalizedPosition = 1f;
        }

        public void ShowError(string message)
        {
            ClearItems();
            _loader.SetActive(false);
            _errorText.text = message;
            _errorPanel.SetActive(true);
        }

        public void SetBreedLoading(string breedId, bool isLoading)
        {
            var item = _items.Find(row => row.BreedId == breedId);
            if (item != null)
                item.SetLoading(isLoading);
        }

        private void ClearItems()
        {
            _itemSubscriptions.Clear();
            foreach (var item in _items)
                _itemPool.Despawn(item);

            _items.Clear();
        }

        private void OnDestroy()
        {
            _itemSubscriptions.Dispose();
            _breedClicked.Dispose();
        }
    }
}
