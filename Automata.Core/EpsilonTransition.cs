namespace Automata.Core;

/// <summary>
/// Represents an epsilon transition in an automaton, defined by a starting state and an ending state.
/// </summary>
/// <param name="FromState">The state from which the transition starts.</param>
/// <param name="ToState">The state to which the transition goes.</param>
public readonly record struct EpsilonTransition(int FromState, int ToState) : ITransition<EpsilonTransition>, IComparable<EpsilonTransition>
{

    /// <summary>
    /// Denotes the string representation for ε - the empty epsilon string.
    /// Epsilon is not part of any alphabet, the symbol is used solely for output purposes, never for computation.
    /// </summary>
    public const string Epsilon = "ε";

    /// <summary>
    /// Reverses the epsilon transition.
    /// </summary>
    /// <returns>A new <see cref="EpsilonTransition"/> with the from and to states swapped.</returns>
    public EpsilonTransition Reverse() => new(ToState, FromState);


    /// <summary>
    /// Compares the current epsilon transition to another epsilon transition.
    /// </summary>
    /// <param name="other">The other epsilon transition to compare to.</param>
    /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(EpsilonTransition other)
    {
        int c = FromState.CompareTo(other.FromState);
        return c != 0 ? c : ToState.CompareTo(other.ToState);
    }

    /// <summary>
    /// Gets a comparer that compares epsilon transitions by their to states.
    /// </summary>
    /// <returns>A comparer that compares epsilon transitions by their to states.</returns>
    public static Comparer<EpsilonTransition> CompareByToState() => Comparer<EpsilonTransition>.Create((t1, t2) =>
    {
        int c = t1.ToState.CompareTo(t2.ToState);
        return c != 0 ? c : t1.FromState.CompareTo(t2.FromState);
    });

}

