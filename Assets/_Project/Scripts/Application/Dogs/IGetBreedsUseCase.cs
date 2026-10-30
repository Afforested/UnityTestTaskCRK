using System.Collections.Generic;
using System.Threading;
using App.Domain.Dogs;
using Cysharp.Threading.Tasks;

namespace App.Application.Dogs
{
    public interface IGetBreedsUseCase
    {
        UniTask<IReadOnlyList<Breed>> ExecuteAsync(CancellationToken cancellationToken);
    }
}
