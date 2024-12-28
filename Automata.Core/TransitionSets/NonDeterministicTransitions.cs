﻿namespace Automata.Core.TransitionSets;

/// <summary>
/// Represents a mutable set of <see cref="Transition"/> for fast lookup and retrieval.
/// <para>This set allows non-deterministic transitions, but no duplicate transitions or epsilon transitions.</para>
/// </summary>
/// <remarks>Internally, this class maintains two ordered sets with the exact same set of transitions, 
/// but with different sort orders. One set is ordered so that all from-states are ordered are consecutive and increasing, 
/// <para>and the other set is ordered where all to-states are consecutive and increasing.</para>
/// <para>That enables fast retrieval of transitions either <c>from</c> or <c>to</c> a certain state, respectively.</para>
/// </remarks>
public class NonDeterministicTransitions : MutableTransitions<Transition>
{
    ///<inheritdoc/>
    public NonDeterministicTransitions() : base() { }

    ///<inheritdoc/>
    public NonDeterministicTransitions(IEnumerable<Transition> initialTransitions) : base(initialTransitions) { }

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The set of transitions from the given state.</returns>
    public SortedSet<Transition> Transitions(int fromState)
        => orderByFromState.Transitions(fromState);

    /// <summary>
    /// Returns the transitions from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The transitions from the given state on the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.</returns>
    public SortedSet<Transition> Transitions(int fromState, int symbol)
       => orderByFromState.Transitions(fromState, symbol);

    /// <summary>
    /// Returns the states reachable from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The states reachable from the given state on the given symbol.</returns>
    public IEnumerable<int> ReachableStates(int fromState, int symbol)
        => Transitions(fromState, symbol).Select(t => t.ToState);

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given states.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    public IntSet AvailableSymbols(IEnumerable<int> fromStates)
        => new(fromStates.SelectMany(s => Transitions(s)).Select(t => t.Symbol));



}
