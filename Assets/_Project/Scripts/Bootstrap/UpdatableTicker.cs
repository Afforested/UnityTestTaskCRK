using System;
using System.Collections.Generic;
using App.Application.Common;
using UnityEngine;
using Zenject;

namespace App.Bootstrap
{
    public sealed class UpdatableTicker : ITickable
    {
        private readonly List<IUpdatable> _updatables;

        public UpdatableTicker(List<IUpdatable> updatables)
        {
            _updatables = updatables ?? throw new ArgumentNullException(nameof(updatables));
        }

        public void Tick()
        {
            var deltaTime = Time.deltaTime;
            for (var i = 0; i < _updatables.Count; i++)
                _updatables[i].Tick(deltaTime);
        }
    }
}
