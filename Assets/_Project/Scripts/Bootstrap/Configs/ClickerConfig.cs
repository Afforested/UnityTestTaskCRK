using App.Application.Clicker;
using App.Domain.Clicker;
using UnityEngine;

namespace App.Bootstrap.Configs
{
    [CreateAssetMenu(fileName = "ClickerConfig", menuName = "App/Configs/Clicker")]
    public sealed class ClickerConfig : ScriptableObject
    {
        [Header("Currency")]
        [SerializeField, Min(0)] private int _startBalance;

        [Header("Energy")]
        [Tooltip("The player starts with full energy, so this is also the starting value.")]
        [SerializeField, Min(1)] private int _maxEnergy = 1000;
        [SerializeField, Min(0)] private int _energyRegenAmount = 10;
        [SerializeField, Min(0.1f)] private float _energyRegenIntervalSeconds = 10f;

        [Header("Tap")]
        [SerializeField, Min(0)] private int _tapReward = 1;
        [SerializeField, Min(0)] private int _tapEnergyCost = 1;

        [Header("Auto collect")]
        [SerializeField, Min(0.1f)] private float _autoCollectIntervalSeconds = 3f;
        [SerializeField, Min(0)] private int _autoCollectReward = 1;
        [SerializeField, Min(0)] private int _autoCollectEnergyCost = 1;

        public ClickerSettings ToSettings() => new ClickerSettings(
            startBalance: _startBalance,
            maxEnergy: _maxEnergy,
            manualCollect: new CollectRule(_tapReward, _tapEnergyCost),
            autoCollect: new CollectRule(_autoCollectReward, _autoCollectEnergyCost),
            autoCollectIntervalSeconds: _autoCollectIntervalSeconds,
            energyRegenAmount: _energyRegenAmount,
            energyRegenIntervalSeconds: _energyRegenIntervalSeconds);
    }
}
