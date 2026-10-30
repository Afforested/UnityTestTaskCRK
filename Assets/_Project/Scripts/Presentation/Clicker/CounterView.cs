using DG.Tweening;
using TMPro;
using UnityEngine;

namespace App.Presentation.Clicker
{
    public sealed class CounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _value;
        [SerializeField] private RectTransform _icon;
        [SerializeField] private Vector3 _punch = new Vector3(0.3f, 0.3f, 0f);
        [SerializeField, Min(0.05f)] private float _punchDuration = 0.3f;
        [SerializeField] private Color _deniedColor = new Color(1f, 0.35f, 0.35f, 1f);

        private Color _normalColor;

        public void SetValue(string text) => _value.text = text;

        public void Punch()
        {
            _icon.DOComplete();
            _icon.DOPunchScale(_punch, _punchDuration, 8, 0.5f).SetLink(gameObject);
        }

        public void PlayDenied()
        {
            _value.DOKill();
            _value.color = _normalColor;
            _value.DOColor(_deniedColor, 0.1f).SetLoops(2, LoopType.Yoyo).SetLink(gameObject);

            _icon.DOComplete();
            _icon.DOPunchRotation(new Vector3(0f, 0f, 25f), 0.35f, 10, 0.5f).SetLink(gameObject);
        }

        private void Awake() => _normalColor = _value.color;
    }
}
