using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Presentation.Dogs
{
    public sealed class BreedItemView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private GameObject _loader;
        [Tooltip("Shown instead of the loader while the row is idle.")]
        [SerializeField] private GameObject _idleIcon;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Loading look: the row stays pressed into its shadow")]
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _loadingSprite;
        [SerializeField] private Color _loadingColor = Color.yellow;
        [Tooltip("Moved by the shadow offset together with the background.")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private Vector2 _loadingOffset = new Vector2(9f, -9f);

        private Sprite _idleSprite;
        private Color _idleColor;
        private Vector2 _idlePosition;

        public string BreedId { get; private set; }

        public IObservable<Unit> Clicked => _button.OnClickAsObservable();

        public void Setup(string breedId, string title)
        {
            BreedId = breedId;
            _title.text = title;
            SetLoading(false);
        }

        public void SetLoading(bool isLoading)
        {
            _loader.SetActive(isLoading);
            _idleIcon.SetActive(!isLoading);

            _background.sprite = isLoading ? _loadingSprite : _idleSprite;
            _background.color = isLoading ? _loadingColor : _idleColor;
            _content.anchoredPosition = isLoading ? _idlePosition + _loadingOffset : _idlePosition;
        }

        public void PlayAppear(float delay)
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, 0.25f).SetDelay(delay).SetLink(gameObject);
        }

        private void Awake()
        {
            _idleSprite = _background.sprite;
            _idleColor = _background.color;
            _idlePosition = _content.anchoredPosition;
        }

        private void ResetState()
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            BreedId = null;
            SetLoading(false);
        }

        public sealed class Pool : MonoMemoryPool<BreedItemView>
        {
            protected override void OnDespawned(BreedItemView item)
            {
                item.ResetState();
                base.OnDespawned(item);
            }
        }
    }
}
