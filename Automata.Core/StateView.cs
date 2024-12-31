using System.Collections;
using System.Diagnostics;

namespace Automata.Core;
public readonly ref struct StateView 
{
    public readonly ReadOnlySpan<Transition> Transitions;

    public int State => Transitions.Length > 0 ? Transitions[0].FromState : Constants.InvalidState;

    public StateView(int fromState, Transition[] transitions) : this(fromState, new ReadOnlySpan<Transition>(transitions)) { }
 
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
