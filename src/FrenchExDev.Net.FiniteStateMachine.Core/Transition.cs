namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;

/// <summary>
/// Represents a transition in a finite state machine.
/// </summary>
/// <typeparam name="TClass">The type of the class that represents the context or data associated with the state machine. Must be a non-nullable
/// type with a parameterless constructor.</typeparam>
/// <typeparam name="TState">The type representing the states of the finite state machine. Must be a non-nullable type.</typeparam>
/// <typeparam name="TTrigger">The type representing the triggers that cause state transitions in the finite state machine. Must be a non-nullable
/// type.</typeparam>
public class Transition<TClass, TState, TTrigger>
    /// Using the generic type constraints to ensure TClass is a non-nullable reference type with a parameterless constructor,
    where TClass : notnull, new()
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Holds the state from which the transition originates, the state to which it leads, and the trigger that causes the transition.
    /// </summary>
    public TState FromState { get; }

    /// <summary>
    /// Holds the state to which the transition leads.
    /// </summary>
    public TState ToState { get; }

    /// <summary>
    /// Holds the trigger that causes the transition to occur. This trigger must be activated for the transition to take place.
    /// </summary>
    public TTrigger Trigger { get; }

    /// <summary>
    /// Holds an optional condition that must evaluate to <see langword="true"/> for the transition to occur.
    /// </summary>
    public Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? Condition { get; }

    /// <summary>
    /// Holds an optional action to execute during the transition. The action receives the instance of the class, the trigger, and the finite state machine as parameters.
    /// </summary>
    public Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? Body { get; }

    /// <summary>
    /// Represents a transition between two states in a finite state machine, triggered by a specific event.
    /// </summary>
    /// <remarks>This constructor initializes a transition with the specified source state, destination state,
    /// trigger,  and optional behavior and condition. Transitions are a core component of finite state machines, 
    /// enabling dynamic state changes based on triggers and conditions.</remarks>
    /// <param name="fromState">The state from which the transition originates.</param>
    /// <param name="toState">The state to which the transition leads.</param>
    /// <param name="trigger">The trigger that causes the transition to occur.</param>
    /// <param name="body">An optional action to execute during the transition. The action receives the current instance of  <typeparamref
    /// name="TClass"/>, the trigger, and the finite state machine as parameters.</param>
    /// <param name="condition">An optional condition that must evaluate to <see langword="true"/> for the transition to occur.  The condition
    /// receives the current instance of <typeparamref name="TClass"/>, the trigger, and the finite state machine as
    /// parameters. If <see langword="null"/>, the transition is always allowed.</param>
    public Transition(TState fromState, TState toState, TTrigger trigger, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null, Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null)
    {
        FromState = fromState;
        ToState = toState;
        Trigger = trigger;
        Body = body;
        Condition = condition;
    }
}
