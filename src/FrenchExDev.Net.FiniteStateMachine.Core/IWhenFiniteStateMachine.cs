namespace FrenchExDev.Net.FiniteStateMachine.Core;

using System;
using System.Collections.Generic;

public interface IWhenFiniteStateMachine<TClass, TState, TTrigger> : IFiniteStateMachine<TClass, TState, TTrigger>
    where TClass : notnull, new()
    where TState : notnull
    where TTrigger : notnull
{
    IReadOnlyDictionary<TState, List<Action<TClass, TTrigger, IFiniteStateMachine<TClass, TState, TTrigger>>>> When { get; }
}
