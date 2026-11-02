using System;
using System.Collections.Generic;
using System.Threading;
using App.Scripts.Core.Common;
using App.Scripts.Core.Constants;
using App.Scripts.Engine.Addressable;
using App.Scripts.UI.Constants;
using App.Scripts.UI.Factory;
using App.Scripts.UI.StaticData;
using App.Scripts.UI.Toolkit.Base;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Services.UIService
{
    public sealed class UIService : IUIService, IDisposable
    {
        private readonly PanelRenderer _panelRenderer;
        private readonly IAddressableService _addressableService;
        private readonly IUIViewFactory _uiViewFactory;
        private readonly Dictionary<Type, UIScreen> _screens = new();
        private readonly ReactiveProperty<UIScreen> _currentScreen = new(null);

        private UIAssetsConfig _assetsConfig;
        private VisualElement _screensContainer;
        private bool _isShowingScreen;

        public VisualElement Root { get; private set; }
        public ReadOnlyReactiveProperty<UIScreen> CurrentScreen => _currentScreen;

        public UIService(PanelRenderer panelRenderer, IAddressableService addressableService, IUIViewFactory uiViewFactory)
        {
            _panelRenderer = panelRenderer;
            _addressableService = addressableService;
            _uiViewFactory = uiViewFactory;
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            _assetsConfig = await _addressableService.Load<UIAssetsConfig>(AddressableConstants.UI_ASSETS_CONFIG, ct);

            VisualTreeAsset mainLayoutAsset = await _addressableService.Load<VisualTreeAsset>(_assetsConfig.MainLayout, ct);

            Root = await GetRootAsync(ct);
            mainLayoutAsset.CloneTree(Root);

            _screensContainer = Root.Q<VisualElement>(UxmlContainerNames.SCREENS_CONTENT);
        }

        public async UniTask ShowScreenAsync<TScreen>(CancellationToken ct = default) where TScreen : UIScreen
        {
            if (_isShowingScreen)
            {
                Log.Warning($"ShowScreenAsync<{typeof(TScreen).Name}> ignored — another screen transition is in progress");
                return;
            }

            _isShowingScreen = true;

            try
            {
                Type screenType = typeof(TScreen);

                if (!_screens.TryGetValue(screenType, out UIScreen screen))
                {
                    screen = await LoadScreenAsync<TScreen>(ct);
                    _screens[screenType] = screen;
                }

                if (_currentScreen.CurrentValue == screen)
                    return;

                CloseCurrentScreen();

                screen.Show();
                _currentScreen.Value = screen;
            }
            finally
            {
                _isShowingScreen = false;
            }
        }

        public void CloseCurrentScreen()
        {
            _currentScreen.CurrentValue?.HideImmediately();
            _currentScreen.Value = null;
        }

        public void Dispose()
        {
            foreach (UIScreen screen in _screens.Values)
                screen.Dispose();

            _screens.Clear();
            _currentScreen.Dispose();
        }

        private UniTask<VisualElement> GetRootAsync(CancellationToken ct)
        {
            UniTaskCompletionSource<VisualElement> tcs = new();

            void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
            {
                renderer.UnregisterUIReloadCallback(OnUIReload);
                tcs.TrySetResult(root);
            }

            _panelRenderer.RegisterUIReloadCallback(OnUIReload);
            return tcs.Task.AttachExternalCancellation(ct);
        }

        private async UniTask<TScreen> LoadScreenAsync<TScreen>(CancellationToken ct) where TScreen : UIScreen
        {
            AssetReference assetReference = _assetsConfig.GetAsset<TScreen>();
            VisualTreeAsset visualTreeAsset = await _addressableService.Load<VisualTreeAsset>(assetReference, ct);

            visualTreeAsset.CloneTree(_screensContainer, out int firstIndex, out _);
            VisualElement screenRoot = _screensContainer[firstIndex];

            return _uiViewFactory.Create<TScreen>(screenRoot);
        }
    }
}
