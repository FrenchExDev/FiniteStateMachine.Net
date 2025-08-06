namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System.Collections.Generic;

public interface IFiniteStateMachine<TClass, TState, TTrigger>
    where TClass : notnull
    where TState : notnull
    where TTrigger : notnull
{
    TransitionResult Fire(TTrigger trigger);
    IEnumerable<TTrigger> GetPossibleTriggers();
    TState CurrentState { get; }
    TClass Object { get; }
}
