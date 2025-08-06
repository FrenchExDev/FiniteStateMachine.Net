namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;
using System.Linq;

public enum TransitionResult
{
    Success,
    InvalidTransition,
    ConditionNotMet
}

public class Transition<TClass, TState, TTrigger>
{
    public TState FromState { get; }
    public TState ToState { get; }
    public TTrigger Trigger { get; }
    public Func<TClass, bool>? Condition { get; }
    public Action<TClass>? Body { get; }

    public Transition(TState fromState, TState toState, TTrigger trigger, Action<TClass>? body = null, Func<TClass, bool>? condition = null)
    {
        FromState = fromState;
        ToState = toState;
        Trigger = trigger;
        Body = body;
        Condition = condition;
    }
}

public class FiniteStateMachineBuilder<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    private readonly Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> _transitions = new();
    private readonly List<TState> _validStates = new();

    public FiniteStateMachineBuilder<TClass, TState, TTrigger> State(TState state)
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

    public FiniteStateMachineBuilder<TClass, TState, TTrigger> Transition(TState fromState, TState toState, TTrigger on, Action<TClass>? body = null, Func<TClass, bool>? condition = null)
    {
        State(fromState);
        State(toState);
        _transitions[fromState].Add(new Transition<TClass, TState, TTrigger>(fromState, toState, on, body, condition));
        return this;
    }

    public FiniteStateMachine<TClass, TState, TTrigger> Build(TClass instance, TState initialState)
    {
        return FiniteStateMachine<TClass, TState, TTrigger>.Create(instance, initialState, _validStates, _transitions);
    }

    public FiniteStateMachine<TClass, TState, TTrigger> Build(TState initialState)
    {
        return FiniteStateMachine<TClass, TState, TTrigger>.Create(new TClass(), initialState, _validStates, _transitions);
    }
}

public class FiniteStateMachine<TClass, TState, TTrigger>
    where TClass : notnull
    where TState : notnull
    where TTrigger : notnull
{
    private TClass _object;
    private TState _currentState;
    private readonly Dictionary<TState, List<Transition<TClass, TState, TTrigger>>> _transitions = new();
    private readonly List<TState> _validStates = new();

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
        return new FiniteStateMachine<TClass, TState, TTrigger>(initialObject, initialState, states, transitions);
    }

    public TransitionResult Fire(TTrigger trigger)
    {
        if (!_transitions.ContainsKey(_currentState))
            return TransitionResult.InvalidTransition;

        var possibleTransitions = _transitions[_currentState]
            .Where(t => t.Trigger.Equals(trigger))
            .ToList();

        var validTransition = possibleTransitions.FirstOrDefault(t => t.Condition == null || t.Condition(_object));

        if (validTransition == null)
            return possibleTransitions.Any() ? TransitionResult.ConditionNotMet : TransitionResult.InvalidTransition;

        _currentState = validTransition.ToState;

        validTransition.Body?.Invoke(_object);

        return TransitionResult.Success;
    }

    public IEnumerable<TTrigger> GetPossibleTriggers()
    {
        return _transitions[_currentState]?.Select(t => t.Trigger) ?? Enumerable.Empty<TTrigger>();
    }
}