using System;

namespace App.Domain.Clicker
{
    public sealed class Energy
    {
        public Energy(int max, int initial)
        {
            if (max <= 0)
                throw new ArgumentOutOfRangeException(nameof(max), max, "Max energy must be positive.");
            if (initial < 0 || initial > max)
                throw new ArgumentOutOfRangeException(nameof(initial), initial, $"Initial energy must be within [0, {max}].");

            Max = max;
            Current = initial;
        }

        public static Energy Full(int max) => new Energy(max, max);

        public event Action<int> Changed;

        public int Max { get; }

        public int Current { get; private set; }

        public bool IsFull => Current >= Max;

        public bool TrySpend(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must not be negative.");

            if (Current < amount)
                return false;

            if (amount > 0)
            {
                Current -= amount;
                Changed?.Invoke(Current);
            }

            return true;
        }

     
        public int Restore(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must not be negative.");

            var restored = Math.Min(amount, Max - Current);
            if (restored == 0)
                return 0;

            Current += restored;
            Changed?.Invoke(Current);
            return restored;
        }
    }
}
