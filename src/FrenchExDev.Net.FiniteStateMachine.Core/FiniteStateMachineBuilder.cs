namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

public class FiniteStateMachineBuilder<TClass, TState, TTrigger>
where TClass : notnull, new()
where TState : notnull
where TTrigger : notnull
{
    protected readonly Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> _transitions = new();
    protected readonly List<TState> _validStates = new();

    public virtual FiniteStateMachineBuilder<TClass, TState, TTrigger> State(TState state)
    {
        if (!_validStates.Contains(state))
        {
            _validStates.Add(state);
        }

        if (!_transitions.ContainsKey(state))
        {
            _transitions[state] = new List<Transition<TClass, TState, TTrigger>>();
        }

        return this;
    }

    public FiniteStateMachineBuilder<TClass, TState, TTrigger> CanTransition(TTrigger on, TState fromState, TState toState, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null, Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null)
    {
        State(fromState);
        State(toState);
        _transitions[fromState].Add(new Transition<TClass, TState, TTrigger>(fromState, toState, on, body, condition));
        return this;
    }

    public virtual FiniteStateMachine<TClass, TState, TTrigger> Build(TClass instance, TState initialState)
    {
        return FiniteStateMachine<TClass, TState, TTrigger>.Create(instance, initialState, _validStates, _transitions);
    }

    public virtual FiniteStateMachine<TClass, TState, TTrigger> Build(TState initialState)
    {
        return FiniteStateMachine<TClass, TState, TTrigger>.Create(new TClass(), initialState, _validStates, _transitions);
    }
}
