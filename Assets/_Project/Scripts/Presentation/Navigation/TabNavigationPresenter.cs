using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

namespace App.Presentation.Navigation
{
    public sealed class TabNavigationPresenter : IInitializable, IDisposable
    {
        public const TabId DefaultTab = TabId.Clicker;

        private readonly ITabBarView _view;
        private readonly Dictionary<TabId, ITabPresenter> _tabs;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private ITabPresenter _current;

        public TabNavigationPresenter(ITabBarView view, List<ITabPresenter> tabs)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            if (tabs == null)
                throw new ArgumentNullException(nameof(tabs));

            _tabs = tabs.ToDictionary(tab => tab.Id);
        }

        public TabId? CurrentTab => _current?.Id;

        public void Initialize()
        {
            foreach (var tab in _tabs.Values)
                tab.Close();

            _view.TabSelected.Subscribe(Select).AddTo(_disposables);
            Select(DefaultTab);
        }

        public void Select(TabId tabId)
        {
            if (_current != null && _current.Id == tabId)
                return;

            if (!_tabs.TryGetValue(tabId, out var next))
                throw new ArgumentOutOfRangeException(nameof(tabId), tabId, "No presenter is registered for the tab.");

            _current?.Close();
            _current = next;
            _view.SetSelected(tabId);
            _current.Open();
        }

        public void Dispose() => _disposables.Dispose();
    }
}
