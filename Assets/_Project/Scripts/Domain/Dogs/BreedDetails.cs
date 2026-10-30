using System;

namespace App.Domain.Dogs
{
    public sealed class BreedDetails
    {
        public BreedDetails(string id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Breed id is required.", nameof(id));

            Id = id;
            Name = name ?? string.Empty;
            Description = description ?? string.Empty;
        }

        public string Id { get; }

        public string Name { get; }

        public string Description { get; }
    }
}
