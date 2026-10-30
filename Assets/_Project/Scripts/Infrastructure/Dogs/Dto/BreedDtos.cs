using System.Collections.Generic;
using Newtonsoft.Json;

namespace App.Infrastructure.Dogs.Dto
{

    internal sealed class BreedListResponseDto
    {
        [JsonProperty("data")]
        public List<BreedDataDto> Data { get; set; }
    }

    internal sealed class BreedResponseDto
    {
        [JsonProperty("data")]
        public BreedDataDto Data { get; set; }
    }

    internal sealed class BreedDataDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attributes")]
        public BreedAttributesDto Attributes { get; set; }
    }

    internal sealed class BreedAttributesDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
