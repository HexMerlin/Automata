using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Automata.Core;

/// <summary>
/// A fast read-only view of a state in an automaton, providing access to transitions from the state.
/// </summary>
/// <remarks>
/// This struct uses a ReadOnlySpan which is only a view on a contiguous memory sequence of <see cref="Transition"/>. 
/// </remarks>
public readonly ref struct StateView
{
    #region Data
    /// <summary>
    /// Transitions from the current state.
    /// </summary>
    public readonly ReadOnlySpan<Transition> Transitions;

    #endregion Data

    /// <summary>
    /// State from which the transitions originate.
    /// </summary>
    public int State => Transitions.Length > 0 ? Transitions[0].FromState : Constants.InvalidState;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateView"/> struct with a specified state and unfiltered transitions.
    /// </summary>
    /// <param name="fromState">State from which the transitions originate.</param>
    /// <param name="transitions">Filtered transitions from the specified state.</param>
    public StateView(int fromState, Transition[] transitions) : this(fromState, new ReadOnlySpan<Transition>(transitions)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateView"/> struct with a specified state and unfiltered transitions.
    /// </summary>
    /// <param name="fromState">State from which the transitions originate.</param>
    /// <param name="transitions">Filtered transitions from the specified state.</param>
    public StateView(int fromState, ReadOnlySpan<Transition> transitions)
    {
        int startIndex = transitions.BinarySearch(Core.Transition.MinTrans(fromState));
        Debug.Assert(startIndex < 0, $"Binary search returned a non-negative index ({startIndex}), which should be impossible given the search key.");
        startIndex = ~startIndex;
        if (startIndex >= transitions.Length || transitions[startIndex].FromState != fromState)
        {
            this.Transitions = ReadOnlySpan<Transition>.Empty; // If no matches exist, return an empty span
            return;
        }
        int endIndex = transitions.Slice(startIndex + 1).BinarySearch(Core.Transition.MaxTrans(fromState));
        Debug.Assert(endIndex < 0, $"Binary search returned a non-negative index ({endIndex}), which should be impossible given the search key.");
        endIndex = startIndex + 1 + (~endIndex);
        this.Transitions = transitions[startIndex..endIndex];
    }

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="symbol">Symbol for the transition.</param>
    /// <returns>
    /// The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.
    /// </returns>
    /// <seealso cref="TryTransition(int, out int)"/>
    public int Transition(int symbol)
    {
        int index = Transitions.BinarySearch(Core.Transition.MinTrans(State, symbol));
        Debug.Assert(index < 0, $"Binary search returned a non-negative index ({index}), which should be impossible given the search key.");
        index = ~index; // Get the insertion point
        return (index < Transitions.Length && Transitions[index].FromState == State && Transitions[index].Symbol == symbol)
            ? Transitions[index].ToState
            : Constants.InvalidState;
    }

    /// <summary>
    /// Tries to get the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <param name="symbol">Symbol for the transition.</param>
    /// <param name="toState">The reachable state, or <see cref="Constants.InvalidState"/> if the method returns false.</param>
    /// <returns><see langword="true"/> <c>iff</c> a reachable state exists.</returns>
    /// <seealso cref="Transition(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryTransition(int symbol, out int toState) => (toState = Transition(symbol)) != Constants.InvalidState;

}
