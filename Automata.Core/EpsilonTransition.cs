using System.Runtime.CompilerServices;
using Automata.Core.Alang;

namespace Automata.Core;

/// <summary>
/// An epsilon transition in an automaton, defined by a starting state and an ending state.
/// </summary>
/// <remarks>An epsilon transition is a transition that lacks a symbol.
/// Epsilon transitions can only exist in non-deterministic finite automata (NFA).
/// </remarks>
/// <param name="FromState">The state origin of the transition.</param>
/// <param name="ToState">The destination state of the transition.</param>
public readonly record struct EpsilonTransition(int FromState, int ToState) : IComparable<EpsilonTransition>
{

    /// <summary>
    /// The state origin of the transition.
    /// </summary>
    public int FromState { get; } = FromState;

    /// <summary>
    /// The destination state of the transition.
    /// </summary>
    public int ToState { get; } = ToState;

    /// <summary>
    /// String representation for ε - the empty epsilon string.
    /// Epsilon is not part of any alphabet; the symbol is used solely for output purposes, never for computation.
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EpsilonTransition MinTrans(int fromState) => new(fromState, int.MinValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EpsilonTransition MaxTrans(int fromState) => new(fromState, int.MaxValue);
}

