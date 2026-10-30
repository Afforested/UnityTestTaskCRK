using UnityEngine;

namespace App.Presentation.Common
{
  
    [RequireComponent(typeof(RectTransform))]
    public sealed class ScaleToFit : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField, Min(0f)] private float _margin = 40f;

        private Rect _parentRect;
        private Vector2 _contentSize;

        private void LateUpdate()
        {
            var parentRect = ((RectTransform)transform.parent).rect;
            var contentSize = _content.rect.size;
            if (parentRect == _parentRect && contentSize == _contentSize)
                return;

            _parentRect = parentRect;
            _contentSize = contentSize;
            if (contentSize.x <= 0f || contentSize.y <= 0f)
                return;

            var position = (Vector2)transform.localPosition;
            var halfWidth = Mathf.Min(position.x - parentRect.xMin, parentRect.xMax - position.x) - _margin;
            var halfHeight = Mathf.Min(position.y - parentRect.yMin, parentRect.yMax - position.y) - _margin;
            var scale = Mathf.Clamp01(Mathf.Min(2f * halfWidth / contentSize.x, 2f * halfHeight / contentSize.y));
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
