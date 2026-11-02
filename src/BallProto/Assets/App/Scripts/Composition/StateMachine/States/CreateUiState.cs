using System.Threading;
using App.Scripts.UI.Constants;
using App.Scripts.UI.Services.Loader;
using App.Scripts.UI.Services.UIService;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace App.Scripts.Composition.StateMachine.States
{
    public sealed class CreateUiState : IState, IEnterableAsyncState
    {
        private readonly IUIService _uiService;
        private readonly ILoaderService _loaderService;

        public CreateUiState(IUIService uiService, ILoaderService loaderService)
        {
            _uiService = uiService;
            _loaderService = loaderService;
        }

        public async UniTask<Next> EnterAsync(CancellationToken ct = default)
        {
            await _uiService.InitializeAsync(ct);

            _loaderService.Initialize(_uiService.Root.Q<VisualElement>(UxmlContainerNames.LOADER));
            _loaderService.Show();

            return Next.To<GameState>();
        }
    }
}
