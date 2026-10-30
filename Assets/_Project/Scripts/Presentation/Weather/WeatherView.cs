using App.Presentation.Common;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Weather
{
    public sealed class WeatherView : ScreenView, IWeatherView
    {
        [SerializeField] private GameObject _loader;
        [SerializeField] private RectTransform _forecastCard;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _placeholderIcon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _updatedAt;
        [Tooltip("Holds the error text and its background.")]
        [SerializeField] private GameObject _errorPanel;
        [SerializeField] private TMP_Text _error;

        public void ShowLoading()
        {
            _loader.SetActive(true);
            _forecastCard.gameObject.SetActive(false);
            _errorPanel.SetActive(false);
        }

        public void ShowForecast(Sprite icon, string title, string description, string updatedAt)
        {
            var isFirstAppearance = !_forecastCard.gameObject.activeSelf;

            _loader.SetActive(false);
            _errorPanel.SetActive(false);
            _forecastCard.gameObject.SetActive(true);

            _icon.sprite = icon != null ? icon : _placeholderIcon;
            _title.text = title;
            _description.text = description;
            _updatedAt.text = updatedAt;

            if (isFirstAppearance)
            {
                _forecastCard.DOKill();
                _forecastCard.localScale = Vector3.one * 0.9f;
                _forecastCard.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetLink(gameObject);
            }
            else
            {
                _updatedAt.rectTransform.DOComplete();
                _updatedAt.rectTransform.DOPunchScale(Vector3.one * 0.12f, 0.3f, 6, 0.5f).SetLink(gameObject);
            }
        }

        public void ShowError(string message)
        {
            _loader.SetActive(false);
            _error.text = message;
            _errorPanel.SetActive(true);
        }
    }
}
