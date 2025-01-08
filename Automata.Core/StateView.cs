using System.Collections;
using System.Diagnostics;

namespace Automata.Core;

/// <summary>
/// Represents a read-only view of a state in an automaton, providing access to transitions from the state.
/// </summary>
/// <remarks>
/// This struct uses a ReadOnlySpan which is only a view on a contiguous memory sequence of <see cref="Transition"/>. 
/// </remarks>
public readonly ref struct StateView : IState
{
    /// <summary>
    /// Gets the transitions from the current state.
    /// </summary>
    public readonly ReadOnlySpan<Transition> Transitions;

    /// <summary>
    /// Gets the state from which the transitions originate.
    /// </summary>
    public int State => Transitions.Length > 0 ? Transitions[0].FromState : Constants.InvalidState;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateView"/> struct with an specified state and unfiltered transitions.
    /// </summary>
    /// <param name="fromState">The state from which the transitions originate.</param>
    /// <param name="transitions">The filtered transitions from the specified state.</param>
    public StateView(int fromState, Transition[] transitions) : this(fromState, new ReadOnlySpan<Transition>(transitions)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateView"/> struct with an specified state and unfiltered transitions.
    /// </summary>
    /// <param name="fromState">The state from which the transitions originate.</param>
    /// <param name="transitions">The filtered transitions from the specified state.</param>
    public StateView(int fromState, ReadOnlySpan<Transition> transitions)
    {
        int startIndex = transitions.BinarySearch(new Transition(fromState, Constants.InvalidSymbolIndex, Constants.InvalidState));
        Debug.Assert(startIndex < 0, $"Binary search returned a non-negative index ({startIndex}), which should be impossible given the search key.");
        startIndex = ~startIndex;
        if (startIndex >= transitions.Length || transitions[startIndex].FromState != fromState)
        {
            this.Transitions = ReadOnlySpan<Transition>.Empty; // If no matches exist, return an empty span
            return;
        }
        int endIndex = transitions.Slice(startIndex + 1).BinarySearch(new Transition(fromState + 1, Constants.InvalidSymbolIndex, Constants.InvalidState));
        Debug.Assert(endIndex < 0, $"Binary search returned a non-negative index ({endIndex}), which should be impossible given the search key.");
        endIndex = ~endIndex;
        this.Transitions = transitions[startIndex..endIndex];
    }

    /// <summary>
    /// Gets the transition for the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol for which to get the transition.</param>
    /// <returns>The transition for the specified symbol, or <see cref="Transition.Invalid"/> if no such transition exists.</returns>
    public Transition Transition(int symbol)
    {
        int index = Transitions.BinarySearch(new Transition(State, symbol, Constants.InvalidState));
        Debug.Assert(index < 0, $"Binary search returned a non-negative index ({index}), which should be impossible given the search key.");
        index = ~index; // Get the insertion point
        return (index < Transitions.Length && Transitions[index].FromState == State && Transitions[index].Symbol == symbol)
            ? Transitions[index]
            : Core.Transition.Invalid;
    }

}
