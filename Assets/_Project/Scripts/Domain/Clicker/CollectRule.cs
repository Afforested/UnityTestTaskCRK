using System;

namespace App.Domain.Clicker
{
    public readonly struct CollectRule
    {
        public CollectRule(int reward, int energyCost)
        {
            if (reward < 0)
                throw new ArgumentOutOfRangeException(nameof(reward), reward, "Reward must not be negative.");
            if (energyCost < 0)
                throw new ArgumentOutOfRangeException(nameof(energyCost), energyCost, "Energy cost must not be negative.");

            Reward = reward;
            EnergyCost = energyCost;
        }

        public int Reward { get; }

        public int EnergyCost { get; }
    }
}
