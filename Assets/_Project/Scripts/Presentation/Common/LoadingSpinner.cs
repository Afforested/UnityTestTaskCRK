using UnityEngine;

namespace App.Presentation.Common
{
    public sealed class LoadingSpinner : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private float _degreesPerSecond = 360f;

        private void OnEnable() => _target.localRotation = Quaternion.identity;

        private void Update() => _target.Rotate(0f, 0f, -_degreesPerSecond * Time.unscaledDeltaTime);

        private void Reset() => _target = transform as RectTransform;
    }
}
