namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a finite state machine that associates states with a collection of actions to be executed when specific
/// triggers occur.
/// </summary>
/// <typeparam name="TClass">The type of the class that the state machine operates on. Must be a non-nullable reference type with a parameterless
/// constructor.</typeparam>
/// <typeparam name="TState">The type representing the states of the finite state machine. Must be non-nullable.</typeparam>
/// <typeparam name="TTrigger">The type representing the triggers that cause state transitions. Must be non-nullable.</typeparam>
public interface IWhenFiniteStateMachine<TClass, TState, TTrigger> : IFiniteStateMachine<TClass, TState, TTrigger>
    /// Using the generic type constraints to ensure TClass is a non-nullable reference type with a parameterless constructor,
    where TClass : notnull, new()
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Gets a read-only dictionary that maps states to a list of actions to be executed  when the state is active and a
    /// specific trigger is fired.
    /// </summary>
    /// <remarks>The actions in the list are executed in the order they appear when the associated state is
    /// active  and the corresponding trigger is fired. This property is typically used to inspect or configure 
    /// state-specific behavior in the finite state machine.</remarks>
    IReadOnlyDictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> WhenStates { get; }


    /// <summary>
    /// Gets a read-only dictionary that maps triggers to a list of actions to be executed when the corresponding
    /// trigger is activated.
    /// </summary>
    /// <remarks>This property provides a way to inspect the configured triggers and their associated actions 
    /// in the finite state machine. Modifications to the returned dictionary are not allowed, ensuring  the integrity
    /// of the trigger-action mappings.</remarks>
    IReadOnlyDictionary<TTrigger, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> WhenTriggers { get; }
}
