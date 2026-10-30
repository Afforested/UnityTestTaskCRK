using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Infrastructure.Network
{
    public interface IRequest<TResult>
    {
        string Name { get; }

        UniTask<TResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}
