using System;
using App.Domain.Clicker;

namespace App.Application.Clicker
{
    public sealed class ClickerSettings
    {
        public ClickerSettings(
            int startBalance,
            int maxEnergy,
            CollectRule manualCollect,
            CollectRule autoCollect,
            float autoCollectIntervalSeconds,
            int energyRegenAmount,
            float energyRegenIntervalSeconds)
        {
            if (startBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(startBalance), startBalance, "Must not be negative.");
            if (maxEnergy <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxEnergy), maxEnergy, "Must be positive.");
            if (autoCollectIntervalSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(autoCollectIntervalSeconds), autoCollectIntervalSeconds, "Must be positive.");
            if (energyRegenAmount < 0)
                throw new ArgumentOutOfRangeException(nameof(energyRegenAmount), energyRegenAmount, "Must not be negative.");
            if (energyRegenIntervalSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(energyRegenIntervalSeconds), energyRegenIntervalSeconds, "Must be positive.");

            StartBalance = startBalance;
            MaxEnergy = maxEnergy;
            ManualCollect = manualCollect;
            AutoCollect = autoCollect;
            AutoCollectIntervalSeconds = autoCollectIntervalSeconds;
            EnergyRegenAmount = energyRegenAmount;
            EnergyRegenIntervalSeconds = energyRegenIntervalSeconds;
        }

        public int StartBalance { get; }

        public int MaxEnergy { get; }

        public CollectRule ManualCollect { get; }

        public CollectRule AutoCollect { get; }

        public float AutoCollectIntervalSeconds { get; }

        public int EnergyRegenAmount { get; }

        public float EnergyRegenIntervalSeconds { get; }
    }
}
