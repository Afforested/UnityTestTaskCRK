using System;

namespace App.Domain.Dogs
{
    public sealed class Breed
    {
        public Breed(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Breed id is required.", nameof(id));

            Id = id;
            Name = name ?? string.Empty;
        }

        public string Id { get; }

        public string Name { get; }
    }
}
