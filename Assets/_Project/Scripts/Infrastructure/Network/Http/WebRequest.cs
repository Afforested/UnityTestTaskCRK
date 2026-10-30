using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace App.Infrastructure.Network.Http
{
    public abstract class WebRequest<TResult> : IRequest<TResult>
    {
        private readonly NetworkSettings _settings;

        protected WebRequest(string url, NetworkSettings settings, string name)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url is required.", nameof(url));

            Url = url;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Name = string.IsNullOrEmpty(name) ? "GET " + url : name;
        }

        public string Name { get; }

        public string Url { get; }

        public async UniTask<TResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var request = UnityWebRequest.Get(Url))
            {
                request.timeout = _settings.TimeoutSeconds;
#if !UNITY_WEBGL
                if (_settings.UserAgent.Length > 0)
                    request.SetRequestHeader("User-Agent", _settings.UserAgent);
#endif

                try
                {
                    
                    await request.SendWebRequest().WithCancellation(cancellationToken);
                }
                catch (UnityWebRequestException exception)
                {
                    throw new WebRequestException(Url, exception.ResponseCode, exception.Error, exception);
                }

                return ParseResponse(request.downloadHandler);
            }
        }

        protected abstract TResult ParseResponse(DownloadHandler downloadHandler);
    }
}
