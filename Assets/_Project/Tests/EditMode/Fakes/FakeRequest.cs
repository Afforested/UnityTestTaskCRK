using System;
using System.Threading;
using App.Infrastructure.Network;
using Cysharp.Threading.Tasks;

namespace App.Tests.Fakes
{
    public sealed class FakeRequest<T> : IRequest<T>
    {
        private readonly UniTaskCompletionSource<T> _completion = new UniTaskCompletionSource<T>();

        public FakeRequest(string name = "fake")
        {
            Name = name;
        }

        public string Name { get; }

        public int StartCount { get; private set; }

        public bool IsStarted => StartCount > 0;

        public CancellationToken ReceivedToken { get; private set; }

        public UniTask<T> ExecuteAsync(CancellationToken cancellationToken)
        {
            StartCount++;
            ReceivedToken = cancellationToken;
            cancellationToken.Register(() => _completion.TrySetCanceled(cancellationToken));
            return _completion.Task;
        }

        public void Complete(T result) => _completion.TrySetResult(result);

        public void Fail(Exception exception) => _completion.TrySetException(exception);
    }
}
