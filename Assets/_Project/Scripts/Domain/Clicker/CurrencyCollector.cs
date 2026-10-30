using System;

namespace App.Domain.Clicker
{
    public sealed class CurrencyCollector
    {
        private readonly Wallet _wallet;
        private readonly Energy _energy;

        public CurrencyCollector(Wallet wallet, Energy energy)
        {
            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            _energy = energy ?? throw new ArgumentNullException(nameof(energy));
        }

        public bool TryCollect(CollectRule rule)
        {
            if (!_energy.TrySpend(rule.EnergyCost))
                return false;

            _wallet.Add(rule.Reward);
            return true;
        }
    }
}
