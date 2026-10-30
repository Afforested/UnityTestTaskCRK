using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Application.Images
{
    public interface IImageGateway
    {
        UniTask<byte[]> DownloadAsync(string url, CancellationToken cancellationToken);
    }
}
