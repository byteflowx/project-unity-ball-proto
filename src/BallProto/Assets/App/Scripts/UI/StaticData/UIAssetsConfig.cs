using System.Collections.Generic;
using App.Scripts.Engine.AssetReferences;
using App.Scripts.UI.Toolkit.Base;
using UnityEngine;

namespace App.Scripts.UI.StaticData
{
    [CreateAssetMenu(fileName = "UIAssetsConfig", menuName = "App/UI Assets Config")]
    public sealed class UIAssetsConfig : ScriptableObject
    {
        [Header("Main Layout")]
        [SerializeField] private AssetReferenceVisualTreeAsset _mainLayout;

        [Header("Views by presenter class name")]
        [SerializeField] private Dictionary<string, AssetReferenceVisualTreeAsset> _views;

        public AssetReferenceVisualTreeAsset MainLayout => _mainLayout;

        public AssetReferenceVisualTreeAsset GetAsset<T>() where T : UIView
        {
            if (!_views.TryGetValue(typeof(T).Name, out AssetReferenceVisualTreeAsset asset))
                throw new KeyNotFoundException($"'{typeof(T).Name}' has no entry in UIAssetsConfig");

            return asset;
        }
    }
}
