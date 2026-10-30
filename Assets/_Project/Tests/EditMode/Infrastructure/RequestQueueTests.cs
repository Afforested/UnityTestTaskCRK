using System;
using System.Threading;
using App.Infrastructure.Network;
using App.Tests.Fakes;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace App.Tests.Infrastructure
{
    public sealed class RequestQueueTests
    {
        private RequestQueue _queue;

        [SetUp]
        public void SetUp() =>
            _queue = new RequestQueue(TestLogger.Create(), new NetworkSettings(10, string.Empty, logRequests: false));

        [TearDown]
        public void TearDown() => _queue.Dispose();

        [Test]
        public void Enqueue_WhenIdle_StartsRequestImmediately()
        {
            var request = new FakeRequest<string>();

            _queue.Enqueue(request);

            Assert.That(request.IsStarted, Is.True);
            Assert.That(_queue.IsBusy, Is.True);
            Assert.That(_queue.PendingCount, Is.Zero);
        }

        [Test]
        public void Enqueue_WhenBusy_StartsNextRequestOnlyAfterPreviousCompletes()
        {
            var first = new FakeRequest<string>();
            var second = new FakeRequest<string>();

            _queue.Enqueue(first);
            _queue.Enqueue(second);

            Assert.That(second.IsStarted, Is.False);
            Assert.That(_queue.PendingCount, Is.EqualTo(1));

            first.Complete("first");

            Assert.That(second.IsStarted, Is.True);
            Assert.That(_queue.PendingCount, Is.Zero);
        }

        [Test]
        public void Requests_AreExecutedInOrderOfEnqueueing()
        {
            var first = new FakeRequest<int>();
            var second = new FakeRequest<int>();
            var third = new FakeRequest<int>();
            _queue.Enqueue(first);
            _queue.Enqueue(second);
            _queue.Enqueue(third);

            first.Complete(1);
            Assert.That(second.IsStarted && !third.IsStarted, Is.True);

            second.Complete(2);
            Assert.That(third.IsStarted, Is.True);
        }

        [Test]
        public void Enqueue_ReturnsResultOfExecutedRequest()
        {
            var request = new FakeRequest<string>();

            var task = _queue.Enqueue(request);
            request.Complete("result");

            Assert.That(task.Status, Is.EqualTo(UniTaskStatus.Succeeded));
            Assert.That(task.GetAwaiter().GetResult(), Is.EqualTo("result"));
            Assert.That(_queue.IsBusy, Is.False);
        }

        [Test]
        public void CancellingWaitingRequest_RemovesItFromQueue_AndItNeverStarts()
        {
            var running = new FakeRequest<string>();
            var waiting = new FakeRequest<string>();
            var cancellation = new CancellationTokenSource();
            _queue.Enqueue(running);
            var waitingTask = _queue.Enqueue(waiting, cancellation.Token);

            cancellation.Cancel();

            Assert.That(waitingTask.Status, Is.EqualTo(UniTaskStatus.Canceled));
            Assert.That(_queue.PendingCount, Is.Zero);

            running.Complete("done");

            Assert.That(waiting.IsStarted, Is.False);
            Assert.That(_queue.IsBusy, Is.False);
        }

        [Test]
        public void CancellingRunningRequest_AbortsIt_AndStartsNextRequest()
        {
            var running = new FakeRequest<string>();
            var next = new FakeRequest<string>();
            var cancellation = new CancellationTokenSource();
            var runningTask = _queue.Enqueue(running, cancellation.Token);
            _queue.Enqueue(next);

            cancellation.Cancel();

            Assert.That(running.ReceivedToken.IsCancellationRequested, Is.True, "The running request must be aborted.");
            Assert.That(runningTask.Status, Is.EqualTo(UniTaskStatus.Canceled));
            Assert.That(next.IsStarted, Is.True);
        }

        [Test]
        public void FailedRequest_PassesExceptionToCaller_AndQueueKeepsWorking()
        {
            var failing = new FakeRequest<string>();
            var next = new FakeRequest<string>();
            var failingTask = _queue.Enqueue(failing);
            var nextTask = _queue.Enqueue(next);

            failing.Fail(new InvalidOperationException("boom"));
            next.Complete("ok");

            Assert.That(failingTask.Status, Is.EqualTo(UniTaskStatus.Faulted));
            Assert.Throws<InvalidOperationException>(() => failingTask.GetAwaiter().GetResult());
            Assert.That(nextTask.GetAwaiter().GetResult(), Is.EqualTo("ok"));
        }

        [Test]
        public void Enqueue_WithAlreadyCancelledToken_DoesNotQueueRequest()
        {
            var request = new FakeRequest<string>();
            var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            var task = _queue.Enqueue(request, cancellation.Token);

            Assert.That(task.Status, Is.EqualTo(UniTaskStatus.Canceled));
            Assert.That(request.IsStarted, Is.False);
            Assert.That(_queue.IsBusy, Is.False);
        }

        [Test]
        public void CancellingCompletedRequest_DoesNotAffectQueue()
        {
            var first = new FakeRequest<string>();
            var second = new FakeRequest<string>();
            var cancellation = new CancellationTokenSource();
            var firstTask = _queue.Enqueue(first, cancellation.Token);
            first.Complete("first");
            _queue.Enqueue(second);

            cancellation.Cancel();

            Assert.That(firstTask.Status, Is.EqualTo(UniTaskStatus.Succeeded));
            Assert.That(second.ReceivedToken.IsCancellationRequested, Is.False);
            Assert.That(_queue.IsBusy, Is.True);
        }

        [Test]
        public void Dispose_CancelsRunningAndWaitingRequests()
        {
            var running = new FakeRequest<string>();
            var waiting = new FakeRequest<string>();
            var runningTask = _queue.Enqueue(running);
            var waitingTask = _queue.Enqueue(waiting);

            _queue.Dispose();

            Assert.That(runningTask.Status, Is.EqualTo(UniTaskStatus.Canceled));
            Assert.That(waitingTask.Status, Is.EqualTo(UniTaskStatus.Canceled));
            Assert.That(waiting.IsStarted, Is.False);
            Assert.Throws<ObjectDisposedException>(() => _queue.Enqueue(new FakeRequest<string>()));
        }

        [Test]
        public void RequestEnqueuedWhenPreviousCompletes_IsStarted()
        {
            var first = new FakeRequest<string>();
            var second = new FakeRequest<string>();

            async UniTaskVoid EnqueueChainAsync()
            {
                await _queue.Enqueue(first);
                _queue.Enqueue(second).Forget();
            }

            EnqueueChainAsync().Forget();
            first.Complete("first");

            Assert.That(second.IsStarted, Is.True);
        }
    }
}
