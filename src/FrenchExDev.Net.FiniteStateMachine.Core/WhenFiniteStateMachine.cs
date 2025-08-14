namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a finite state machine that supports executing actions when entering specific states.
/// </summary>
/// <remarks>This class extends the functionality of a standard finite state machine by allowing actions to be
/// executed when specific states are entered. These actions are defined in the <see cref="WhenStates"/> dictionary and are
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
    private readonly Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> _whenStates;

    private readonly Dictionary<TTrigger, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> _whenTriggers = new();

    /// <summary>
    /// Gives access to the dictionary that maps states to a list of actions to be executed when the state is active and a specific trigger is fired.
    /// </summary>
    public IReadOnlyDictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> WhenStates => _whenStates.AsReadOnly();

    /// <summary>
    /// Gets a read-only dictionary that maps triggers to a list of actions to be executed when the corresponding
    /// trigger is activated.
    /// </summary>
    /// <remarks>This property provides a way to inspect the configured triggers and their associated actions 
    /// in the finite state machine. Modifications to the returned dictionary are not allowed, ensuring  the integrity
    /// of the trigger-action mappings.</remarks>
    public IReadOnlyDictionary<TTrigger, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> WhenTriggers => _whenTriggers.AsReadOnly();

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
        Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> whenStates,
        Dictionary<TTrigger, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> whenTriggers
    )
    {
        var fsm = new WhenFiniteStateMachine<TClass, TState, TTrigger>(
            instance: initialObject,
            initialState: initialState,
            states: states,
            transitions: transitions,
            whenStates: whenStates,
            whenTriggers: whenTriggers
        );

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
    /// <param name="whenStates">A dictionary mapping each state to a list of actions to execute when the state is entered. Each action is
    /// invoked with the current context, trigger, and state machine instance.</param>
    /// <param name="whenTriggers">A dictionary mapping each state to a list of actions to execute when the evet is triggered. Each action is
    /// invoked with the current context, trigger, and state machine instance.</param>
    /// <returns>A new instance of the <see cref="WhenFiniteStateMachine{TClass, TState, TTrigger}"/> class configured with the
    /// specified states, transitions, and actions.</returns>
    public static WhenFiniteStateMachine<TClass, TState, TTrigger> Create(
        TState initialState,
        List<TState> states,
        Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions,
        Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> whenStates,
        Dictionary<TTrigger, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> whenTriggers
    )
    {
        var fsm = new WhenFiniteStateMachine<TClass, TState, TTrigger>(new(), initialState, states, transitions, whenStates, whenTriggers);
        return fsm;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhenFiniteStateMachine{TClass, TState, TTrigger}"/> class with the specified parameters.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="initialState"></param>
    /// <param name="states"></param>
    /// <param name="transitions"></param>
    /// <param name="whenStates"></param>
    /// <param name="whenTriggers"></param>
    protected WhenFiniteStateMachine(
        TClass instance,
        TState initialState,
        List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions,
        Dictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> whenStates,
        Dictionary<TTrigger, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> whenTriggers
    ) : base(instance, initialState, states, transitions)
    {
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));
        ArgumentNullException.ThrowIfNull(initialState, nameof(initialState));
        ArgumentNullException.ThrowIfNull(states, nameof(states));
        ArgumentNullException.ThrowIfNull(whenStates, nameof(whenStates));
        ArgumentNullException.ThrowIfNull(whenTriggers, nameof(whenTriggers));
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));
        ArgumentNullException.ThrowIfNull(transitions, nameof(transitions));

        _whenStates = whenStates;
        _whenTriggers = whenTriggers;
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
        var previousState = CurrentState;

        // Check if there are any actions associated with the trigger
        if (_whenTriggers.TryGetValue(trigger, out var opnTrigger))
        {
            foreach (var action in opnTrigger)
            {
                action(Object, trigger, this);
            }
        }

        var result = base.Fire(trigger);

        switch (result)
        {
            case TransitionResult.InvalidTransition:
                return TransitionResult.InvalidTransition;
            case TransitionResult.ConditionNotMet:
                return TransitionResult.ConditionNotMet;
            case TransitionResult.Success:
                if (_whenStates.TryGetValue(CurrentState, out var whenStates))
                    foreach (var action in whenStates)
                    {
                        action(Object, trigger, this);
                    }
                return TransitionResult.Success;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, "Unknown transition result.");
        }
    }
}
