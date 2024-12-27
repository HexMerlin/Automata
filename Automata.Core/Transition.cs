namespace Automata.Core;

/// <summary>
/// Represents a (symbolic) transition in an automaton, defined by a starting state, a symbol, and an ending state.
/// </summary>
/// <remarks>A <see cref="Transition"/> always has a (non-epsilon) symbol and cannot represent an epsilon transition</remarks>
/// <param name="FromState">The state from which the transition starts.</param>
/// <param name="Symbol">The symbol that triggers the transition.</param>
/// <param name="ToState">The state to which the transition goes.</param>
public readonly record struct Transition(int FromState, int Symbol, int ToState) : ITransition<Transition>, IComparable<Transition>
{
    public static Transition Invalid => new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Transition"/> struct with default values.
    /// </summary>
    public Transition() : this(Constants.InvalidState, Constants.InvalidSymbolIndex, Constants.InvalidState) {}

    /// <summary>
    /// Indicates whether the transition is invalid.
    /// </summary>
    public bool IsInvalid => FromState == Constants.InvalidState;

    /// <summary>
    /// Reverses the transition.
    /// </summary>
    /// <returns>A new <see cref="Transition"/> with the from and to states swapped.</returns>
    public Transition Reverse() => new Transition(ToState, Symbol, FromState);

    /// <summary>
    /// Compares the current transition to another transition.
    /// </summary>
    /// <param name="other">The other transition to compare to.</param>
    /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(Transition other)
    {
        int c = FromState.CompareTo(other.FromState);
        if (c != 0) return c;

        c = Symbol.CompareTo(other.Symbol);
        if (c != 0) return c;

        return ToState.CompareTo(other.ToState);
    }

    /// <summary>
    /// Gets a comparer that compares transitions by their to states.
    /// </summary>
    /// <returns>A comparer that compares transitions by their to states.</returns>
    public static Comparer<Transition> CompareByToState() => Comparer<Transition>.Create((t1, t2) =>
    {
        int c = t1.ToState.CompareTo(t2.ToState);
        if (c != 0) return c;

        c = t1.Symbol.CompareTo(t2.Symbol);
        if (c != 0) return c;

        return t1.FromState.CompareTo(t2.FromState);
    });
    

}
