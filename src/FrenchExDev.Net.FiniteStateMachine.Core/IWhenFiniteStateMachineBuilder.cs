namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;

/// <summary>
/// Defines a builder for configuring and constructing a finite state machine with specific actions  to be executed when
/// a particular state is entered.
/// </summary>
/// <typeparam name="TClass">The type of the class that represents the context or data associated with the state machine. Must be a non-nullable
/// type with a parameterless constructor.</typeparam>
/// <typeparam name="TState">The type representing the states of the finite state machine. Must be a non-nullable type.</typeparam>
/// <typeparam name="TTrigger">The type representing the triggers that cause state transitions in the finite state machine. Must be a non-nullable
/// type.</typeparam>
public interface IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger> : IFiniteStateMachineBuilder<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Configures an action to be executed when the state machine enters the specified state.
    /// </summary>
    /// <param name="state">The state for which the action should be executed.</param>
    /// <param name="action">The action to execute when the state machine enters the specified state.  The action receives the instance of
    /// the class, the trigger that caused the transition,  and the state machine itself as parameters.</param>
    /// <returns>An instance of <see cref="IWhenFiniteStateMachineBuilder{TClass, TState, TTrigger}"/>  to allow further
    /// configuration of the state machine.</returns>
    IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger> When(TState state, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>> action);

    /// <summary>
    /// Builds and returns a finite state machine configured with the specified instance and initial state.
    /// </summary>
    /// <remarks>This method initializes a finite state machine for the provided instance and sets its
    /// starting state. The returned state machine can then be used to define transitions and handle triggers.</remarks>
    /// <param name="instance">The instance of type <typeparamref name="TClass"/> that the state machine will operate on.</param>
    /// <param name="initialState">The initial state of the state machine, represented by a value of type <typeparamref name="TState"/>.</param>
    /// <returns>An <see cref="IWhenFiniteStateMachine{TClass, TState, TTrigger}"/> instance configured with the specified
    /// instance and initial state.</returns>
    IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TClass instance, TState initialState);

    /// <summary>
    /// Builds and returns a finite state machine configured with the specified initial state.
    /// </summary>
    /// <param name="initialState">The initial state of the state machine, represented by a value of type <typeparamref name="TState"/>.</param>
    /// <returns>An <see cref="IWhenFiniteStateMachine{TClass, TState, TTrigger}"/> instance configured with the specified
    /// initial state.</returns>
    IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TState initialState);
}
