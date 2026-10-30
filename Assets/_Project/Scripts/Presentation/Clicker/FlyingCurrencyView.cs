using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace App.Presentation.Clicker
{
    public sealed class FlyingCurrencyView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _amount;
        [SerializeField, Min(0.1f)] private float _duration = 0.8f;
        [SerializeField, Min(0f)] private float _riseHeight = 170f;
        [SerializeField, Min(0f)] private float _horizontalSpread = 80f;

        private Sequence _sequence;

        public void Fly(string text, Vector3 worldFrom, Vector3 worldTo, Action onArrived)
        {
            Stop();
            _amount.text = text;

            var parent = _rectTransform.parent;
            var start = parent.InverseTransformPoint(worldFrom);
            start.x += Random.Range(-_horizontalSpread, _horizontalSpread);
            start.z = 0f;
            var apex = start + new Vector3(0f, _riseHeight, 0f);
            var target = parent.InverseTransformPoint(worldTo);
            target.z = 0f;

            _rectTransform.localPosition = start;
            _rectTransform.localScale = Vector3.one * 0.6f;
            _canvasGroup.alpha = 1f;

            var riseDuration = _duration * 0.35f;
            var flyDuration = _duration - riseDuration;

            _sequence = DOTween.Sequence()
                .Append(_rectTransform.DOLocalMove(apex, riseDuration).SetEase(Ease.OutCubic))
                .Join(_rectTransform.DOScale(1.1f, riseDuration).SetEase(Ease.OutBack))
                .Append(_rectTransform.DOLocalMove(target, flyDuration).SetEase(Ease.InCubic))
                .Join(_rectTransform.DOScale(0.5f, flyDuration).SetEase(Ease.InQuad))
                .Insert(_duration * 0.8f, _canvasGroup.DOFade(0f, _duration * 0.2f))
                .SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable)
                .OnComplete(() => onArrived?.Invoke());
        }

        private void Stop()
        {
            _sequence?.Kill();
            _sequence = null;
        }

        public sealed class Pool : MonoMemoryPool<FlyingCurrencyView>
        {
            protected override void OnDespawned(FlyingCurrencyView item)
            {
                item.Stop();
                base.OnDespawned(item);
            }
        }
    }
}
