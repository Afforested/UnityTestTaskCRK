using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Infrastructure.Network
{
    public interface IRequestQueue
    {
        int PendingCount { get; }
        bool IsBusy { get; }
        UniTask<TResult> Enqueue<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
    }
}
