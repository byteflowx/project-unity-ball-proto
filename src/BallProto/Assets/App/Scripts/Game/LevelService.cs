using System.Threading;
using App.Scripts.Core.Constants;
using App.Scripts.Engine.Addressable;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Scripts.Game
{
    public sealed class LevelService : ILevelService
    {
        private readonly IAddressableService _addressables;
        private readonly IObjectResolver _resolver;

        public LevelService(IAddressableService addressables, IObjectResolver resolver)
        {
            _addressables = addressables;
            _resolver = resolver;
        }

        public async UniTask LoadAsync(CancellationToken ct = default)
        {
            GameObject prefab = await _addressables.Load<GameObject>(AddressableConstants.LEVEL, ct);
            _resolver.InjectGameObject(Object.Instantiate(prefab));
        }
    }
}
