using System;

namespace App.Infrastructure.Dogs
{
    public sealed class DogApiSettings
    {
        public DogApiSettings(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base url is required.", nameof(baseUrl));

            BaseUrl = baseUrl.TrimEnd('/');
        }

        public string BaseUrl { get; }
    }
}
