using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Application.Images
{
    public interface ILoadImageUseCase
    {
        UniTask<byte[]> ExecuteAsync(string url, CancellationToken cancellationToken);
    }
}
