using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Navigation
{
    public sealed class TabButtonView : MonoBehaviour
    {
        [SerializeField] private TabId _tab;
        [SerializeField] private Button _button;
        [SerializeField] private Graphic _icon;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Graphic _selectionBackground;
        [Tooltip("Label colors.")]
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _normalColor = Color.gray;
        [SerializeField] private Color _selectedIconColor = Color.white;
        [SerializeField] private Color _normalIconColor = Color.white;
        [SerializeField, Min(0f)] private float _animationDuration = 0.2f;
        [SerializeField, Min(1f)] private float _selectedIconScale = 1.15f;

        public TabId Tab => _tab;

        public IObservable<Unit> Clicked => _button.OnClickAsObservable();

        public void SetSelected(bool isSelected)
        {
            _icon.DOKill();
            _icon.DOColor(isSelected ? _selectedIconColor : _normalIconColor, _animationDuration).SetLink(gameObject);
            _icon.rectTransform.DOKill();
            _icon.rectTransform.DOScale(isSelected ? _selectedIconScale : 1f, _animationDuration).SetEase(Ease.OutBack).SetLink(gameObject);

            _label.DOKill();
            _label.DOColor(isSelected ? _selectedColor : _normalColor, _animationDuration).SetLink(gameObject);

            _selectionBackground.DOKill();
            _selectionBackground.DOFade(isSelected ? 1f : 0f, _animationDuration).SetLink(gameObject);
        }
    }
}
