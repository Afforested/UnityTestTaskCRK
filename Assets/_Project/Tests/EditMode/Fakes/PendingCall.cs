using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Tests.Fakes
{
    public sealed class PendingCall<T>
    {
        private readonly UniTaskCompletionSource<T> _completion = new UniTaskCompletionSource<T>();

        public PendingCall(string argument, CancellationToken cancellationToken)
        {
            Argument = argument;
            CancellationToken = cancellationToken;
            cancellationToken.Register(() => _completion.TrySetCanceled(cancellationToken));
        }

        public string Argument { get; }

        public CancellationToken CancellationToken { get; }

        public bool IsCancelled => CancellationToken.IsCancellationRequested;

        public UniTask<T> Task => _completion.Task;

        public void Complete(T result) => _completion.TrySetResult(result);

        public void Fail(System.Exception exception) => _completion.TrySetException(exception);
    }
}
