using System.Runtime.CompilerServices;
using Automata.Core.Alang;

namespace Automata.Core;

/// <summary>
/// Represents a (symbolic) transition in an automaton, defined by a starting state, a symbol, and an ending state.
/// </summary>
/// <remarks>A <see cref="Transition"/> always has a (non-epsilon) symbol and cannot represent an epsilon transition</remarks>
public readonly record struct Transition : IComparable<Transition>
{

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

    /// <summary>
    /// Initializes a new instance of the <see cref="Transition"/> struct.
    /// </summary>
    /// <param name="FromState">The state origin of the transition.</param>
    /// <param name="Symbol">The symbol for the transition.</param>
    /// <param name="ToState">The destination state of the transition.</param>
    /// <exception cref="ArgumentException">Thrown if any of arguments has a negative value.</exception>
    public Transition(int fromState, int symbol, int toState) 
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(fromState, 0, nameof(fromState));
        ArgumentOutOfRangeException.ThrowIfLessThan(symbol, 0, nameof(symbol));
        ArgumentOutOfRangeException.ThrowIfLessThan(toState, 0, nameof(toState));
        FromState = fromState;
        Symbol = symbol;
        ToState = toState;
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
    /// Creates a search key transition with the from state specified.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <returns>A <see cref="Transition"/> with the specified <paramref name="fromState"/>, <see cref="Constants.InvalidSymbolIndex"/> as the symbol, and <see cref="Constants.InvalidState"/> as the destination state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Transition FromStateSearchKey(int fromState) => new Transition(fromState, Constants.InvalidSymbolIndex, Constants.InvalidState, Unchecked.Yes);

    /// <summary>
    /// Creates a search key transition with the from state specified.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <param name="symbol">The symbol for the transition.</param>
    /// <returns>A <see cref="Transition"/> with the specified <paramref name="fromState"/>, <see cref="Constants.InvalidSymbolIndex"/> as the symbol, and <see cref="Constants.InvalidState"/> as the destination state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Transition FromStateSymbolSearchKey(int fromState, int symbol) => new Transition(fromState, symbol, Constants.InvalidState, Unchecked.Yes);

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
    public override string ToString() => $"{FromState}->{ToState} ({Symbol})";

    /// <summary>
    /// Creates a minimum transition for the given state and symbol.
    /// </summary>
    /// <param name="fromState">From state.</param>
    /// <param name="symbol">Symbol for the transition (default is <see cref="int.MinValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the minimum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transition MinTrans(int fromState, int symbol = int.MinValue) => new(fromState, symbol, int.MinValue, Unchecked.Yes);

    /// <summary>
    /// Creates a maximum transition for the given state and symbol.
    /// </summary>
    /// <param name="fromState">From state.</param>
    /// <param name="symbol">Symbol for the transition (default is <see cref="int.MaxValue"/>).</param>
    /// <returns>A <see cref="Transition"/> representing the maximum transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transition MaxTrans(int fromState, int symbol = int.MaxValue) => new(fromState, symbol, int.MaxValue, Unchecked.Yes);
}
