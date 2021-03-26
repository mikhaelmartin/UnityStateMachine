using System;
using System.Collections.Generic;

namespace StateMachine
{
    internal sealed class AnyState { }

    internal interface IState<T>
    {
        internal HashSet<Type> PreviousStateTypes { get; }
        internal void OnStateEnter(T owner);
        internal void OnStateExit(T owner);
        internal void OnStateUpdate(T owner, float deltaTime);
        internal void OnStateFixedUpdate(T owner, float fixedDeltaTime);
    }

    internal interface ICollision<T, TCollision>
    {
        internal void OnCollisionEnter(T owner, TCollision other);
        internal void OnCollisionStay(T owner, TCollision other);
        internal void OnCollisionExit(T owner, TCollision other);
    }

    internal interface ITrigger<T, TTrigger>
    {
        internal void OnTriggerEnter(T owner, TTrigger other);
        internal void OnTriggerStay(T owner, TTrigger other);
        internal void OnTriggerExit(T owner, TTrigger other);
    }

    internal class StateEventArgs<T> : EventArgs
    {
        internal T Owner { get; }
        internal IState<T> PreviousState { get; }
        internal IState<T> CurrentState { get; }
        public StateEventArgs(T owner, IState<T> previousState, IState<T> currentState)
        {
            Owner = owner;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }

    internal class StateMachine<T>
    {
        private T owner;
        private IState<T> previousState;
        private IState<T> currentState;

        internal event EventHandler<StateEventArgs<T>> StateChanged;
        protected virtual void OnStateChanged(StateEventArgs<T> e)
        {
            StateChanged?.Invoke(this, e);
        }

        internal IState<T> CurrentState
        {
            get => currentState;
        }

        internal IState<T> PreviousState
        {
            get => previousState;
        }

        internal StateMachine(T owner, IState<T> initialState)
        {
            this.owner = owner;
            this.currentState = initialState;
            this.currentState.OnStateEnter(owner);
        }

        internal void ChangeState(IState<T> newState)
        {
            if (this.currentState.GetType() != newState.GetType())
            {
                if (newState.PreviousStateTypes.Contains(typeof(AnyState)) ||
                   newState.PreviousStateTypes.Contains(this.currentState.GetType()))
                {
                    this.currentState.OnStateExit(owner);
                    this.previousState = this.currentState;
                    this.currentState = newState;
                    this.currentState.OnStateEnter(owner);

                    OnStateChanged(new StateEventArgs<T>(
                        owner, this.previousState, this.currentState));
                }
            }
        }

        internal void Update(float deltaTime)
        {
            this.currentState.OnStateUpdate(owner, deltaTime);
        }

        internal void FixedUpdate(float fixedDeltaTime)
        {
            this.currentState.OnStateFixedUpdate(owner, fixedDeltaTime);
        }
    }
}