using App.Application.Clicker;
using App.Domain.Clicker;

namespace App.Tests.Fakes
{
    public static class TestSettings
    {
        public static ClickerSettings Clicker(
            int startBalance = 0,
            int maxEnergy = 1000,
            int tapReward = 1,
            int tapEnergyCost = 1,
            int autoReward = 1,
            int autoEnergyCost = 1,
            float autoCollectInterval = 3f,
            int energyRegenAmount = 10,
            float energyRegenInterval = 10f) =>
            new ClickerSettings(
                startBalance,
                maxEnergy,
                new CollectRule(tapReward, tapEnergyCost),
                new CollectRule(autoReward, autoEnergyCost),
                autoCollectInterval,
                energyRegenAmount,
                energyRegenInterval);
    }
}
