using System.Threading;
using App.Scripts.Core.Constants;
using App.Scripts.Engine.Audio;
using App.Scripts.Engine.SceneLoader;
using App.Scripts.Game;
using App.Scripts.UI.Services.Loader;
using App.Scripts.UI.Services.UIService;
using App.Scripts.UI.Toolkit.Screens;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Composition.StateMachine.States
{
    public sealed class GameState : IState, IEnterableAsyncState
    {
        private readonly ISceneLoaderService _sceneLoader;
        private readonly ILevelService _levelService;
        private readonly IUIService _uiService;
        private readonly ILoaderService _loaderService;
        private readonly IAudioService _audio;

        public GameState(ISceneLoaderService sceneLoader, ILevelService levelService, IUIService uiService, ILoaderService loaderService, IAudioService audio)
        {
            _sceneLoader = sceneLoader;
            _levelService = levelService;
            _uiService = uiService;
            _loaderService = loaderService;
            _audio = audio;
        }

        public async UniTask<Next> EnterAsync(CancellationToken ct = default)
        {
            _uiService.CloseCurrentScreen();

            await _sceneLoader.LoadAsync(SceneName.GAMEPLAY_SCENE_NAME, ct);
            await _levelService.LoadAsync(ct);
            await _uiService.ShowScreenAsync<GameScreen>(ct);

            _loaderService.Hide();

            await _audio.PlayMusicAsync(AddressableConstants.THEME_MUSIC, ct);

            return Next.None;
        }
    }
}
