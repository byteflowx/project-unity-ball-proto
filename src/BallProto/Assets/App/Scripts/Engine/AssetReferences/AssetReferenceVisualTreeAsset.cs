using System;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace App.Scripts.Engine.AssetReferences
{
    [Serializable]
    public sealed class AssetReferenceVisualTreeAsset : AssetReferenceT<VisualTreeAsset>
    {
        public AssetReferenceVisualTreeAsset(string guid) : base(guid)
        {
        }
    }
}
