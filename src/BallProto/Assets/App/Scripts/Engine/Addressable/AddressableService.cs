using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.Scripts.Engine.Addressable
{
    public sealed class AddressableService : IAddressableService, IDisposable
    {
        private readonly Dictionary<(string Address, Type Type), AsyncOperationHandle> _cache = new();

        public async UniTask<T> Load<T>(string address, CancellationToken ct = default) where T : class
        {
            return await LoadInternal<T>(address, ct);
        }

        public async UniTask<TAsset> Load<TAsset>(AssetReference assetReference, CancellationToken ct = default) where TAsset : class
        {
            return await LoadInternal<TAsset>(assetReference.AssetGUID, ct);
        }

        public void Dispose()
        {
            foreach (AsyncOperationHandle handle in _cache.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _cache.Clear();
        }

        private async UniTask<T> LoadInternal<T>(string address, CancellationToken ct) where T : class
        {
            (string, Type) key = (address, typeof(T));

            if (_cache.TryGetValue(key, out AsyncOperationHandle cached))
                return await cached.Convert<T>().ToUniTask(cancellationToken: ct);

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            _cache[key] = handle;

            try
            {
                return await handle.ToUniTask(cancellationToken: ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                _cache.Remove(key);
                Addressables.Release(handle);
                throw;
            }
        }
    }
}
