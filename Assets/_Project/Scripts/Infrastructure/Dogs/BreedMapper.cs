using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Domain.Dogs;
using App.Infrastructure.Dogs.Dto;

namespace App.Infrastructure.Dogs
{
    internal static class BreedMapper
    {
        public static IReadOnlyList<Breed> ToBreeds(BreedListResponseDto response)
        {
            var data = response?.Data;
            if (data == null)
                throw new InvalidDataException("The breeds response has no data.");

            return data
                .Where(item => !string.IsNullOrWhiteSpace(item?.Id))
                .Select(item => new Breed(item.Id, item.Attributes?.Name))
                .ToList();
        }

        public static BreedDetails ToDetails(BreedResponseDto response)
        {
            var data = response?.Data;
            if (data == null || string.IsNullOrWhiteSpace(data.Id))
                throw new InvalidDataException("The breed response has no data.");

            return new BreedDetails(data.Id, data.Attributes?.Name, data.Attributes?.Description);
        }
    }
}
