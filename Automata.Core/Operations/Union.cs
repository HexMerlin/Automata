namespace Automata.Core.Operations;
public static partial class Ops
{
    /// <summary>
    /// Creates union of two finite state automata.
    /// </summary>
    /// <param name="left">First automaton.</param>
    /// <param name="other">Second automaton.</param>
    /// <remarks>Creates a new automaton. For optimal performance, use <see cref="UnionWith(Nfa, IDfa)"/> when possible to reduce overhead.
    /// <para>Resulting alphabet of <paramref name="source"/> will be the union of both alphabets, irrespective of whether all symbols were referenced by <paramref name="right"/>.</para>
    /// </remarks>
    /// <returns>A new deterministic finite automaton representing a union of the two automata.</returns>
    public static IDfa Union(IFsa left, IDfa other)
    {
        Nfa result = left.AsNfa(enforceNew: true); //Initialize result to clone of other
        result.UnionWith(other); //union with other in-place
        return result.AsIDfa();
    }

    /// <summary>
    /// Unites the source automaton with another automaton (in-place union).
    /// </summary>
    /// <param name="source">Source automaton to mutate.</param>
    /// <param name="other">Automaton to union with.</param>
    /// <remarks>
    /// This operation mutates <paramref name="source"/> to represent the union of the two automata.
    /// <para>Resulting alphabet of <paramref name="source"/> will be the union of both alphabets, irrespective of whether all symbols were referenced by <paramref name="right"/>.</para>
    /// </remarks>
    public static void UnionWith(this Nfa source, IDfa other)
    {
        // Merge the alphabets and get symbol mappings
        Dictionary<int, int> symbolMapRightToSource = source.Alphabet.UnionWith(other.Alphabet);

        // Check if right is empty
        if (other.IsEmpty)
            return;

        // Offset other's states to avoid conflicts
        int stateOffset = source.MaxState + 1;

        // Add other's transitions to source
        foreach (Transition t in other.SymbolicTransitions())
            source.Add(new Transition(t.FromState + stateOffset, symbolMapRightToSource[t.Symbol], t.ToState + stateOffset));

        // Add other's epsilon-transitions to source (if any)
        foreach (EpsilonTransition t in other.EpsilonTransitions())
            source.Add(new EpsilonTransition(t.FromState + stateOffset, t.ToState + stateOffset));

        // Include other's initial state
        source.SetInitial(other.InitialState + stateOffset);

        // Include other's final states
        source.SetFinal(other.FinalStates.Select(s => s + stateOffset));
    }
}
