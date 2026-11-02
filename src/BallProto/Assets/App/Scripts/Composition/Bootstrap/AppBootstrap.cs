using System;
using System.Threading;
using App.Scripts.AppFlow;
using App.Scripts.Core.Common;
using App.Scripts.UI.Services.Loader;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace App.Scripts.Composition.Bootstrap
{
    public sealed class AppBootstrap : IAsyncStartable
    {
        private readonly IAppFlowService _appFlow;
        private readonly ILoaderService _loaderService;

        public AppBootstrap(IAppFlowService appFlow, ILoaderService loaderService)
        {
            _appFlow = appFlow;
            _loaderService = loaderService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            try
            {
                await _appFlow.BootAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Log.Exception(exception);
                _loaderService.Hide();
            }
        }
    }
}
