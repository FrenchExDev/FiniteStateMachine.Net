namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

public class WhenFiniteStateMachine<TClass, TState, TTrigger> : FiniteStateMachine<TClass, TState, TTrigger>, IWhenFiniteStateMachine<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    private readonly Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> _when;
    public IReadOnlyDictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> When => _when.AsReadOnly();

    public static WhenFiniteStateMachine<TClass, TState, TTrigger> Create(TClass initialObject, TState initialState, List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions, Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> when)
    {
        var fsm = new WhenFiniteStateMachine<TClass, TState, TTrigger>(initialObject, initialState, states, transitions, when);
        return fsm;
    }

    public static WhenFiniteStateMachine<TClass, TState, TTrigger> Create(TState initialState, List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions, Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> when)
    {
        var fsm = new WhenFiniteStateMachine<TClass, TState, TTrigger>(new(), initialState, states, transitions, when);
        return fsm;
    }

    protected WhenFiniteStateMachine(TClass instance, TState initialState, List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions, Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> when) : base(instance, initialState, states, transitions)
    {
        _when = when;
    }

    public new TransitionResult Fire(TTrigger trigger)
    {
        var result = base.Fire(trigger);
        if (result is TransitionResult.Success)
        {
            foreach (var action in _when.GetValueOrDefault(CurrentState, new List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>()))
            {
                action(Object, trigger, this);
            }
        }
        return result;
    }
}
