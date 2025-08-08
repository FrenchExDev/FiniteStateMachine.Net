namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A Finite State Machine implementation.
/// </summary>
/// <typeparam name="TClass">The attached object, will be provided as arg in functions</typeparam>
/// <typeparam name="TState">The State type</typeparam>
/// <typeparam name="TTrigger">The Trigger type</typeparam>
public class FiniteStateMachine<TClass, TState, TTrigger> : IFiniteStateMachine<TClass, TState, TTrigger>
    /// Using the generic type constraints to ensure TClass is a non-nullable reference type with a parameterless constructor,
    where TClass : notnull, new()
    /// and TState and TTrigger as non-nullable reference types.
    where TState : notnull
    where TTrigger : notnull
{
    /// <summary>
    /// Returns the current object instance that the FSM is managing.
    /// </summary>
    public TClass Object => _object;

    /// <summary>
    /// Returns the current state of the FSM.
    /// </summary>
    public TState CurrentState => _currentState;

    /// <summary>
    /// Holds the current object instance that the FSM is managing.
    /// </summary>
    protected TClass _object;

    /// <summary>
    /// Holds the current state of the FSM.
    /// </summary>
    protected TState _currentState;

    /// <summary>
    ///  Holds the transitions for each state.
    /// </summary>
    protected readonly Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> _transitions = new();

    /// <summary>
    /// Holds the valid states for the FSM.
    /// </summary>
    protected readonly List<TState> _validStates = new();

    /// <summary>
    /// Constructor for the FiniteStateMachine.
    /// </summary>
    /// <param name="instance">The object being passed as subject to transitions bodies</param>
    /// <param name="initialState">The initial state of the FSM</param>
    /// <param name="states">States </param>
    /// <param name="transitions"></param>
    protected FiniteStateMachine(
        TClass instance, 
        TState initialState, 
        List<TState> states, 
        Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions
    )
    {
        _object = instance;
        _currentState = initialState;
        _transitions = transitions;
        _validStates = states;
    }

    /// <summary>
    /// Creates a new instance of a finite state machine with the specified initial object, initial state, states, and
    /// transitions.
    /// </summary>
    /// <param name="initialObject">The object associated with the state machine. Cannot be <see langword="null"/>.</param>
    /// <param name="initialState">The initial state of the state machine. Must be included in the <paramref name="states"/> collection.</param>
    /// <param name="states">A list of all possible states for the state machine. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="transitions">A dictionary mapping each state to a list of transitions that define valid state changes. Each key must
    /// correspond to a state in <paramref name="states"/>. Cannot be <see langword="null"/>.</param>
    /// <returns>A new instance of <see cref="FiniteStateMachine{TClass, TState, TTrigger}"/> initialized with the specified
    /// parameters.</returns>
    public static FiniteStateMachine<TClass, TState, TTrigger> Create(TClass initialObject, TState initialState, List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions)
    {
        var fsm = new FiniteStateMachine<TClass, TState, TTrigger>(initialObject, initialState, states, transitions);
        return fsm;
    }

    /// <summary>
    /// Fires a transition based on the provided trigger.
    /// </summary>
    /// <param name="trigger">The trigger that initiates the transition.</param>
    /// <returns>The result of the transition attempt.</returns>
    public virtual TransitionResult Fire(TTrigger trigger)
    {
        if (!_transitions.ContainsKey(_currentState))
            return TransitionResult.InvalidTransition;

        var possibleTransitions = _transitions[_currentState]
            .Where(t => t.Trigger.Equals(trigger))
            .ToList();

        var validTransition = possibleTransitions.FirstOrDefault(t => t.Condition == null || t.Condition(_object, trigger, this));

        if (validTransition == null)
            return possibleTransitions.Any() ? TransitionResult.ConditionNotMet : TransitionResult.InvalidTransition;

        _currentState = validTransition.ToState;

        validTransition.Body?.Invoke(_object, trigger, this);

        return TransitionResult.Success;
    }

    /// <summary>
    /// Retrieves all possible triggers for the current state.
    /// </summary>
    /// <remarks>This method returns the triggers associated with the transitions available from the current
    /// state. If no transitions are defined for the current state, an empty collection is returned.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the triggers for the current state.  Returns an empty collection if
    /// no transitions are available.</returns>
    public virtual IEnumerable<TTrigger> GetPossibleTriggers()
    {
        return _transitions[_currentState]?.Select(t => t.Trigger) ?? Enumerable.Empty<TTrigger>();
    }
}