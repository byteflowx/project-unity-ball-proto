using System;
using App.Scripts.Engine.Time;
using App.Scripts.UI.Toolkit.Components;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Services.Loader
{
    public sealed class LoaderService : ILoaderService, IDisposable
    {
        private readonly IUnityTimeService _timeService;

        private LoaderView _loaderView;

        public LoaderService(IUnityTimeService timeService)
        {
            _timeService = timeService;
        }

        public void Initialize(VisualElement loaderContainer) => _loaderView = new LoaderView(loaderContainer, _timeService);

        public void Show() => _loaderView?.Show();

        public void Hide() => _loaderView?.Hide();

        public void Dispose()
        {
            _loaderView?.Dispose();
            _loaderView = null;
        }
    }
}
