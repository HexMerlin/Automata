using System.Collections;
using System.Diagnostics;

namespace Automata.Core.TransitionSets;

/// <summary>
/// A highly optimized immutable set of deterministic transitions.
/// </summary>
/// <remarks>
/// <see cref="ImmutableTransitions"/> is characterized by:
/// <list type="bullet">
/// <item>
/// <description>
/// <b>Immutable:</b> Structural and behavioral invariance, ensuring thread safety and predictable behavior.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Performance:</b> Optimized for fast, read-only operations with minimal memory overhead.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Ordering:</b> Transitions are sorted according to the default order of <see cref="Core.Transition"/>.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Deterministic:</b> The set cannot contain more than one transition with a specific (FromState, Symbol) pair.
/// </description>
/// </item>
/// </list>
/// </remarks>
public class ImmutableTransitions : IEnumerable<Transition>
{
    private readonly Transition[] transitions;

    public readonly int StateCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableTransitions"/> class, ensuring the input is sorted and deterministic.
    /// </summary>
    /// <param name="transitions">The input set of transitions.</param>
    /// <exception cref="ArgumentException">Thrown if the transitions are not deterministic.</exception>
    public ImmutableTransitions(IEnumerable<Transition> transitions)
    { 
        this.transitions = transitions.OrderBy(t => t).ToArray();
        this.StateCount = 1 + MaxStateAndAssert(this.transitions);
    }

    /// <returns>The number of transitions in the set.</returns>
    public int TransitionCount => transitions.Length;

    /// <summary>
    /// Returns the transition from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The source state.</param>
    /// <param name="symbol">The symbol of the transition.</param>
    /// <returns>
    /// The transition from the given state with the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.
    /// </returns>
    public Transition Transition(int fromState, int symbol)
    {
        var searchKey = new Transition(fromState, symbol, Constants.InvalidState);

        int index = Array.BinarySearch(transitions, searchKey);

        Debug.Assert(index < 0, $"Binary search returned a non-negative index ({index}), which should be impossible given the search key.");
        index = ~index; // Get the insertion point

        return (index < transitions.Length && transitions[index].FromState == fromState && transitions[index].Symbol == symbol)
            ? transitions[index]
            : Core.Transition.Invalid;
    }

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The source state.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> containing the transitions from the given state.</returns>
    public ReadOnlySpan<Transition> Transitions(int fromState)
    {
        // Define the search key for the start of the range
        var startKey = new Transition(fromState, Constants.InvalidSymbolIndex, Constants.InvalidState);

        // Find the start of the range
        int startIndex = Array.BinarySearch(transitions, startKey);
        startIndex = startIndex < 0 ? ~startIndex : startIndex;

        // If no matches exist, return an empty span
        if (startIndex >= transitions.Length || transitions[startIndex].FromState != fromState)
        {
            return ReadOnlySpan<Transition>.Empty;
        }

        // Optimize the second search by starting at startIndex + 1
        var endKey = new Transition(fromState + 1, Constants.InvalidSymbolIndex, Constants.InvalidState);
        int endIndex = Array.BinarySearch(transitions, startIndex + 1, transitions.Length - startIndex - 1, endKey);
        endIndex = endIndex < 0 ? ~endIndex : endIndex;

        // Return the span over the calculated range
        return transitions.AsSpan(startIndex, endIndex - startIndex);
    }

    /// <summary>
    /// Finds the maximum state in the set of transitions.
    /// Also asserts that the transitions are deterministic.
    /// </summary>
    /// <param name="transitions">The transition array</param>
    /// <returns>The maximum state referenced, or <see cref="Constants.InvalidState"/> if the array is empty.</returns>
    /// <exception cref="ArgumentException">If the transitions are not deterministic.</exception>
    private static int MaxStateAndAssert(Transition[] transitions)
    {
        int maxState = Constants.InvalidState;
        int fromState = Constants.InvalidState;
        int symbol = Constants.InvalidSymbolIndex;
     
        for (int i = 0; i < transitions.Length; i++)
        { 
            Transition t = transitions[i];
            if (t.FromState == fromState && t.Symbol == symbol)
                throw new ArgumentException("The transitions must be deterministic: every (FromState, Symbol)-tuple must be unique.");
            (fromState, symbol, int toState) = t;
            maxState = Math.Max(maxState, Math.Max(fromState, toState));
        }
        return maxState;
    }

    public IEnumerator<Transition> GetEnumerator() => ((IEnumerable<Transition>)transitions).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    //    => orderByFromState.Transitions(fromState);

    ///// <summary>
    ///// Returns the state reachable from the given state on the given symbol.
    ///// </summary>
    ///// <param name="fromState">The state from which to start.</param>
    ///// <param name="symbol">The symbol to transition on.</param>
    ///// <returns>The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.</returns>
    //public int ReachableState(int fromState, int symbol)
    //    => orderByFromState.Transition(fromState, symbol).ToState;

    ///// <summary>
    ///// Returns the set of symbols that can be used to transition directly from the given state.
    ///// </summary>
    ///// <param name="fromState">The state from which to start.</param>
    ///// <remarks>Since the underlying set is deterministic, the returned symbols is a set, meaning every symbol can occur only once.</remarks>
    ///// <returns>The set of symbols that can be used to transition directly from the given state.</returns>
    //public IEnumerable<int> AvailableSymbols(int fromState)
    //    => orderByFromState.AvailableSymbols(fromState);

    ///// <summary>
    ///// Returns a set of symbols that can be used to transition directly from the given states.
    ///// </summary>
    ///// <param name="fromStates">The states from which to start.</param>
    ///// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    //public IntSet AvailableSymbols(IEnumerable<int> fromStates)
    //    => new(fromStates.SelectMany(AvailableSymbols));


}
