using System;

namespace App.Domain.Clicker
{
    public sealed class Wallet
    {
        public Wallet(int initialBalance)
        {
            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), initialBalance, "Balance must not be negative.");

            Balance = initialBalance;
        }

        public event Action<int> BalanceChanged;

        public int Balance { get; private set; }

        public void Add(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must not be negative.");

            if (amount == 0)
                return;

            Balance = checked(Balance + amount);
            BalanceChanged?.Invoke(Balance);
        }
    }
}
