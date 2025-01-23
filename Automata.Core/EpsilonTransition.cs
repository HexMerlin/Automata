using System.Runtime.CompilerServices;
using Automata.Core.Alang;

namespace Automata.Core;

/// <summary>
/// An epsilon transition in an automaton, defined by a starting state and an ending state.
/// </summary>
/// <remarks>An epsilon transition is a transition that lacks a symbol.
/// Epsilon transitions can only exist in non-deterministic finite automata (NFA).
/// </remarks>
public readonly record struct EpsilonTransition : IComparable<EpsilonTransition>
{
    #region Data

    /// <summary>
    /// The state origin of the transition.
    /// </summary>
    public int FromState { get; }

    /// <summary>
    /// The destination state of the transition.
    /// </summary>
    public int ToState { get; }

    #endregion Data

    /// <summary>
    /// Initializes a new instance of the <see cref="EpsilonTransition"/> struct.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <param name="toState">The destination state of the transition.</param>
    /// <exception cref="ArgumentException">Thrown if any of arguments has a negative value.</exception>
    public EpsilonTransition(int fromState, int toState)
    {
        FromState = fromState.ShouldNotBeNegative();
        ToState = toState.ShouldNotBeNegative();
    }

    /// <summary>
    /// String representation for ε - the empty epsilon string.
    /// Epsilon is not part of any alphabet; the symbol is used solely for output purposes, never for computation.
    /// </summary>
    public const string Epsilon = "ε";

    /// <summary>
    /// Private enum for private constructor without argument checks.
    /// </summary>
    private enum Unchecked { Yes }

    /// <summary>
    /// Unsafe private constructor that does not check arguments.
    /// </summary>
    private EpsilonTransition(int fromState, int toState, Unchecked _)
    {
        FromState = fromState;
        ToState = toState;
    }

    /// <summary>
    /// A search key with the minimum possible value for the toState.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static EpsilonTransition MinTransSearchKey(int fromState) => new(fromState, int.MinValue, Unchecked.Yes);

    /// <summary>
    /// A search key with the maximum possible value for the toState.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static EpsilonTransition MaxTransSearchKey(int fromState) => new(fromState, int.MaxValue, Unchecked.Yes);

    /// <summary>
    /// Reverses the epsilon transition.
    /// </summary>
    /// <returns>A new <see cref="EpsilonTransition"/> with the from and to states swapped.</returns>
    public EpsilonTransition Reverse() => new(ToState, FromState);

    /// <summary>
    /// Compares the current epsilon transition to another epsilon transition.
    /// </summary>
    /// <param name="other">Other epsilon transition to compare to.</param>
    /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(EpsilonTransition other)
    {
        int c = FromState.CompareTo(other.FromState);
        return c != 0 ? c : ToState.CompareTo(other.ToState);
    }

    /// <summary>
    /// String that represents the current transition.
    /// </summary>
    /// <returns>A string that represents the current transition.</returns>
    public override string ToString() => $"{FromState}->{ToState}";

}
