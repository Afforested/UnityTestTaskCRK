using System.Collections.Generic;
using App.Presentation.Navigation;
using App.Tests.Fakes;
using NUnit.Framework;

namespace App.Tests.Presentation
{
    public sealed class TabNavigationPresenterTests
    {
        private List<string> _journal;
        private FakeTabBarView _view;
        private TabNavigationPresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            _journal = new List<string>();
            _view = new FakeTabBarView();
            var tabs = new List<ITabPresenter>
            {
                new FakeTabPresenter(TabId.Clicker, _journal),
                new FakeTabPresenter(TabId.Weather, _journal),
                new FakeTabPresenter(TabId.Dogs, _journal),
            };
            _presenter = new TabNavigationPresenter(_view, tabs);
        }

        [TearDown]
        public void TearDown() => _presenter.Dispose();

        [Test]
        public void Initialize_ClosesAllTabs_AndOpensClicker()
        {
            _presenter.Initialize();

            Assert.That(_journal, Is.EqualTo(new[] { "close Clicker", "close Weather", "close Dogs", "open Clicker" }));
            Assert.That(_view.Selected, Is.EqualTo(TabId.Clicker));
        }

        [Test]
        public void SelectingTab_ClosesCurrentTabBeforeOpeningNext()
        {
            _presenter.Initialize();
            _journal.Clear();

            _view.Selections.OnNext(TabId.Weather);

            Assert.That(_journal, Is.EqualTo(new[] { "close Clicker", "open Weather" }));
            Assert.That(_view.Selected, Is.EqualTo(TabId.Weather));
            Assert.That(_presenter.CurrentTab, Is.EqualTo(TabId.Weather));
        }

        [Test]
        public void SelectingCurrentTab_DoesNothing()
        {
            _presenter.Initialize();
            _journal.Clear();

            _view.Selections.OnNext(TabId.Clicker);

            Assert.That(_journal, Is.Empty);
        }
    }
}
