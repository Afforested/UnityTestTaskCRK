using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Clicker
{
    public sealed class ClickButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [Tooltip("Animated child, so the clickable area and the shadow stay still while the visual moves.")]
        [SerializeField] private RectTransform _visual;
        [SerializeField] private Graphic _tintTarget;
        [Tooltip("How deep the visual sinks, as a share of its height. Matches the shadow offset.")]
        [SerializeField, Range(0f, 0.2f)] private float _pressDepth = 0.042f;
        [SerializeField, Min(0.05f)] private float _pressDuration = 0.26f;
        [SerializeField, Min(0f)] private float _deniedShakeStrength = 24f;
        [SerializeField] private Color _deniedColor = new Color(1f, 0.44f, 0.35f, 1f);

        private Color _normalColor;
        private Vector2 _restPosition;

        public IObservable<Unit> Clicked => _button.OnClickAsObservable();

        public void PlayPress()
        {
            _visual.DOComplete();
            var pressedPosition = _restPosition - new Vector2(0f, _visual.rect.height * _pressDepth);

            DOTween.Sequence()
                .Append(_visual.DOAnchorPos(pressedPosition, _pressDuration * 0.25f).SetEase(Ease.OutQuad))
                .Append(_visual.DOAnchorPos(_restPosition, _pressDuration * 0.75f).SetEase(Ease.OutBack, 3f))
                .SetTarget(_visual)
                .SetLink(gameObject);
        }

        public void PlayDenied()
        {
            _visual.DOComplete();
            _visual.DOShakeAnchorPos(0.35f, new Vector2(_deniedShakeStrength, 0f), 16, 0f).SetLink(gameObject);

            _tintTarget.DOKill();
            _tintTarget.color = _normalColor;
            _tintTarget.DOColor(_deniedColor, 0.08f).SetLoops(2, LoopType.Yoyo).SetLink(gameObject);
        }

        private void Awake()
        {
            _normalColor = _tintTarget.color;
            _restPosition = _visual.anchoredPosition;
        }
    }
}
