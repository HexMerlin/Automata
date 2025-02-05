using System.Runtime.CompilerServices;
using Automata.Core.Alang;

namespace Automata.Core;

/// <summary>
/// Represents a (symbolic) transition in an automaton, defined by a starting state, a symbol, and an ending state.
/// </summary>
/// <remarks>A <see cref="Transition"/> always has a (non-epsilon) symbol and cannot represent an epsilon transition</remarks>
public readonly record struct Transition : IComparable<Transition>
{
    #region Data

    /// <summary>
    /// The state origin of the transition.
    /// </summary>
    public int FromState { get; }
    
    /// <summary>
    /// Symbol for the transition.
    /// </summary>
    public int Symbol { get; }

    /// <summary>
    /// The destination state of the transition.
    /// </summary>
    public int ToState { get; }

    #endregion Data

    #region Accessors

    /// <summary>
    /// Initializes a new instance of the <see cref="Transition"/> struct.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <param name="symbol">The symbol for the transition.</param>
    /// <param name="toState">The destination state of the transition.</param>
    /// <exception cref="ArgumentException">Thrown if any of arguments has a negative value.</exception>
    public Transition(int fromState, int symbol, int toState) 
    {
        FromState = fromState.ShouldNotBeNegative();
        Symbol = symbol.ShouldNotBeNegative();
        ToState = toState.ShouldNotBeNegative();
    }

    /// <summary>
    /// Private enum for private constructor without argument checks.
    /// </summary>
    private enum Unchecked { Yes }

    /// <summary>
    /// Unsafe private constructor that does not check arguments.
    /// </summary>
    private Transition(int fromState, int symbol, int toState, Unchecked _)
    {
        FromState = fromState;
        Symbol = symbol;
        ToState = toState;
    }

    /// <summary>
    /// Invalid transition.
    /// </summary>
    public static Transition Invalid => new(Constants.InvalidState, Constants.InvalidSymbolIndex, Constants.InvalidState, Unchecked.Yes);

    /// <summary>
    /// Creates a minimum transition for the given state and symbol.
    /// </summary>
    /// <remarks>
    /// The returned transition will sort directly before any other transition with the given properties.
    /// This is an invalid Transition, use this only as a search key for binary search. 
    /// </remarks>
    /// <param name="fromState">From state.</param>
    /// <param name="symbol">Symbol for the transition (default is <see cref="int.MinValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the minimum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Transition MinTrans(int fromState, int symbol = int.MinValue) => new(fromState, symbol, int.MinValue, Unchecked.Yes);

    /// <summary>
    /// Creates a maximum transition for the given state and symbol.
    /// </summary>
    /// <remarks>
    /// The returned transition will sort directly after any other transition with the given properties.
    /// This is an invalid Transition, use this only as a search key for binary search. 
    /// </remarks>
    /// <param name="fromState">From state.</param>
    /// <param name="symbol">Symbol for the transition (default is <see cref="int.MaxValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the maximum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Transition MaxTrans(int fromState, int symbol = int.MaxValue) => new(fromState, symbol, int.MaxValue, Unchecked.Yes);

    /// <summary>
    /// Min search key used for reverse sorted transitions (using <see cref="ComparerByToState"/>).
    /// </summary>
    /// <param name="toState">A given toState</param>
    /// <returns>A transition that will be less than all other transitions with the same <see cref="ToState"/></returns>
    /// <seealso cref="Transition.ComparerByToState"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Transition MinTransReverse(int toState) => new(int.MinValue, int.MinValue, toState, Unchecked.Yes);

    /// <summary>
    /// Max search key used for reverse sorted transitions (using <see cref="ComparerByToState"/>).
    /// </summary>
    /// <param name="toState">A given toState</param>
    /// <returns>A transition that will be greater than all other transitions with the same <see cref="ToState"/></returns>
    /// <seealso cref="Transition.ComparerByToState"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Transition MaxTransReverse(int toState) => new(int.MaxValue, int.MaxValue, toState, Unchecked.Yes);


    /// <summary>
    /// Indicates whether the transition is invalid.
    /// </summary>
    public bool IsInvalid => FromState == Constants.InvalidState;

    /// <summary>
    /// Reverses the transition.
    /// </summary>
    /// <returns>A new <see cref="Transition"/> with the from and to states swapped.</returns>
    public Transition Reverse() => new(ToState, Symbol, FromState);

    /// <summary>
    /// Compares the current transition to another transition by default order: {FromState, Symbol, ToState}.
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
    /// A comparer that compares transitions by their to states.
    /// </summary>
    /// <returns>A comparer that compares transitions in reversed order: {ToState, Symbol, FromState}.</returns>
    public static Comparer<Transition> ComparerByToState() => Comparer<Transition>.Create((t1, t2) =>
    {
        int c = t1.ToState.CompareTo(t2.ToState);
        if (c != 0) return c;

        c = t1.Symbol.CompareTo(t2.Symbol);
        if (c != 0) return c;

        return t1.FromState.CompareTo(t2.FromState);
    });

    /// <summary>
    /// String that represents the current transition.
    /// </summary>
    /// <returns>A string that represents the current transition.</returns>
    public override string ToString() => $"{FromState}->{ToState} ({Symbol})";

    #endregion Accessors
}
