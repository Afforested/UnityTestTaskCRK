using App.Domain.Common;
using NUnit.Framework;

namespace App.Tests.Domain
{
    public sealed class PeriodicTimerTests
    {
        [Test]
        public void Advance_BeforeInterval_CompletesNothing()
        {
            var timer = new PeriodicTimer(3f);

            Assert.That(timer.Advance(2.9f), Is.Zero);
            Assert.That(timer.Remaining, Is.EqualTo(0.1f).Within(1e-4f));
        }

        [Test]
        public void Advance_ReachingInterval_CompletesOnePeriod_AndKeepsRemainder()
        {
            var timer = new PeriodicTimer(3f);

            Assert.That(timer.Advance(3.5f), Is.EqualTo(1));
            Assert.That(timer.Progress, Is.EqualTo(0.5f / 3f).Within(1e-4f));
        }

        [Test]
        public void Advance_OverSeveralIntervals_CompletesEachOfThem()
        {
            var timer = new PeriodicTimer(3f);

            Assert.That(timer.Advance(9.2f), Is.EqualTo(3));
        }

        [Test]
        public void Advance_InSmallSteps_AccumulatesTime()
        {
            var timer = new PeriodicTimer(1f);
            var completed = 0;

            for (var i = 0; i < 100; i++)
                completed += timer.Advance(0.05f);

            Assert.That(completed, Is.InRange(4, 5));
        }
    }
}
