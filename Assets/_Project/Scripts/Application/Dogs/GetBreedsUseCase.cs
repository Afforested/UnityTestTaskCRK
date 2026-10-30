using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Domain.Dogs;
using Cysharp.Threading.Tasks;

namespace App.Application.Dogs
{
    public sealed class GetBreedsUseCase : IGetBreedsUseCase
    {
        private readonly IDogBreedsGateway _gateway;
        private readonly DogsSettings _settings;

        public GetBreedsUseCase(IDogBreedsGateway gateway, DogsSettings settings)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async UniTask<IReadOnlyList<Breed>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var breeds = await _gateway.GetBreedsAsync(_settings.BreedsCount, cancellationToken);
            return breeds.Count <= _settings.BreedsCount
                ? breeds
                : breeds.Take(_settings.BreedsCount).ToList();
        }
    }
}
