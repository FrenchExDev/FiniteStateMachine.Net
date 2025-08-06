namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;

public interface IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger> : IFiniteStateMachineBuilder<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger> When(TState state, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>> action);
    IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TClass instance, TState initialState);
    IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TState initialState);
}
