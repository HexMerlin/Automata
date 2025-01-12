namespace Automata.Core.Operations;

public static partial class Ops
{
    /// <summary>
    /// Concatenates two finite state automata.
    /// </summary>
    /// <param name="left">The left finite state automaton.</param>
    /// <param name="right">The right finite automaton.</param>
    /// <remarks>Creates a new automaton. For optimal performance, use <see cref="Append(Nfa, IDfa)"/> when possible to reduce overhead.
    /// <para>Resulting alphabet of <paramref name="source"/> will be the union of both alphabets, irrespective of whether all symbols were referenced by <paramref name="right"/>.</para>
    /// </remarks>
    /// <returns>A new deterministic finite automaton representing a concatenation of the two automata.</returns>
    public static IDfa Concatenation(IFsa left, IDfa right)
    {
        Nfa result = left.AsNfa(enforceNew: true); //Initialize result to clone of left
        result.Append(right); //append right to result in-place
        return result.AsIDfa();
    }

    /// <summary>
    /// Appends another automaton to the source automaton (in-place concatenation).
    /// </summary>
    /// <param name="source">Source automaton to append to.</param>
    /// <param name="right">Automaton to append.</param>
    /// <remarks>
    /// This operation mutates <paramref name="source"/>.
    /// <para>Resulting alphabet of <paramref name="source"/> will be the union of both alphabets, irrespective of whether all symbols were referenced by <paramref name="right"/>.</para>
    /// </remarks>
    public static void Append(this Nfa source, IDfa right)
    {
        // Merge the alphabets and get symbol mappings
        Dictionary<int, int> symbolMapRightToSource = source.Alphabet.UnionWith(right.Alphabet);

        // Check if right is empty
        if (right.IsEmpty)
            return;

        // Offset right's states to avoid conflicts
        int stateOffset = source.MaxState + 1;

        // Adjusted initial state of right
        int rightInitialStateInSource = right.InitialState + stateOffset;

        // Add right's transitions to source
        foreach (Transition t in right.SymbolicTransitions())
            source.Add(new Transition(t.FromState + stateOffset, symbolMapRightToSource[t.Symbol], t.ToState + stateOffset));

        // Add right's epsilon-transitions to source (if any)
        foreach (EpsilonTransition t in right.EpsilonTransitions())
            source.Add(new EpsilonTransition(t.FromState + stateOffset, t.ToState + stateOffset));

        // Add epsilon-transitions from source's final states to right's initial state
        foreach (int finalState in source.FinalStates)
            source.Add(new EpsilonTransition(finalState, rightInitialStateInSource));

        // Clear source's final states and set right's final states
        source.ClearFinalStates();

        // Assign right's final states to source
        source.SetFinal(right.FinalStates.Select(s => s + stateOffset));
    }
}
