using System;
using System.Threading;
using App.Application.Images;
using App.Infrastructure.Network;
using App.Infrastructure.Network.Http;
using Cysharp.Threading.Tasks;

namespace App.Infrastructure.Images
{
    public sealed class ImageGateway : IImageGateway
    {
        private readonly IRequestQueue _queue;
        private readonly NetworkSettings _networkSettings;

        public ImageGateway(IRequestQueue queue, NetworkSettings networkSettings)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _networkSettings = networkSettings ?? throw new ArgumentNullException(nameof(networkSettings));
        }

        public UniTask<byte[]> DownloadAsync(string url, CancellationToken cancellationToken)
        {
            var request = new BytesWebRequest(url, _networkSettings, "Image " + url);
            return _queue.Enqueue(request, cancellationToken);
        }
    }
}
