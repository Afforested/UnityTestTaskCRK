using System;

namespace App.Infrastructure.Network.Http
{
    public sealed class WebRequestException : Exception
    {
        public WebRequestException(string url, long statusCode, string error, Exception innerException = null)
            : base($"Request to {url} failed: {error} (HTTP {statusCode}).", innerException)
        {
            Url = url;
            StatusCode = statusCode;
        }

        public WebRequestException(string url, string reason, Exception innerException = null)
            : base($"Request to {url} failed: {reason}", innerException)
        {
            Url = url;
        }

        public string Url { get; }

        public long StatusCode { get; }
    }
}
