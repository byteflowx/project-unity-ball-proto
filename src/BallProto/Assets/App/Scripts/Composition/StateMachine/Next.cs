using System;

namespace App.Scripts.Composition.StateMachine
{
    public readonly struct Next
    {
        public static readonly Next None = default;

        public readonly Type StateType;

        private Next(Type stateType) => StateType = stateType;

        public static Next To<TState>() where TState : IState => new(typeof(TState));
    }
}
