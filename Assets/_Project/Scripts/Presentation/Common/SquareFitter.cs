using UnityEngine;
using UnityEngine.EventSystems;

namespace App.Presentation.Common
{
 
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SquareFitter : UIBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField, Range(0.1f, 1f)] private float _fill = 0.85f;
        [SerializeField, Min(0f)] private float _maxSize = 640f;

        protected override void OnEnable()
        {
            base.OnEnable();
            Fit();
        }

        protected override void OnRectTransformDimensionsChange() => Fit();

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                    Fit();
            };
        }
#endif

        private void Fit()
        {
            if (_target == null)
                return;

            var area = ((RectTransform)transform).rect;
            var size = Mathf.Min(area.width, area.height) * _fill;
            if (_maxSize > 0f)
                size = Mathf.Min(size, _maxSize);

            _target.sizeDelta = new Vector2(size, size);
        }
    }
}
