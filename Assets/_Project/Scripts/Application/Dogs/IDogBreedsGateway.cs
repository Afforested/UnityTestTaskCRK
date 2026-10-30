using System.Collections.Generic;
using System.Threading;
using App.Domain.Dogs;
using Cysharp.Threading.Tasks;

namespace App.Application.Dogs
{
    public interface IDogBreedsGateway
    {
        UniTask<IReadOnlyList<Breed>> GetBreedsAsync(int count, CancellationToken cancellationToken);

        UniTask<BreedDetails> GetBreedDetailsAsync(string breedId, CancellationToken cancellationToken);
    }
}
