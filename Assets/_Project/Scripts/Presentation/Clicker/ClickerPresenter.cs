using System;
using System.Globalization;
using App.Application.Clicker;
using App.Presentation.Navigation;
using UniRx;
using Zenject;

namespace App.Presentation.Clicker
{
    public sealed class ClickerPresenter : ITabPresenter, IInitializable, IDisposable
    {
        private static readonly NumberFormatInfo NumberFormat =
            NumberFormatInfo.ReadOnly(new NumberFormatInfo { NumberGroupSeparator = " " });

        private readonly IClickerView _view;
        private readonly IClickerState _state;
        private readonly ICollectCurrencyUseCase _collectCurrency;
        private readonly IAutoCollectService _autoCollect;
        private readonly IEnergyRegenerationService _energyRegeneration;
        private readonly ClickerSettings _settings;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isOpen;

        public ClickerPresenter(
            IClickerView view,
            IClickerState state,
            ICollectCurrencyUseCase collectCurrency,
            IAutoCollectService autoCollect,
            IEnergyRegenerationService energyRegeneration,
            ClickerSettings settings)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _collectCurrency = collectCurrency ?? throw new ArgumentNullException(nameof(collectCurrency));
            _autoCollect = autoCollect ?? throw new ArgumentNullException(nameof(autoCollect));
            _energyRegeneration = energyRegeneration ?? throw new ArgumentNullException(nameof(energyRegeneration));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public TabId Id => TabId.Clicker;

        public void Initialize()
        {
            _view.SetIncomeHint(
                $"+{_settings.ManualCollect.Reward} за тап и +{_settings.AutoCollect.Reward} " +
                $"каждые {_settings.AutoCollectIntervalSeconds.ToString("0.#", CultureInfo.InvariantCulture)} с");

            _view.ButtonClicked
                .Subscribe(_ => _collectCurrency.Execute(CollectSource.Manual))
                .AddTo(_disposables);

            _collectCurrency.Results
                .Subscribe(OnCollectAttempt)
                .AddTo(_disposables);

            _state.Balance
                .Subscribe(balance => _view.SetBalance(balance.ToString("N0", NumberFormat)))
                .AddTo(_disposables);

            _state.CurrentEnergy
                .Subscribe(_ => RefreshEnergy())
                .AddTo(_disposables);

            _autoCollect.Progress
                .Subscribe(_view.SetAutoCollectProgress)
                .AddTo(_disposables);

            _energyRegeneration.SecondsUntilRestore
                .Select(seconds => (int)Math.Ceiling(seconds))
                .DistinctUntilChanged()
                .Subscribe(_ => RefreshEnergyHint())
                .AddTo(_disposables);
        }

        public void Open()
        {
            _isOpen = true;
            _view.Show();
        }

        public void Close()
        {
            _isOpen = false;
            _view.Hide();
        }

        public void Dispose() => _disposables.Dispose();

        private void OnCollectAttempt(CollectResult result)
        {
            if (!_isOpen)
                return;

            if (result.IsSuccess)
                _view.PlayCollect("+" + result.Reward.ToString(NumberFormat));
            else if (result.Source == CollectSource.Manual)
                _view.PlayNotEnoughEnergy();
        }

        private void RefreshEnergy()
        {
            var current = _state.CurrentEnergy.Value;
            var max = _state.MaxEnergy;
            _view.SetEnergy(current.ToString("N0", NumberFormat), max.ToString("N0", NumberFormat), (float)current / max);
            RefreshEnergyHint();
        }

        private void RefreshEnergyHint()
        {
            if (_state.CurrentEnergy.Value >= _state.MaxEnergy)
            {
                _view.SetEnergyHint("Энергия заполнена");
                return;
            }

            var seconds = (int)Math.Ceiling(_energyRegeneration.SecondsUntilRestore.Value);
            _view.SetEnergyHint($"+{_energyRegeneration.RestoreAmount} через {seconds} с");
        }
    }
}
