namespace FrenchExDev.Net.FiniteStateMachine.Core;

/// <summary>
/// Represents the result of a transition in a finite state machine.
/// </summary>
public enum TransitionResult
{
    /// <summary>
    /// On successful transition, the state machine has successfully transitioned to a new state.
    /// </summary>
    Success,

    /// <summary>
    /// The transition is invalid and cannot be executed.
    /// </summary>
    InvalidTransition,

    /// <summary>
    /// Represents an exception that is thrown when a specific condition required for an operation is not met.
    /// </summary>
    /// <remarks>This exception is typically used to indicate that a precondition or business rule has been
    /// violated, preventing the operation from proceeding. Ensure that the required condition is satisfied before
    /// attempting the operation again.</remarks>
    ConditionNotMet
}
