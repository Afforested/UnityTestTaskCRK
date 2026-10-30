using System;

namespace App.Domain.Common
{
   
    public sealed class PeriodicTimer
    {
        private float _elapsed;

        public PeriodicTimer(float intervalSeconds)
        {
            if (intervalSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(intervalSeconds), intervalSeconds, "Interval must be positive.");

            Interval = intervalSeconds;
        }

        public float Interval { get; }

        public float Progress => _elapsed / Interval;

        public float Remaining => Interval - _elapsed;

        public int Advance(float deltaTime)
        {
            if (deltaTime < 0f)
                throw new ArgumentOutOfRangeException(nameof(deltaTime), deltaTime, "Delta time must not be negative.");

            _elapsed += deltaTime;
            if (_elapsed < Interval)
                return 0;

            var completed = (int)(_elapsed / Interval);
            _elapsed = Math.Max(0f, _elapsed - completed * Interval);
            return completed;
        }

        public void Reset() => _elapsed = 0f;
    }
}
