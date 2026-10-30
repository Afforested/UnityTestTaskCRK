using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Popup
{

    public sealed class InfoPopupView : MonoBehaviour, IPopupView
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private VerticalLayoutGroup _panelLayout;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _message;
        [SerializeField] private ScrollRect _messageScroll;
        [SerializeField] private LayoutElement _messageLayout;
        [SerializeField] private Button _closeButton;
        [SerializeField, Range(0.3f, 0.95f)] private float _maxHeightFraction = 0.8f;
        [SerializeField, Min(0f)] private float _minMessageHeight = 80f;
        [SerializeField, Min(0f)] private float _animationDuration = 0.22f;

        public IObservable<Unit> CloseClicked => _closeButton.OnClickAsObservable();

        public void Show(string title, string message)
        {
            _title.text = title;
            _message.text = message;

            gameObject.SetActive(true);
            FitHeightToContent();

            _canvasGroup.DOKill();
            _panel.DOKill();
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 0f;
            _panel.localScale = Vector3.one * 0.85f;
            _canvasGroup.DOFade(1f, _animationDuration).SetLink(gameObject);
            _panel.DOScale(1f, _animationDuration).SetEase(Ease.OutBack).SetLink(gameObject);
        }

        public void Hide()
        {
            if (this == null || !gameObject.activeSelf)
                return;

            _canvasGroup.DOKill();
            _panel.DOKill();
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0f, _animationDuration * 0.8f)
                .SetLink(gameObject)
                .OnComplete(() => gameObject.SetActive(false));
            _panel.DOScale(0.9f, _animationDuration * 0.8f).SetEase(Ease.InQuad).SetLink(gameObject);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled)
                FitHeightToContent();
        }

        private void FitHeightToContent()
        {
            var contentWidth = _panel.rect.width - _panelLayout.padding.horizontal;
            var titleHeight = _title.GetPreferredValues(_title.text, contentWidth, 0f).y;
            var textHeight = _message.GetPreferredValues(_message.text, contentWidth, 0f).y;
            var buttonHeight = LayoutUtility.GetPreferredHeight((RectTransform)_closeButton.transform);
            var otherContentHeight = _panelLayout.padding.vertical
                + _panelLayout.spacing * (_panel.childCount - 1)
                + titleHeight
                + buttonHeight;

            var maxPanelHeight = ((RectTransform)_panel.parent).rect.height * _maxHeightFraction;
            var messageHeight = Mathf.Min(textHeight, Mathf.Max(_minMessageHeight, maxPanelHeight - otherContentHeight));

            _messageLayout.preferredHeight = messageHeight;
            _messageScroll.vertical = textHeight > messageHeight + 1f;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
            _messageScroll.verticalNormalizedPosition = 1f;
        }
    }
}
