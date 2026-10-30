using UniRx;

namespace App.Application.Clicker
{
    public interface IClickerState
    {
        IReadOnlyReactiveProperty<int> Balance { get; }

        IReadOnlyReactiveProperty<int> CurrentEnergy { get; }

        int MaxEnergy { get; }
    }
}
