using System;
using App.Domain.Clicker;
using UniRx;

namespace App.Application.Clicker
{
    public sealed class ClickerState : IClickerState, IDisposable
    {
        private readonly Wallet _wallet;
        private readonly Energy _energy;
        private readonly ReactiveProperty<int> _balance;
        private readonly ReactiveProperty<int> _currentEnergy;

        public ClickerState(Wallet wallet, Energy energy)
        {
            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            _energy = energy ?? throw new ArgumentNullException(nameof(energy));

            _balance = new ReactiveProperty<int>(_wallet.Balance);
            _currentEnergy = new ReactiveProperty<int>(_energy.Current);

            _wallet.BalanceChanged += OnBalanceChanged;
            _energy.Changed += OnEnergyChanged;
        }

        public IReadOnlyReactiveProperty<int> Balance => _balance;

        public IReadOnlyReactiveProperty<int> CurrentEnergy => _currentEnergy;

        public int MaxEnergy => _energy.Max;

        public void Dispose()
        {
            _wallet.BalanceChanged -= OnBalanceChanged;
            _energy.Changed -= OnEnergyChanged;
            _balance.Dispose();
            _currentEnergy.Dispose();
        }

        private void OnBalanceChanged(int balance) => _balance.Value = balance;

        private void OnEnergyChanged(int energy) => _currentEnergy.Value = energy;
    }
}
