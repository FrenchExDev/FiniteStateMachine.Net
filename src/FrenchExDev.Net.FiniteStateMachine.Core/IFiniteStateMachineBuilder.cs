namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;

public interface IFiniteStateMachineBuilder<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    IFiniteStateMachineBuilder<TClass, TState, TTrigger> State(TState state);
    IFiniteStateMachineBuilder<TClass, TState, TTrigger> CanTransition(TState fromState, TState toState, TTrigger on, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null, Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null);

    IFiniteStateMachine<TClass, TState, TTrigger> Build(TClass instance, TState initialState);
    IFiniteStateMachine<TClass, TState, TTrigger> Build(TState initialState);
}
