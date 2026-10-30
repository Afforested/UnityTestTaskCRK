using System;

namespace App.Presentation.Navigation
{
    public interface ITabBarView
    {
        IObservable<TabId> TabSelected { get; }

        void SetSelected(TabId tab);
    }
}
