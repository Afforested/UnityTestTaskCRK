using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Infrastructure.Network
{
    public sealed class RequestQueue : IRequestQueue, IDisposable
    {
        private const string LogTag = nameof(RequestQueue);

        private readonly LinkedList<Entry> _pending = new LinkedList<Entry>();
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private readonly ILogger _logger;
        private readonly bool _isVerbose;

        private Entry _running;
        private int _lastId;
        private bool _isProcessing;
        private bool _isDisposed;

        public RequestQueue(ILogger logger, NetworkSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _isVerbose = settings.LogRequests;
        }

        public int PendingCount => _pending.Count;

        public bool IsBusy => _running != null;

        public UniTask<TResult> Enqueue<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(RequestQueue));
            if (cancellationToken.IsCancellationRequested)
                return UniTask.FromCanceled<TResult>(cancellationToken);

            var entry = new Entry<TResult>(++_lastId, request, cancellationToken);
            entry.Node = _pending.AddLast(entry);
            entry.RegisterCancellation(OnEntryCancelled);
            Log($"{entry} enqueued, waiting: {_pending.Count}");

            if (!_isProcessing)
                ProcessAsync().Forget();

            return entry.Task;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            while (_pending.First != null)
            {
                var entry = _pending.First.Value;
                _pending.RemoveFirst();
                entry.CancelWaiting();
            }

            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private void OnEntryCancelled(Entry entry)
        {
            if (entry.State != EntryState.Waiting)
                return;

            _pending.Remove(entry.Node);
            entry.CancelWaiting();
            Log($"{entry} cancelled while waiting, removed from the queue, waiting: {_pending.Count}");
        }

        private async UniTaskVoid ProcessAsync()
        {
            _isProcessing = true;
            try
            {
                while (!_isDisposed && _pending.First != null)
                {
                    var entry = _pending.First.Value;
                    _pending.RemoveFirst();
                    _running = entry;

                    Log($"{entry} started");
                    var stopwatch = Stopwatch.StartNew();
                    await entry.RunAsync(_lifetime.Token);

                    _running = null;
                    LogCompletion(entry, stopwatch.ElapsedMilliseconds);

                    entry.Complete();
                }
            }
            finally
            {
                _running = null;
                _isProcessing = false;
            }
        }

        private void LogCompletion(Entry entry, long elapsedMilliseconds)
        {
            switch (entry.State)
            {
                case EntryState.Succeeded:
                    Log($"{entry} succeeded in {elapsedMilliseconds} ms, waiting: {_pending.Count}");
                    break;
                case EntryState.Cancelled:
                    Log($"{entry} aborted while running after {elapsedMilliseconds} ms, waiting: {_pending.Count}");
                    break;
                default:
                    _logger.Log(LogType.Warning, LogTag, $"{entry} failed in {elapsedMilliseconds} ms: {entry.Error?.Message}");
                    break;
            }
        }

        private void Log(string message)
        {
            if (_isVerbose)
                _logger.Log(LogType.Log, LogTag, message);
        }

        private enum EntryState
        {
            Waiting,
            Running,
            Succeeded,
            Failed,
            Cancelled,
        }

        private abstract class Entry
        {
            private readonly int _id;
            private readonly string _name;
            private CancellationTokenRegistration _registration;

            protected Entry(int id, string name, CancellationToken cancellationToken)
            {
                _id = id;
                _name = name;
                CancellationToken = cancellationToken;
            }

            public LinkedListNode<Entry> Node { get; set; }

            public EntryState State { get; protected set; } = EntryState.Waiting;

            public Exception Error { get; protected set; }

            protected CancellationToken CancellationToken { get; }

            public void RegisterCancellation(Action<Entry> onCancelled) =>
                _registration = CancellationToken.Register(() => onCancelled(this));

            public abstract UniTask RunAsync(CancellationToken queueToken);

            public abstract void Complete();

            public abstract void CancelWaiting();

            public override string ToString() => $"#{_id} '{_name}'";

            protected void ReleaseRegistration() => _registration.Dispose();
        }

        private sealed class Entry<TResult> : Entry
        {
            private readonly IRequest<TResult> _request;
            private readonly UniTaskCompletionSource<TResult> _completion = new UniTaskCompletionSource<TResult>();

            private TResult _result;

            public Entry(int id, IRequest<TResult> request, CancellationToken cancellationToken)
                : base(id, request.Name, cancellationToken)
            {
                _request = request;
            }

            public UniTask<TResult> Task => _completion.Task;

            public override async UniTask RunAsync(CancellationToken queueToken)
            {
                State = EntryState.Running;

                using (var linked = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, queueToken))
                {
                    try
                    {
                        _result = await _request.ExecuteAsync(linked.Token);
                        linked.Token.ThrowIfCancellationRequested();
                        State = EntryState.Succeeded;
                    }
                    catch (OperationCanceledException) when (linked.IsCancellationRequested)
                    {
                        State = EntryState.Cancelled;
                    }
                    catch (Exception exception)
                    {
                        State = EntryState.Failed;
                        Error = exception;
                    }
                }
            }

            public override void Complete()
            {
                ReleaseRegistration();

                switch (State)
                {
                    case EntryState.Succeeded:
                        _completion.TrySetResult(_result);
                        break;
                    case EntryState.Cancelled:
                        _completion.TrySetCanceled(CancellationToken);
                        break;
                    default:
                        _completion.TrySetException(Error);
                        break;
                }

                _result = default;
            }

            public override void CancelWaiting()
            {
                State = EntryState.Cancelled;
                ReleaseRegistration();
                _completion.TrySetCanceled(CancellationToken);
            }
        }
    }
}
