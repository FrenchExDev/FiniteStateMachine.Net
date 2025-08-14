namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides a builder for creating and configuring a finite state machine with "when" conditions.
/// </summary>
/// <remarks>This builder extends the functionality of <see cref="FiniteStateMachineBuilder{TClass, TState,
/// TTrigger}"/>  by allowing the configuration of actions to be executed when the state machine enters specific states.
/// It supports defining state transitions, conditions, and actions to be performed during transitions or  when entering
/// a state.</remarks>
/// <typeparam name="TClass">The type of the class that represents the context or instance associated with the state machine.  Must be a
/// non-nullable reference type and have a parameterless constructor.</typeparam>
/// <typeparam name="TState">The type representing the states of the finite state machine. Must be a non-nullable type.</typeparam>
/// <typeparam name="TTrigger">The type representing the triggers that cause state transitions in the finite state machine.  Must be a non-nullable
/// type.</typeparam>
public class WhenFiniteStateMachineBuilder<TClass, TState, TTrigger> : FiniteStateMachineBuilder<TClass, TState, TTrigger>, IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger>
    /// Using the generic type constraints to ensure TClass is a non-nullable reference type with a parameterless constructor,
    where TClass : notnull, new()
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Holds a dictionary mapping states to a list of actions to be executed when the state is active and a specific trigger is fired.
    /// </summary>
    private Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> _whenState = new();

    /// <summary>
    /// Represents a mapping of triggers to a list of actions to be executed when the trigger is activated.
    /// </summary>
    /// <remarks>Each trigger is associated with a list of actions that are executed in the context of a
    /// finite state machine. The actions take the current state machine class, the trigger, and the state machine
    /// instance as parameters.</remarks>

    private Dictionary<TTrigger, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> _whenTrigger = new();


    /// <summary>
    /// Builds and returns a finite state machine configured with the specified instance and initial state.
    /// </summary>
    /// <param name="instance">The instance of the class that the finite state machine will operate on. This parameter cannot be null.</param>
    /// <param name="initialState">The initial state of the finite state machine. This must be a valid state defined in the state machine's
    /// configuration.</param>
    /// <returns>An <see cref="IWhenFiniteStateMachine{TClass, TState, TTrigger}"/> instance representing the configured finite state
    /// machine.</returns>
    public IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TClass instance, TState initialState)
    {
        return WhenFiniteStateMachine<TClass, TState, TTrigger>.Create(instance, initialState, _validStates, _transitions, _whenState, _whenTrigger);
    }

    /// <summary>
    /// Creates and returns a finite state machine configured with the specified initial state.
    /// </summary>
    /// <remarks>The returned state machine is configured with the states, transitions, and conditions defined
    /// during the setup process.</remarks>
    /// <param name="initialState">The initial state of the finite state machine. Must be a valid state defined in the state machine.</param>
    /// <returns>An instance of <see cref="IWhenFiniteStateMachine{TClass, TState, TTrigger}"/> representing the configured
    /// finite state machine.</returns>
    public IWhenFiniteStateMachine<TClass, TState, TTrigger> BuildWhen(TState initialState)
    {
        return WhenFiniteStateMachine<TClass, TState, TTrigger>.Create(initialState, _validStates, _transitions, _whenState, _whenTrigger);
    }

    public IFiniteStateMachineBuilder<TClass, TState, TTrigger> Transition(
        TState fromState,
        TState toState,
        TTrigger on,
        Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null,
        Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null
    )
    {
        base.CanTransition(on, fromState, toState, body, condition);
        return this;
    }

    /// <summary>
    /// Configures an action to be executed when the state machine enters the specified state.
    /// </summary>
    /// <remarks>This method allows you to define custom behavior for specific states in the state machine. 
    /// Multiple actions can be added for the same state, and they will be executed in the order they are
    /// added.</remarks>
    /// <param name="state">The state for which the action should be executed.</param>
    /// <param name="action">The action to execute when the state machine enters the specified state.  The action receives the instance of
    /// the class, the trigger that caused the transition,  and the state machine itself as parameters.</param>
    /// <returns>An instance of <see cref="IWhenFiniteStateMachineBuilder{TClass, TState, TTrigger}"/>  to allow for method
    /// chaining.</returns>
    public IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger> When(TState state, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>> action)
    {
        if (!_whenState.ContainsKey(state))
        {
            _whenState[state] = new();
        }

        _whenState[state].Add(action);

        return this;
    }

    /// <summary>
    /// Configures the finite state machine to execute the specified action when the given trigger occurs.
    /// </summary>
    /// <param name="event">The trigger that causes the action to execute. Cannot be null.</param>
    /// <param name="action">The action to execute when the specified trigger occurs. The action receives the current instance of 
    /// <typeparamref name="TClass"/>, the trigger, and the finite state machine as parameters.</param>
    /// <returns>An instance of <see cref="IWhenFiniteStateMachineBuilder{TClass, TState, TTrigger}"/> to allow for method
    /// chaining.</returns>
    public IWhenFiniteStateMachineBuilder<TClass, TState, TTrigger> On(TTrigger @event, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>> action)
    {
        if (!_whenTrigger.ContainsKey(@event))
        {
            _whenTrigger[@event] = new();
        }

        _whenTrigger[@event].Add(action);

        return this;
    }

    /// <summary>
    /// Implements the method to build a finite state machine with the specified instance and initial state.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="initialState"></param>
    /// <returns></returns>
    IFiniteStateMachine<TClass, TState, TTrigger> IFiniteStateMachineBuilder<TClass, TState, TTrigger>.Build(TClass instance, TState initialState)
    {
        return BuildWhen(instance, initialState);
    }

    /// <summary>
    /// Implements the method to build a finite state machine with the specified initial state.
    /// </summary>
    /// <param name="initialState"></param>
    /// <returns></returns>
    IFiniteStateMachine<TClass, TState, TTrigger> IFiniteStateMachineBuilder<TClass, TState, TTrigger>.Build(TState initialState)
    {
        return BuildWhen(initialState);
    }

    /// <summary>
    /// Configures a state in the finite state machine.
    /// </summary>
    /// <param name="state">The state to be added to the finite state machine. This must be a valid state of type <typeparamref
    /// name="TState"/>.</param>
    /// <returns>An instance of <see cref="IFiniteStateMachineBuilder{TClass, TState, TTrigger}"/> to allow for method chaining.</returns>
    IFiniteStateMachineBuilder<TClass, TState, TTrigger> IFiniteStateMachineBuilder<TClass, TState, TTrigger>.State(TState state)
    {
        base.State(state);
        return this;
    }
}
