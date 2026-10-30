using App.Application.Weather;
using App.Infrastructure.Weather;
using UnityEngine;

namespace App.Bootstrap.Configs
{
    [CreateAssetMenu(fileName = "WeatherConfig", menuName = "App/Configs/Weather")]
    public sealed class WeatherConfig : ScriptableObject
    {
        [SerializeField] private string _forecastUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
        [Tooltip("small, medium or large; empty keeps the size returned by the service.")]
        [SerializeField] private string _iconSize = "large";
        [SerializeField, Min(1f)] private float _pollIntervalSeconds = 5f;

        public WeatherSettings ToSettings() => new WeatherSettings(_pollIntervalSeconds);

        public WeatherApiSettings ToApiSettings() => new WeatherApiSettings(_forecastUrl, _iconSize);
    }
}
