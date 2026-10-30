using System;
using System.Linq;
using App.Domain.Dogs;
using App.Presentation.Dogs;
using App.Tests.Fakes;
using NUnit.Framework;

namespace App.Tests.Presentation
{
    public sealed class DogsPresenterTests
    {
        private FakeDogsView _view;
        private FakeGetBreedsUseCase _getBreeds;
        private FakeGetBreedDetailsUseCase _getDetails;
        private FakePopupService _popup;
        private DogsPresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            _view = new FakeDogsView();
            _getBreeds = new FakeGetBreedsUseCase();
            _getDetails = new FakeGetBreedDetailsUseCase();
            _popup = new FakePopupService();
            _presenter = new DogsPresenter(_view, _getBreeds, _getDetails, _popup, TestLogger.Create());
            _presenter.Initialize();
        }

        [TearDown]
        public void TearDown() => _presenter.Dispose();

        [Test]
        public void Open_ShowsLoader_AndRequestsBreeds()
        {
            _presenter.Open();

            Assert.That(_view.IsVisible, Is.True);
            Assert.That(_view.IsLoadingShown, Is.True);
            Assert.That(_getBreeds.Calls, Has.Count.EqualTo(1));
        }

        [Test]
        public void LoadedBreeds_AreShownAsNumberedList()
        {
            OpenWithBreeds("Akita", "Beagle");

            Assert.That(_view.IsLoadingShown, Is.False);
            Assert.That(_view.Breeds.Select(item => item.Title), Is.EqualTo(new[] { "1 - Akita", "2 - Beagle" }));
        }

        [Test]
        public void ClickOnBreed_ShowsRowLoader_AndRequestsBreedById()
        {
            OpenWithBreeds("Akita", "Beagle");

            _view.BreedClicks.OnNext("id-0");

            Assert.That(_view.LoadingBreeds, Is.EquivalentTo(new[] { "id-0" }));
            Assert.That(_getDetails.LastCall.Argument, Is.EqualTo("id-0"));
        }

        [Test]
        public void LoadedBreedDetails_HideLoader_AndShowPopupWithNameAndDescription()
        {
            OpenWithBreeds("Akita");
            _view.BreedClicks.OnNext("id-0");

            _getDetails.LastCall.Complete(new BreedDetails("id-0", "Akita", "Loyal and brave."));

            Assert.That(_view.LoadingBreeds, Is.Empty);
            Assert.That(_popup.Last, Is.EqualTo(("Akita", "Loyal and brave.")));
        }

        [Test]
        public void ClickOnAnotherBreed_CancelsPreviousRequest_MovesLoader_AndRequestsNewBreed()
        {
            OpenWithBreeds("Akita", "Beagle");
            _view.BreedClicks.OnNext("id-0");
            var previous = _getDetails.LastCall;

            _view.BreedClicks.OnNext("id-1");

            Assert.That(previous.IsCancelled, Is.True, "The previous request must be cancelled or removed from the queue.");
            Assert.That(_view.LoadingBreeds, Is.EquivalentTo(new[] { "id-1" }));
            Assert.That(_getDetails.Calls, Has.Count.EqualTo(2));
            Assert.That(_getDetails.LastCall.Argument, Is.EqualTo("id-1"));
            Assert.That(_popup.Shown, Is.Empty);
        }

        [Test]
        public void ClickOnSameBreedWhileItLoads_DoesNotRequestItAgain()
        {
            OpenWithBreeds("Akita");
            _view.BreedClicks.OnNext("id-0");

            _view.BreedClicks.OnNext("id-0");

            Assert.That(_getDetails.Calls, Has.Count.EqualTo(1));
            Assert.That(_getDetails.LastCall.IsCancelled, Is.False);
        }

        [Test]
        public void Close_CancelsBreedsListRequest()
        {
            _presenter.Open();
            var breedsCall = _getBreeds.LastCall;

            _presenter.Close();

            Assert.That(breedsCall.IsCancelled, Is.True);
            Assert.That(_view.IsVisible, Is.False);
        }

        [Test]
        public void Close_CancelsBreedDetailsRequest_AndRemovesLoader()
        {
            OpenWithBreeds("Akita");
            _view.BreedClicks.OnNext("id-0");
            var detailsCall = _getDetails.LastCall;

            _presenter.Close();

            Assert.That(detailsCall.IsCancelled, Is.True);
            Assert.That(_view.LoadingBreeds, Is.Empty);
        }

        [Test]
        public void FailedBreedsRequest_ShowsError()
        {
            _presenter.Open();

            _getBreeds.LastCall.Fail(new InvalidOperationException("offline"));

            Assert.That(_view.Error, Is.Not.Null);
            Assert.That(_view.IsLoadingShown, Is.False);
        }

        [Test]
        public void ReopeningTab_RequestsBreedsAgain()
        {
            OpenWithBreeds("Akita");
            _presenter.Close();

            _presenter.Open();

            Assert.That(_getBreeds.Calls, Has.Count.EqualTo(2));
        }

        private void OpenWithBreeds(params string[] names)
        {
            _presenter.Open();
            _getBreeds.LastCall.Complete(names.Select((name, index) => new Breed("id-" + index, name)).ToList());
        }
    }
}
