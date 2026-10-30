using UniRx;

namespace App.Application.Clicker
{
    public interface IEnergyRegenerationService
    {
        int RestoreAmount { get; }

        IReadOnlyReactiveProperty<float> SecondsUntilRestore { get; }
    }
}
