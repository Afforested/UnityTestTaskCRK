using System;

namespace App.Application.Dogs
{
    public sealed class DogsSettings
    {
        public DogsSettings(int breedsCount)
        {
            if (breedsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(breedsCount), breedsCount, "Must be positive.");

            BreedsCount = breedsCount;
        }

        public int BreedsCount { get; }
    }
}
