namespace Automata.Core.Operations;

public static partial class Ops
{
    /// <summary>
    /// Concatenates two finite state automata.
    /// </summary>
    /// <param name="left">The left finite state automaton.</param>
    /// <param name="right">The right finite automaton.</param>
    /// <remarks>Creates a new automaton. For optimal performance, use <see cref="ConcatenationWith(Nfa, FsaDet)"/> when possible to reduce overhead.
    /// <para>The resulting alphabet will be the union of both alphabets, irrespective of whether all symbols were referenced by <paramref name="right"/>.</para>
    /// </remarks>
    /// <returns>A new deterministic finite automaton representing a concatenation of the two automata.</returns>
    public static FsaDet Concatenation(Fsa left, FsaDet right)
    {
        Nfa result = left.AsNfa(); //assert correct type
        result.ConcatenationWith(right); //append right to result in-place
        return result.AsDeterministic();
    }

    /// <summary>
    /// Mutating concatenation: Appends another automaton to the source automaton.
    /// </summary>
    /// <param name="source">Source automaton to append to.</param>
    /// <param name="right">Automaton to append.</param>
    /// <remarks>
    /// This operation mutates <paramref name="source"/>.
    /// <para>Resulting alphabet of <paramref name="source"/> will be the union of both alphabets, irrespective of whether all symbols were referenced by <paramref name="right"/>.</para>
    /// </remarks>
    /// <returns>Source automaton <paramref name="source"/></returns>
    public static Nfa ConcatenationWith(this Nfa source, FsaDet right)
    {
        if (ReferenceEquals(source, right))
            throw new ArgumentException("Operands must not be the same instance.");

        // Merge the alphabets and get symbol mappings
        Dictionary<int, int> symbolMapRightToSource = source.Alphabet.UnionWith(right.Alphabet);

        // Offset right's states to avoid conflicts
        int stateOffset = source.MaxState + 1;

        // Adjusted initial state of right
        int rightInitialStateInSource = right.InitialState + stateOffset;

        // Add right's transitions to source
        foreach (Transition t in right.Transitions())
            source.Add(new Transition(t.FromState + stateOffset, symbolMapRightToSource[t.Symbol], t.ToState + stateOffset));

        // Add right's epsilon-transitions to source (if any)
        foreach (EpsilonTransition t in right.EpsilonTransitions())
            source.Add(new EpsilonTransition(t.FromState + stateOffset, t.ToState + stateOffset));

        // Connect the two automata: Add epsilon-transitions from source's final states to right's initial state
        foreach (int finalState in source.FinalStates)
            source.Add(new EpsilonTransition(finalState, rightInitialStateInSource));

        // Clear source's final states and set right's final states
        source.ClearFinalStates();

        // Assign right's final states to source
        source.SetFinal(right.FinalStates.Select(s => s + stateOffset));

        return source;
    }
}
