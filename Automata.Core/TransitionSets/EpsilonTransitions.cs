namespace Automata.Core.TransitionSets;

/// <summary>
/// Represents a mutable set of <see cref="EpsilonTransition"/> for fast lookup and retrieval.
/// </summary>
/// <remarks>Internally, this class maintains two ordered sets with the exact same set of transitions, 
/// but with different sort orders. One set is ordered so that all from-states are ordered are consecutive and increasing, 
/// <para>and the other set is ordered where all to-states are consecutive and increasing.</para>
/// <para>That enables fast retrieval of transitions either <c>from</c> or <c>to</c> a certain state, respectively.</para>
/// </remarks>
public class EpsilonTransitions : Transitions<EpsilonTransition>
{
    ///<inheritdoc/>
    public EpsilonTransitions() : base() { }

    ///<inheritdoc/>
    public EpsilonTransitions(IEnumerable<EpsilonTransition> initialTransitions) : base(initialTransitions) { }

    /// <summary>
    /// Returns the states reachable from the given state on a single epsilon transition.
    /// If the input state has an epsilon loop on itself, it will be included in the result.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The states reachable from the given state on a single epsilon transition.</returns>
    public IEnumerable<int> ReachableStatesOnSingleEpsilon(int fromState)
        => orderByFromState.GetViewBetween(new EpsilonTransition(fromState, int.MinValue), new EpsilonTransition(fromState, int.MaxValue)).Select(t => t.ToState);

    /// <summary>
    /// Extends the set of states with their epsilon closure in place.
    /// </summary>
    /// <remarks>Epsilon closure is all reachable states on epsilon transitions</remarks>
    /// <param name="fromStates">The set of states to extend.</param>
    public void ReachableStatesOnEpsilonInPlace(HashSet<int> fromStates)
    {
        var queue = new Queue<int>(fromStates);

        while (queue.Count != 0)
        {
            int state = queue.Dequeue();
            IEnumerable<int> newStates = ReachableStatesOnSingleEpsilon(state);
            foreach (var newState in newStates)
                if (fromStates.Add(newState))
                    queue.Enqueue(newState);
        }
    }
}

