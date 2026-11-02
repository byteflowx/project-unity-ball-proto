using System.Threading;
using App.Scripts.Engine.Audio;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Composition.StateMachine.States
{
    public sealed class LoadAudioState : IState, IEnterableAsyncState
    {
        private readonly IAudioService _audio;

        public LoadAudioState(IAudioService audio)
        {
            _audio = audio;
        }

        public async UniTask<Next> EnterAsync(CancellationToken ct = default)
        {
            await _audio.InitializeAsync(ct);

            return Next.To<CreateUiState>();
        }
    }
}
