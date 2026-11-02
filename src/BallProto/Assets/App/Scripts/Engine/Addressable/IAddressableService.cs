using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace App.Scripts.Engine.Addressable
{
    public interface IAddressableService
    {
        UniTask<T> Load<T>(string address, CancellationToken ct = default) where T : class;
        UniTask<TAsset> Load<TAsset>(AssetReference assetReference, CancellationToken ct = default) where TAsset : class;
    }
}
