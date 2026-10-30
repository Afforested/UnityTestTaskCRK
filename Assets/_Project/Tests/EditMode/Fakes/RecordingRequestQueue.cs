using System.Collections.Generic;
using System.Threading;
using App.Infrastructure.Network;
using Cysharp.Threading.Tasks;

namespace App.Tests.Fakes
{
    public sealed class RecordingRequestQueue : IRequestQueue
    {
        private readonly object _response;

        public RecordingRequestQueue(object response)
        {
            _response = response;
        }

        public List<object> Requests { get; } = new List<object>();

        public int PendingCount => 0;

        public bool IsBusy => false;

        public UniTask<TResult> Enqueue<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return UniTask.FromResult((TResult)_response);
        }
    }
}
