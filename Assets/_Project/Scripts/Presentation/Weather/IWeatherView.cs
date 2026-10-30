using App.Presentation.Common;
using UnityEngine;

namespace App.Presentation.Weather
{
    public interface IWeatherView : IScreenView
    {
        void ShowLoading();
 
        void ShowForecast(Sprite icon, string title, string description, string updatedAt);

        void ShowError(string message);
    }
}
