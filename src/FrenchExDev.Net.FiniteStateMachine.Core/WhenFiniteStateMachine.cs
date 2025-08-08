namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a finite state machine that supports executing actions when entering specific states.
/// </summary>
/// <remarks>This class extends the functionality of a standard finite state machine by allowing actions to be
/// executed when specific states are entered. These actions are defined in the <see cref="When"/> dictionary and are
/// invoked after a successful state transition triggered by <see cref="Fire(TTrigger)"/>.</remarks>
/// <typeparam name="TClass">The type of the object associated with the state machine. Must be a non-nullable reference type with a parameterless
/// constructor.</typeparam>
/// <typeparam name="TState">The type representing the states of the state machine. Must be non-nullable.</typeparam>
/// <typeparam name="TTrigger">The type representing the triggers that cause state transitions. Must be non-nullable.</typeparam>
public class WhenFiniteStateMachine<TClass, TState, TTrigger> : FiniteStateMachine<TClass, TState, TTrigger>, IWhenFiniteStateMachine<TClass, TState, TTrigger>
    /// Using the generic type constraints to ensure TClass is a non-nullable reference type with a parameterless constructor,
    where TClass : notnull, new()
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Holds a dictionary mapping states to a list of actions to be executed when the state is active and a specific trigger is fired.
    /// </summary>
    private readonly Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> _when;

    /// <summary>
    /// Gives access to the dictionary that maps states to a list of actions to be executed when the state is active and a specific trigger is fired.
    /// </summary>
    public IReadOnlyDictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> When => _when.AsReadOnly();

    /// <summary>
    /// Creates a new instance of the <see cref="WhenFiniteStateMachine{TClass, TState, TTrigger}"/> class with the
    /// specified initial object, initial state, states, transitions, and actions.
    /// </summary>
    /// <param name="initialObject">The initial object associated with the finite state machine. Cannot be <see langword="null"/>.</param>
    /// <param name="initialState">The initial state of the finite state machine. Must be a valid state included in <paramref name="states"/>.</param>
    /// <param name="states">A list of all possible states for the finite state machine. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="transitions">A dictionary mapping each state to a list of transitions that define the valid state changes for that state.
    /// Each transition specifies the conditions under which the state change occurs. Cannot be <see langword="null"/>.</param>
    /// <param name="when">A dictionary mapping each state to a list of actions to be executed when the state is active and a trigger is
    /// received. Each action is invoked with the associated object, the trigger, and the finite state machine instance.
    /// Cannot be <see langword="null"/>.</param>
    /// <returns>A new instance of the <see cref="WhenFiniteStateMachine{TClass, TState, TTrigger}"/> class initialized with the
    /// specified parameters.</returns>
    public static WhenFiniteStateMachine<TClass, TState, TTrigger> Create(
        TClass initialObject,
        TState initialState,
        List<TState> states,
        Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions,
        Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> when
    )
    {
        var fsm = new WhenFiniteStateMachine<TClass, TState, TTrigger>(initialObject, initialState, states, transitions, when);
        return fsm;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhenFiniteStateMachine{TClass, TState, TTrigger}"/> class with the
    /// specified initial state, states, transitions, and actions.
    /// </summary>
    /// <param name="initialState">The initial state of the finite state machine. This state must be included in the <paramref name="states"/>
    /// collection.</param>
    /// <param name="states">A list of all possible states for the finite state machine. This collection must include the <paramref
    /// name="initialState"/>.</param>
    /// <param name="transitions">A dictionary mapping each state to a list of valid transitions from that state. Each transition defines the
    /// conditions under which the state changes.</param>
    /// <param name="when">A dictionary mapping each state to a list of actions to execute when the state is entered. Each action is
    /// invoked with the current context, trigger, and state machine instance.</param>
    /// <returns>A new instance of the <see cref="WhenFiniteStateMachine{TClass, TState, TTrigger}"/> class configured with the
    /// specified states, transitions, and actions.</returns>
    public static WhenFiniteStateMachine<TClass, TState, TTrigger> Create(
        TState initialState,
        List<TState> states,
        Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions,
        Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> when
    )
    {
        var fsm = new WhenFiniteStateMachine<TClass, TState, TTrigger>(new(), initialState, states, transitions, when);
        return fsm;
    }

    /// <summary>
    /// Constructor for the WhenFiniteStateMachine class.
    /// </summary>
    /// <param name="instance">Instance</param>
    /// <param name="initialState">Initial state</param>
    /// <param name="states">List of all possible states</param>
    /// <param name="transitions">Dictionary of state transitions</param>
    /// <param name="when">Dictionary of actions to execute when the state is entered</param>
    protected WhenFiniteStateMachine(
        TClass instance,
        TState initialState,
        List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions,
        Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> when
    ) : base(instance, initialState, states, transitions)
    {
        _when = when;
    }

    /// <summary>
    /// Fires the specified trigger, causing a state transition if the trigger is valid for the current state.
    /// </summary>
    /// <remarks>If the state transition is successful, any associated actions for the new state are executed.
    /// These actions are defined for the current state and are invoked with the object, the trigger,  and the state
    /// machine instance as parameters.</remarks>
    /// <param name="trigger">The trigger to fire, which may cause a state transition.</param>
    /// <returns>A <see cref="TransitionResult"/> indicating the outcome of the operation.  Returns <see
    /// cref="TransitionResult.Success"/> if the state transition was successful; otherwise,  returns a result
    /// indicating the reason for failure.</returns>
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
