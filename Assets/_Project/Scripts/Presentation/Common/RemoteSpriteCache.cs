using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using App.Application.Images;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Presentation.Common
{
    public sealed class RemoteSpriteCache : IDisposable
    {
        private readonly ILoadImageUseCase _loadImage;
        private readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        public RemoteSpriteCache(ILoadImageUseCase loadImage)
        {
            _loadImage = loadImage ?? throw new ArgumentNullException(nameof(loadImage));
        }

        public async UniTask<Sprite> GetAsync(string url, CancellationToken cancellationToken)
        {
            if (_sprites.TryGetValue(url, out var cached))
                return cached;

            var bytes = await _loadImage.ExecuteAsync(url, cancellationToken);

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            if (!texture.LoadImage(bytes, markNonReadable: true))
            {
                Object.Destroy(texture);
                throw new InvalidDataException($"Image from {url} could not be decoded.");
            }

            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _sprites[url] = sprite;
            return sprite;
        }

        public void Dispose()
        {
            foreach (var sprite in _sprites.Values)
            {
                if (sprite == null)
                    continue;

                Object.Destroy(sprite.texture);
                Object.Destroy(sprite);
            }

            _sprites.Clear();
        }
    }
}
