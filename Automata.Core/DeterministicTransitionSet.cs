using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core;

/// <summary>
/// Represents a mutable set of <see cref="SymbolicTransition"/> for fast lookup and retrieval.
/// <para>This class enforces deterministic transitions, and allows no non-deterministic, duplicate or epsilon transitions.</para>
/// </summary>
/// <remarks>Internally, this class maintains two ordered sets with the exact same set of transitions, 
/// but with different sort orders. One set is ordered so that all from-states are ordered are consecutive and increasing, 
/// <para>and the other set is ordered where all to-states are consecutive and increasing.</para>
/// <para>That enables fast retrieval of transitions either <c>from</c> or <c>to</c> a certain state, respectively.</para>
/// </remarks>
public class DeterministicTransitionSet : TransitionSet<SymbolicTransition>
{
    ///<inheritdoc/>
    public DeterministicTransitionSet() : base() { }

    ///<inheritdoc/>
    public DeterministicTransitionSet(IEnumerable<SymbolicTransition> initialTransitions) : base(initialTransitions) { }

    /// <summary>
    /// Adds a transition to the DFA.
    /// </summary>
    /// <remarks>If a transition with the same from-state and the same symbol already exists, that transition will be replaced.</remarks>
    /// <param name="transition">The transition to add.</param>
    public override void Add(SymbolicTransition transition)
    {
        int existingToState = ReachableState(transition.FromState, transition.Symbol);
        if (existingToState == transition.ToState)
            return; //the same transition already exists, we are done
        if (existingToState != Constants.InvalidState)
            Remove(transition); //remove the existing transition
        base.Add(transition);
    }

    /// <summary>
    /// Adds all provided transitions that are currently not present in set.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public override void UnionWith(IEnumerable<SymbolicTransition> transitions)
    {
        foreach (SymbolicTransition transition in transitions)
            Add(transition);
    }

    /// <summary>
    /// Returns the transition from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The transition from the given state on the given symbol, or <see cref="SymbolicTransition.Invalid"/> if no such transition exists.</returns>
    public SymbolicTransition Transition(int fromState, int symbol)
       => orderByFromState.GetViewBetween(MinTrans(fromState, symbol), MaxTrans(fromState, symbol)).FirstOrDefault(SymbolicTransition.Invalid);

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The set of transitions from the given state.</returns>
    public SortedSet<SymbolicTransition> Transitions(int fromState)
        => orderByFromState.GetViewBetween(MinTrans(fromState), MaxTrans(fromState));

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.</returns>
    public int ReachableState(int fromState, int symbol)
        => Transition(fromState, symbol).ToState;

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given state.</returns>
    public IEnumerable<int> AvailableSymbols(int fromState)
        => orderByFromState.GetViewBetween(MinTrans(fromState), MaxTrans(fromState)).Select(t => t.Symbol);

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given states.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    public IntSet AvailableSymbols(IEnumerable<int> fromStates)
        => new(fromStates.SelectMany(s => orderByFromState.GetViewBetween(MinTrans(s), MaxTrans(s)).Select(t => t.Symbol)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SymbolicTransition MinTrans(int fromState, int symbol = int.MinValue) => new(fromState, symbol, int.MinValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SymbolicTransition MaxTrans(int fromState, int symbol = int.MaxValue) => new(fromState, symbol, int.MaxValue);

}

