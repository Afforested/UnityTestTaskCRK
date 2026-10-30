using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Application.Images
{
    public sealed class LoadImageUseCase : ILoadImageUseCase
    {
        private readonly IImageGateway _gateway;

        public LoadImageUseCase(IImageGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        public UniTask<byte[]> ExecuteAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image url is required.", nameof(url));

            return _gateway.DownloadAsync(url, cancellationToken);
        }
    }
}
