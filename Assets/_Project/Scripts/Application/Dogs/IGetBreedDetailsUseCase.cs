using System.Threading;
using App.Domain.Dogs;
using Cysharp.Threading.Tasks;

namespace App.Application.Dogs
{
    public interface IGetBreedDetailsUseCase
    {
        UniTask<BreedDetails> ExecuteAsync(string breedId, CancellationToken cancellationToken);
    }
}
