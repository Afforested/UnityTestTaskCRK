using System;
using UniRx;

namespace App.Presentation.Popup
{
    public interface IPopupView
    {
        IObservable<Unit> CloseClicked { get; }

        void Show(string title, string message);

        void Hide();
    }
}
