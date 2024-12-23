using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core;

/// <summary>
/// Represents a set of epsilon transitions.
/// </summary>
public class EpsilonTransitionSet : TransitionSet<EpsilonTransition>
{
    public EpsilonTransitionSet(IEnumerable<EpsilonTransition> initialTransitions) : base(initialTransitions) { }

    /// <summary>
    /// Returns the states reachable from the given state on a single epsilon transition.
    /// If the input state has an epsilon loop on itself, it will be included in the result.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The states reachable from the given state on a single epsilon transition.</returns>
    public IEnumerable<int> ReachableStatesOnSingleEpsilon(int fromState)
        => orderDefault.GetViewBetween(new EpsilonTransition(fromState, int.MinValue), new EpsilonTransition(fromState, int.MaxValue)).Select(t => t.ToState);

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
            {
                if (fromStates.Add(newState))
                    queue.Enqueue(newState);
            }
        }
    }
}

