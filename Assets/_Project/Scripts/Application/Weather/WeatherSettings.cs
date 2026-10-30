using System;

namespace App.Application.Weather
{
    public sealed class WeatherSettings
    {
        public WeatherSettings(float pollIntervalSeconds)
        {
            if (pollIntervalSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(pollIntervalSeconds), pollIntervalSeconds, "Must be positive.");

            PollIntervalSeconds = pollIntervalSeconds;
        }

        public float PollIntervalSeconds { get; }
    }
}
