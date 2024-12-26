namespace Automata.Core;

/// <summary>
/// Represents a generic transition with static support for obtaining a comparer.
/// </summary>
/// <typeparam name="T">The type of transition.</typeparam>
public interface ITransition<T> where T : struct
{
    ///<summary>The state from which the transition starts.</summary>
    int FromState { get; }

    ///<summary>The state to which the transition goes.</summary>
    int ToState { get; }

    /// <summary>
    /// Gets the comparer for ordering by "to state".
    /// </summary>
    static abstract Comparer<T> CompareByToState();
}