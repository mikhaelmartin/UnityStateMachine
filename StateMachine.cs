using System;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    internal sealed class AnyState { }

    internal abstract class FiniteState<T>
    {
        internal abstract HashSet<Type> PreviousStateTypes { get; }
        internal virtual void OnStateEnter(T owner) { }
        internal virtual void OnStateExit(T owner) { }
        internal virtual void OnUpdate(T owner, float deltaTime) { }
        internal virtual void OnFixedUpdate(T owner, float fixedDeltaTime) { }
        internal virtual void OnCollisionEnter(T owner, Collision other) { }
        internal virtual void OnCollisionStay(T owner, Collision other) { }
        internal virtual void OnCollisionExit(T owner, Collision other) { }
        internal virtual void OnTriggerEnter(T owner, Collider other) { }
        internal virtual void OnTriggerStay(T owner, Collider other) { }
        internal virtual void OnTriggerExit(T owner, Collider other) { }
        internal virtual void OnCollisionEnter2D(T owner, Collision2D other) { }
        internal virtual void OnCollisionStay2D(T owner, Collision2D other) { }
        internal virtual void OnCollisionExit2D(T owner, Collision2D other) { }
        internal virtual void OnTriggerEnter2D(T owner, Collider2D other) { }
        internal virtual void OnTriggerStay2D(T owner, Collider2D other) { }
        internal virtual void OnTriggerExit2D(T owner, Collider2D other) { }
    }

    internal class StateEventArgs<T> : EventArgs
    {
        internal T Owner { get; }
        internal FiniteState<T> PreviousState { get; }
        internal FiniteState<T> CurrentState { get; }
        public StateEventArgs(T owner, FiniteState<T> previousState, FiniteState<T> currentState)
        {
            Owner = owner;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }

    internal class StateMachine<T>
    {
        private T owner;
        private FiniteState<T> previousState;
        private FiniteState<T> currentState;
        internal event EventHandler<StateEventArgs<T>> StateChanged;
        protected virtual void OnStateChanged(StateEventArgs<T> e) { StateChanged?.Invoke(this, e); }
        internal FiniteState<T> CurrentState { get => currentState; }
        internal FiniteState<T> PreviousState { get => previousState; }
        internal StateMachine(T owner, FiniteState<T> initialState)
        {
            this.owner = owner;
            this.currentState = initialState;
            this.currentState.OnStateEnter(owner);
        }

        internal void ChangeState(FiniteState<T> newState)
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

        internal void Update(float deltaTime) { currentState.OnUpdate(owner, deltaTime); }
        internal void FixedUpdate(float fixedDeltaTime) { currentState.OnFixedUpdate(owner, fixedDeltaTime); }
        internal void OnCollisionEnter(Collision other) { currentState.OnCollisionEnter(owner, other); }
        internal void OnCollisionStay(Collision other) { currentState.OnCollisionStay(owner, other); }
        internal void OnCollisionExit(Collision other) { currentState.OnCollisionExit(owner, other); }
        internal void OnTriggerEnter(Collider other) { currentState.OnTriggerEnter(owner, other); }
        internal void OnTriggerStay(Collider other) { currentState.OnTriggerStay(owner, other); }
        internal void OnTriggerExit(Collider other) { currentState.OnTriggerExit(owner, other); }
        internal void OnCollisionEnter2D(Collision2D other) { currentState.OnCollisionEnter2D(owner, other); }
        internal void OnCollisionStay2D(Collision2D other) { currentState.OnCollisionStay2D(owner, other); }
        internal void OnCollisionExit2D(Collision2D other) { currentState.OnCollisionExit2D(owner, other); }
        internal void OnTriggerEnter2D(Collider2D other) { currentState.OnTriggerEnter2D(owner, other); }
        internal void OnTriggerStay2D(Collider2D other) { currentState.OnTriggerStay2D(owner, other); }
        internal void OnTriggerExit2D(Collider2D other) { currentState.OnTriggerExit2D(owner, other); }
    }
}