namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

public class WhenFiniteStateMachineBuilder<TClass, TState, TTrigger> : FiniteStateMachineBuilder<TClass, TState, TTrigger>, IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    private Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> _when = new();

    public IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TClass instance, TState initialState)
    {
        return WhenFiniteStateMachine<TClass, TState, TTrigger>.Create(instance, initialState, _validStates, _transitions, _when);
    }

    public IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TState initialState)
    {
        return WhenFiniteStateMachine<TClass, TState, TTrigger>.Create(initialState, _validStates, _transitions, _when);
    }

    public IFiniteStateMachineBuilder<TClass, TState, TTrigger> CanTransition(TState fromState, TState toState, TTrigger on, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null, Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null)
    {
        base.CanTransition(on, fromState, toState, body, condition);
        return this;
    }

    public IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger> When(TState state, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>> action)
    {
        if (!_when.ContainsKey(state))
        {
            _when[state] = new();
        }

        _when[state].Add(action);

        return this;
    }

    IFiniteStateMachine<TClass, TState, TTrigger> IFiniteStateMachineBuilder<TClass, TState, TTrigger>.Build(TClass instance, TState initialState)
    {
        return Build(instance, initialState);
    }

    IFiniteStateMachine<TClass, TState, TTrigger> IFiniteStateMachineBuilder<TClass, TState, TTrigger>.Build(TState initialState)
    {
        return Build(initialState);
    }

    IFiniteStateMachineBuilder<TClass, TState, TTrigger> IFiniteStateMachineBuilder<TClass, TState, TTrigger>.State(TState state)
    {
        base.State(state);
        return this;
    }
}
