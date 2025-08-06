namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;

public class Transition<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    public TState FromState { get; }
    public TState ToState { get; }
    public TTrigger Trigger { get; }
    public Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? Condition { get; }
    public Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? Body { get; }

    public Transition(TState fromState, TState toState, TTrigger trigger, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null, Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null)
    {
        FromState = fromState;
        ToState = toState;
        Trigger = trigger;
        Body = body;
        Condition = condition;
    }
}
