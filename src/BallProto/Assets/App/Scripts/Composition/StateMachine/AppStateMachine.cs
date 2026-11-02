using System;
using System.Collections.Generic;
using System.Threading;
using App.Scripts.Core.Common;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Composition.StateMachine
{
    public sealed class AppStateMachine : IDisposable
    {
        private readonly Dictionary<Type, IState> _states;
        private readonly CancellationTokenSource _lifetimeCts = new();

        private IState _currentState;
        private Type _pendingState;
        private bool _isRunning;

        public AppStateMachine(IReadOnlyList<IState> states)
        {
            _states = new Dictionary<Type, IState>(states.Count);

            foreach (IState state in states)
                _states[state.GetType()] = state;
        }

        public async UniTask SwitchStateAsync<TState>(CancellationToken ct = default) where TState : IState
        {
            ct.ThrowIfCancellationRequested();

            if (_isRunning)
            {
                _pendingState = typeof(TState);
                Log.Warning($"Switch to {typeof(TState).Name} deferred — it runs after the transition in progress");
                return;
            }

            _isRunning = true;

            try
            {
                Type next = typeof(TState);

                while (next != null)
                {
                    if (!_states.TryGetValue(next, out IState state))
                        throw new InvalidOperationException($"State {next.Name} is not registered");

                    _currentState = state;

                    Log.Info($"Enter {state.GetType().Name}");

                    next = state is IEnterableAsyncState enterable
                        ? (await enterable.EnterAsync(_lifetimeCts.Token)).StateType
                        : null;

                    next ??= TakePendingState();
                }
            }
            finally
            {
                _isRunning = false;
                _pendingState = null;
            }
        }

        private Type TakePendingState()
        {
            Type queued = _pendingState;
            _pendingState = null;

            if (queued == _currentState?.GetType())
                return null;

            return queued;
        }

        public void Dispose()
        {
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }
    }
}
