using UniRx;

namespace App.Application.Clicker
{
    public interface IAutoCollectService
    {
        IReadOnlyReactiveProperty<float> Progress { get; }
    }
}
