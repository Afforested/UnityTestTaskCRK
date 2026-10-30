using System;
using App.Presentation.Common;
using UniRx;

namespace App.Presentation.Clicker
{
    public interface IClickerView : IScreenView
    {
        IObservable<Unit> ButtonClicked { get; }

        void SetBalance(string text);

        void SetIncomeHint(string text);
        
        void SetEnergy(string current, string max, float fill);

        void SetEnergyHint(string text);

        void SetAutoCollectProgress(float progress);

        void PlayCollect(string rewardText);

        void PlayNotEnoughEnergy();
    }
}
