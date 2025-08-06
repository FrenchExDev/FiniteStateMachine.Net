namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System.Collections.Generic;
using System.Linq;

public class FiniteStateMachine<TClass, TState, TTrigger> : IFiniteStateMachine<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    protected TClass _object;
    public TClass Object => _object;
    protected TState _currentState;
    protected readonly Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> _transitions = new();
    protected readonly List<TState> _validStates = new();
    public TState CurrentState => _currentState;

    protected FiniteStateMachine(TClass instance, TState initialState, List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions)
    {
        _object = instance;
        _currentState = initialState;
        _transitions = transitions;
        _validStates = states;
    }

    public static FiniteStateMachine<TClass, TState, TTrigger> Create(TClass initialObject, TState initialState, List<TState> states, Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> transitions)
    {
        var fsm = new FiniteStateMachine<TClass, TState, TTrigger>(initialObject, initialState, states, transitions);
        return fsm;
    }

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

    public virtual IEnumerable<TTrigger> GetPossibleTriggers()
    {
        return _transitions[_currentState]?.Select(t => t.Trigger) ?? Enumerable.Empty<TTrigger>();
    }
}