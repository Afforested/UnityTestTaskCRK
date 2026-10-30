using DG.Tweening;
using UnityEngine;

namespace App.Presentation.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenView : MonoBehaviour, IScreenView
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField, Min(0f)] private float _fadeDuration = 0.2f;

        public void Show()
        {
            _canvasGroup.DOKill();
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, _fadeDuration).SetLink(gameObject);
        }

        public void Hide()
        {
            _canvasGroup.DOKill();
            gameObject.SetActive(false);
        }

        protected virtual void Reset() => _canvasGroup = GetComponent<CanvasGroup>();
    }
}
