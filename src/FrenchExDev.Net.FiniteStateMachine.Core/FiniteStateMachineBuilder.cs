namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides a builder for constructing a finite state machine with customizable states, transitions, and behaviors.
/// </summary>
/// <remarks>This class allows users to define valid states, configure transitions between states, and specify
/// optional conditions and actions to be executed during transitions. Once configured, the state machine can be built
/// and used to manage state transitions for the associated instance of <typeparamref name="TClass"/>.</remarks>
/// <typeparam name="TClass">The type of the class instance associated with the state machine. Must be a non-nullable reference type with a
/// parameterless constructor.</typeparam>
/// <typeparam name="TState">The type representing the states of the finite state machine. Must be a non-nullable type.</typeparam>
/// <typeparam name="TTrigger">The type representing the triggers that cause state transitions. Must be a non-nullable type.</typeparam>
public class FiniteStateMachineBuilder<TClass, TState, TTrigger>
    /// Using the generic type constraints to ensure TClass is a non-nullable reference type with a parameterless constructor,
    where TClass : notnull, new()
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Holds the transitions for each state.
    /// </summary>
    protected readonly Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> _transitions = new();

    /// <summary>
    /// Represents a collection of valid states for the current context.
    /// </summary>
    /// <remarks>This field is intended to store a list of states that are considered valid for the operation
    /// or process being implemented. It is initialized as an empty list and can be populated as needed by derived
    /// classes or during initialization.</remarks>
    protected readonly List<TState> _validStates = new();

    /// <summary>
    /// Configures the specified state within the finite state machine.
    /// </summary>
    /// <remarks>If the specified state is not already part of the valid states, it will be added. 
    /// Additionally, if no transitions are defined for the state, an empty transition list  will be initialized for
    /// it.</remarks>
    /// <param name="state">The state to be added or configured in the finite state machine.</param>
    /// <returns>The current instance of <see cref="FiniteStateMachineBuilder{TClass, TState, TTrigger}"/>,  allowing for method
    /// chaining.</returns>
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

    /// <summary>
    /// Defines a transition in the finite state machine that occurs when a specific trigger is activated,
    /// </summary>
    /// <param name="on"></param>
    /// <param name="fromState"></param>
    /// <param name="toState"></param>
    /// <param name="body"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public FiniteStateMachineBuilder<TClass, TState, TTrigger> Transition(TTrigger on, TState fromState, TState toState, Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>? body = null, Func<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>, bool>? condition = null)
    {
        /// adds both fromState and toState as valid states
        State(fromState);
        State(toState);

        /// adds the transition to the transitions dictionary
        _transitions[fromState].Add(new Transition<TClass, TState, TTrigger>(fromState, toState, on, body, condition));

        return this;
    }

    /// <summary>
    /// Builds and returns a finite state machine for the specified instance and initial state.
    /// </summary>
    /// <param name="instance">The object instance that the finite state machine will manage.</param>
    /// <param name="initialState">The initial state of the finite state machine.</param>
    /// <returns>A <see cref="FiniteStateMachine{TClass, TState, TTrigger}"/> configured with the specified instance,  initial
    /// state, valid states, and transitions.</returns>
    public virtual FiniteStateMachine<TClass, TState, TTrigger> Build(TClass instance, TState initialState)
    {
        // Uses the Create method to instantiate the FiniteStateMachine with the provided parameters.
        return FiniteStateMachine<TClass, TState, TTrigger>.Create(instance, initialState, _validStates, _transitions);
    }

    /// <summary>
    /// Builds and returns a new instance of a finite state machine initialized with the specified initial state.
    /// </summary>
    /// <param name="initialState">The initial state of the finite state machine. Must be a valid state defined in the state machine configuration.</param>
    /// <returns>A new instance of <see cref="FiniteStateMachine{TClass, TState, TTrigger}"/> initialized with the specified
    /// initial state.</returns>
    public virtual FiniteStateMachine<TClass, TState, TTrigger> Build(TState initialState)
    {
        // Uses the Create method to instantiate the FiniteStateMachine with a new instance of TClass and the provided initial state.
        return FiniteStateMachine<TClass, TState, TTrigger>.Create(new TClass(), initialState, _validStates, _transitions);
    }
}
