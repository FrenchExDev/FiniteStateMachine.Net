namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System.Collections.Generic;

/// <summary>
/// Represents a finite state machine that manages state transitions for a specified object.
/// </summary>
/// <typeparam name="TClass">The type of the object associated with the state machine. Must be non-nullable.</typeparam>
/// <typeparam name="TState">The type representing the states of the state machine. Must be non-nullable.</typeparam>
/// <typeparam name="TTrigger">The type representing the triggers that cause state transitions. Must be non-nullable.</typeparam>
public interface IFiniteStateMachine<TClass, TState, TTrigger>
    // Using the generic type constraints to ensure TClass is a non-nullable reference type,
    where TClass : notnull
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Executes the transition associated with the specified trigger.
    /// </summary>
    /// <param name="trigger">The trigger that initiates the transition.</param>
    /// <returns>A <see cref="TransitionResult"/> indicating the outcome of the transition, including whether it was successful
    /// and any associated state changes.</returns>
    TransitionResult Fire(TTrigger trigger);

    /// <summary>
    /// Gets the possible triggers that can be used to transition from the current state.
    /// </summary>
    /// <returns>A collection of possible triggers</returns>
    IEnumerable<TTrigger> GetPossibleTriggers();

    /// <summary>
    /// Gets the current state of the object.
    /// </summary>
    TState CurrentState { get; }

    /// <summary>
    /// Gets the instance of the object of type <typeparamref name="TClass"/>.
    /// </summary>
    TClass Object { get; }
}
