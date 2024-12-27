namespace Automata.Core.TransitionSets;

/// <summary>
/// Represents a mutable set of <see cref="SymbolicTransition"/> for fast lookup and retrieval.
/// <para>This class enforces deterministic transitions, and allows no non-deterministic, duplicate or epsilon transitions.</para>
/// </summary>
/// <remarks>Internally, this class maintains two ordered sets with the exact same set of transitions, 
/// but with different sort orders. One set is ordered so that all from-states are ordered are consecutive and increasing, 
/// <para>and the other set is ordered where all to-states are consecutive and increasing.</para>
/// <para>That enables fast retrieval of transitions either <c>from</c> or <c>to</c> a certain state, respectively.</para>
/// </remarks>
public class DeterministicTransitions : Transitions<SymbolicTransition>
{
    ///<inheritdoc/>
    public DeterministicTransitions() : base() { }

    ///<inheritdoc/>
    public DeterministicTransitions(IEnumerable<SymbolicTransition> initialTransitions) : base(initialTransitions) { }

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
       => orderByFromState.Transition(fromState, symbol);

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The set of transitions from the given state.</returns>
    public SortedSet<SymbolicTransition> Transitions(int fromState)
        => orderByFromState.Transitions(fromState);

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.</returns>
    public int ReachableState(int fromState, int symbol)
        => orderByFromState.Transition(fromState, symbol).ToState;

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <remarks>Since the underlying set is deterministic, the returned symbols is a set, meaning every symbol can occur only once.</remarks>
    /// <returns>The set of symbols that can be used to transition directly from the given state.</returns>
    public IEnumerable<int> AvailableSymbols(int fromState)
        => orderByFromState.AvailableSymbols(fromState);

    /// <summary>
    /// Returns a set of symbols that can be used to transition directly from the given states.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    public IntSet AvailableSymbols(IEnumerable<int> fromStates)
        => new(fromStates.SelectMany(AvailableSymbols));

}

