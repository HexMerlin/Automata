using System.Runtime.CompilerServices;

namespace Automata.Core;

/// <summary>
/// Represents a (symbolic) transition in an automaton, defined by a starting state, a symbol, and an ending state.
/// </summary>
/// <remarks>A <see cref="Transition"/> always has a (non-epsilon) symbol and cannot represent an epsilon transition</remarks>
/// <param name="FromState">The state from which the transition starts.</param>
/// <param name="Symbol">The symbol that triggers the transition.</param>
/// <param name="ToState">The state to which the transition goes.</param>
public readonly record struct Transition(int FromState, int Symbol, int ToState) : IComparable<Transition>
{
    /// <summary>
    /// Invalid transition.
    /// </summary>
    public static Transition Invalid => new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Transition"/> struct that is equivalent to <see cref="Invalid"/>.
    /// </summary>
    public Transition() : this(Constants.InvalidState, Constants.InvalidSymbolIndex, Constants.InvalidState) { }

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
    /// <param name="other">Other transition to compare to.</param>
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
    /// String that represents the current transition.
    /// </summary>
    /// <returns>A string that represents the current transition.</returns>
    public override string ToString() => $"{FromState}=>{ToState} ({Symbol})";

    /// <summary>
    /// Creates a minimum transition for the given state and symbol.
    /// </summary>
    /// <param name="fromState">From state.</param>
    /// <param name="symbol">Symbol for the transition (default is <see cref="int.MinValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the minimum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transition MinTrans(int fromState, int symbol = int.MinValue) => new(fromState, symbol, int.MinValue);

    /// <summary>
    /// Creates a maximum transition for the given state and symbol.
    /// </summary>
    /// <param name="fromState">From state.</param>
    /// <param name="symbol">Symbol for the transition (default is <see cref="int.MaxValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the maximum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transition MaxTrans(int fromState, int symbol = int.MaxValue) => new(fromState, symbol, int.MaxValue);
}
