using System;

namespace App.Infrastructure.Network
{
    public sealed class NetworkSettings
    {
        public NetworkSettings(int timeoutSeconds, string userAgent, bool logRequests)
        {
            if (timeoutSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), timeoutSeconds, "Must be positive.");

            TimeoutSeconds = timeoutSeconds;
            UserAgent = userAgent ?? string.Empty;
            LogRequests = logRequests;
        }

        public int TimeoutSeconds { get; }

        public string UserAgent { get; }

        public bool LogRequests { get; }
    }
}
