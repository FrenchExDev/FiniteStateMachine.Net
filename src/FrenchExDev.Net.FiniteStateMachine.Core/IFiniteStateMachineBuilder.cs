namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;

/// <summary>
/// Declares the interface for building a finite state machine.
/// </summary>
/// <typeparam name="TClass">The object</typeparam>
/// <typeparam name="TState">State type</typeparam>
/// <typeparam name="TTrigger">Trigger type</typeparam>
public interface IFiniteStateMachineBuilder<TClass, TState, TTrigger>
    /// Using the generic type constraints to ensure TClass is a non-nullable reference type with a parameterless constructor,
    where TClass : notnull, new()
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Configures a state within the finite state machine.
    /// </summary>
    /// <remarks>Use this method to define and configure a specific state in the finite state machine.  After
    /// calling this method, additional configuration methods can be chained to specify behaviors, transitions, or other
    /// properties of the state.</remarks>
    /// <param name="state">The state to configure. This must be a valid state within the state machine.</param>
    /// <returns>An <see cref="IFiniteStateMachineBuilder{TClass, TState, TTrigger}"/> instance that allows further configuration
    /// of the state machine.</returns>
    IFiniteStateMachineBuilder<TClass, TState, TTrigger> State(TState state);

    /// <summary>
    /// Configures a state transition in the finite state machine.
    /// </summary>
    /// <param name="fromState">The state from which the transition originates.</param>
    /// <param name="toState">The state to which the transition leads.</param>
    /// <param name="on">The trigger that causes the transition to occur.</param>
    /// <param name="body">An optional action to execute during the transition. The action receives the instance of the class,  the
    /// trigger, and the finite state machine as parameters.</param>
    /// <param name="condition">An optional condition that must evaluate to <see langword="true"/> for the transition to occur.  The condition
    /// receives the instance of the class, the trigger, and the finite state machine as parameters.</param>
    /// <returns>The current instance of the finite state machine builder, allowing for further configuration.</returns>
    IFiniteStateMachineBuilder<TClass, TState, TTrigger> CanTransition(TState fromState, TState toState, TTrigger on, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null, Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null);

    /// <summary>
    /// Builds and returns a finite state machine configured with the specified instance and initial state.
    /// </summary>
    /// <param name="instance">The object instance that the finite state machine will operate on. This parameter cannot be null.</param>
    /// <param name="initialState">The initial state of the finite state machine. This must be a valid state defined in the state machine's
    /// configuration.</param>
    /// <returns>An instance of <see cref="IFiniteStateMachine{TClass, TState, TTrigger}"/> representing the configured finite
    /// state machine.</returns>
    IFiniteStateMachine<TClass, TState, TTrigger> Build(TClass instance, TState initialState);

    /// <summary>
    /// Builds and returns a finite state machine configured with the specified initial state.
    /// </summary>
    /// <remarks>The returned finite state machine is fully configured and ready for use. Ensure that the
    /// initial state provided is part of the defined state set.</remarks>
    /// <param name="initialState">The state in which the finite state machine will start. This must be a valid state defined in the state machine
    /// configuration.</param>
    /// <returns>An instance of <see cref="IFiniteStateMachine{TClass, TState, TTrigger}"/> initialized with the specified
    /// initial state.</returns>
    IFiniteStateMachine<TClass, TState, TTrigger> Build(TState initialState);
}
