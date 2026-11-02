using System.Threading;
using App.Scripts.AppFlow;
using App.Scripts.Composition.StateMachine;
using App.Scripts.Composition.StateMachine.States;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Composition.Flow
{
    public sealed class AppFlowService : IAppFlowService
    {
        private readonly AppStateMachine _stateMachine;
        private bool _booted;

        public AppFlowService(AppStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public async UniTask BootAsync(CancellationToken ct = default)
        {
            if (_booted)
                return;

            _booted = true;

            try
            {
                await _stateMachine.SwitchStateAsync<LoadAudioState>(ct);
            }
            catch
            {
                _booted = false;
                throw;
            }
        }

        public UniTask ToGameAsync(CancellationToken ct = default)
        {
            return _stateMachine.SwitchStateAsync<GameState>(ct);
        }
    }
}
