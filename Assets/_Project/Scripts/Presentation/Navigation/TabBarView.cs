using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace App.Presentation.Navigation
{
    public sealed class TabBarView : MonoBehaviour, ITabBarView
    {
        [SerializeField] private TabButtonView[] _buttons;

        private IObservable<TabId> _tabSelected;

        public IObservable<TabId> TabSelected =>
            _tabSelected ??= _buttons.Select(button => button.Clicked.Select(_ => button.Tab)).Merge();

        public void SetSelected(TabId tab)
        {
            foreach (var button in _buttons)
                button.SetSelected(button.Tab == tab);
        }
    }
}
