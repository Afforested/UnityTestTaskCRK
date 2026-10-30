using System;
using System.Collections.Generic;
using System.Threading;
using App.Application.Dogs;
using App.Domain.Dogs;
using App.Infrastructure.Dogs.Dto;
using App.Infrastructure.Network;
using App.Infrastructure.Network.Http;
using Cysharp.Threading.Tasks;

namespace App.Infrastructure.Dogs
{
    public sealed class DogBreedsGateway : IDogBreedsGateway
    {
        private readonly IRequestQueue _queue;
        private readonly DogApiSettings _apiSettings;
        private readonly NetworkSettings _networkSettings;

        public DogBreedsGateway(IRequestQueue queue, DogApiSettings apiSettings, NetworkSettings networkSettings)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
            _networkSettings = networkSettings ?? throw new ArgumentNullException(nameof(networkSettings));
        }

        public async UniTask<IReadOnlyList<Breed>> GetBreedsAsync(int count, CancellationToken cancellationToken)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Must be positive.");

            var url = $"{_apiSettings.BaseUrl}/breeds?page%5Bsize%5D={count}";
            var request = new JsonWebRequest<BreedListResponseDto>(url, _networkSettings, "Dog breeds");
            var response = await _queue.Enqueue(request, cancellationToken);
            return BreedMapper.ToBreeds(response);
        }

        public async UniTask<BreedDetails> GetBreedDetailsAsync(string breedId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(breedId))
                throw new ArgumentException("Breed id is required.", nameof(breedId));

            var url = $"{_apiSettings.BaseUrl}/breeds/{Uri.EscapeDataString(breedId)}";
            var request = new JsonWebRequest<BreedResponseDto>(url, _networkSettings, "Breed " + breedId);
            var response = await _queue.Enqueue(request, cancellationToken);
            return BreedMapper.ToDetails(response);
        }
    }
}
