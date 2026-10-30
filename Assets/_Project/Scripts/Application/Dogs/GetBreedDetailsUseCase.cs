using System;
using System.Threading;
using App.Domain.Dogs;
using Cysharp.Threading.Tasks;

namespace App.Application.Dogs
{
    public sealed class GetBreedDetailsUseCase : IGetBreedDetailsUseCase
    {
        private readonly IDogBreedsGateway _gateway;

        public GetBreedDetailsUseCase(IDogBreedsGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        public UniTask<BreedDetails> ExecuteAsync(string breedId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(breedId))
                throw new ArgumentException("Breed id is required.", nameof(breedId));

            return _gateway.GetBreedDetailsAsync(breedId, cancellationToken);
        }
    }
}
