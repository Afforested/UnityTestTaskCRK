using System;
using System.Collections.Generic;
using App.Presentation.Common;
using UniRx;

namespace App.Presentation.Dogs
{
    public interface IDogsView : IScreenView
    {
        IObservable<string> BreedClicked { get; }

        IObservable<Unit> RetryClicked { get; }

        void ShowLoading();

        void ShowBreeds(IReadOnlyList<BreedListItem> breeds);

        void ShowError(string message);

        void SetBreedLoading(string breedId, bool isLoading);
    }
}
