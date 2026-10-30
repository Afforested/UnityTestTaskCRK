using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Presentation.Dogs;
using App.Presentation.Navigation;
using App.Presentation.Popup;
using App.Presentation.Weather;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace App.Tests.Fakes
{
    public sealed class FakeDogsView : IDogsView
    {
        public Subject<string> BreedClicks { get; } = new Subject<string>();

        public Subject<Unit> RetryClicks { get; } = new Subject<Unit>();

        public bool IsVisible { get; private set; }

        public bool IsLoadingShown { get; private set; }

        public string Error { get; private set; }

        public List<BreedListItem> Breeds { get; } = new List<BreedListItem>();

        public HashSet<string> LoadingBreeds { get; } = new HashSet<string>();

        public IObservable<string> BreedClicked => BreedClicks;

        public IObservable<Unit> RetryClicked => RetryClicks;

        public void Show() => IsVisible = true;

        public void Hide() => IsVisible = false;

        public void ShowLoading()
        {
            IsLoadingShown = true;
            Error = null;
            Breeds.Clear();
        }

        public void ShowBreeds(IReadOnlyList<BreedListItem> breeds)
        {
            IsLoadingShown = false;
            Breeds.Clear();
            Breeds.AddRange(breeds);
        }

        public void ShowError(string message)
        {
            IsLoadingShown = false;
            Error = message;
        }

        public void SetBreedLoading(string breedId, bool isLoading)
        {
            if (isLoading)
                LoadingBreeds.Add(breedId);
            else
                LoadingBreeds.Remove(breedId);
        }
    }

    public sealed class FakeWeatherView : IWeatherView
    {
        public bool IsVisible { get; private set; }

        public bool IsLoadingShown { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Error { get; private set; }

        public void Show() => IsVisible = true;

        public void Hide() => IsVisible = false;

        public void ShowLoading() => IsLoadingShown = true;

        public void ShowForecast(Sprite icon, string title, string description, string updatedAt)
        {
            IsLoadingShown = false;
            Title = title;
            Description = description;
        }

        public void ShowError(string message)
        {
            IsLoadingShown = false;
            Error = message;
        }
    }

    public sealed class FakeTabBarView : ITabBarView
    {
        public Subject<TabId> Selections { get; } = new Subject<TabId>();

        public TabId? Selected { get; private set; }

        public IObservable<TabId> TabSelected => Selections;

        public void SetSelected(TabId tab) => Selected = tab;
    }

    public sealed class FakeTabPresenter : ITabPresenter
    {
        private readonly List<string> _journal;

        public FakeTabPresenter(TabId id, List<string> journal)
        {
            Id = id;
            _journal = journal;
        }

        public TabId Id { get; }

        public bool IsOpen { get; private set; }

        public void Open()
        {
            IsOpen = true;
            _journal.Add($"open {Id}");
        }

        public void Close()
        {
            IsOpen = false;
            _journal.Add($"close {Id}");
        }
    }

    public sealed class FakePopupService : IPopupService
    {
        public List<(string Title, string Message)> Shown { get; } = new List<(string Title, string Message)>();

        public (string Title, string Message) Last => Shown.Last();

        public UniTask ShowAsync(string title, string message, CancellationToken cancellationToken = default)
        {
            Shown.Add((title, message));
            return UniTask.CompletedTask;
        }
    }
}
