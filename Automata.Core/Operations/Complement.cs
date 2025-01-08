namespace Automata.Core.Operations;

/// <summary>
/// Provides operations for finite automata.
/// </summary>
public static partial class Ops
{
    /// <summary>
    /// Computes the complement of a given canonical finite automaton (CFA).
    /// </summary>
    /// <param name="cfa">The canonical finite automaton to complement.</param>
    /// <returns>A new deterministic finite automaton (DFA) representing the complement of the input CFA.</returns>
    public static Dfa Complement(Cfa cfa)
    {
        MutableAlphabet alphabet = new(cfa.Alphabet);

        int initialState = cfa.InitialState;

        // Notice: The line below is also adding a new final 'trap state' to the DFA, hence the +1.
        var finalStates = Enumerable.Range(0, cfa.StateCount + 1).Except(cfa.FinalStates);
        int trapState = cfa.StateCount;

        Dfa dfa = new Dfa(alphabet, cfa, initialState, finalStates);

        // Iterate over all states in DFA (including the trap state) and add transitions to the trap state for all missing symbols in the alphabet.
        // Consequently, the new trap state will have a self-loop for all symbols in the alphabet.
        for (int state = 0; state < cfa.StateCount + 1; state++)
        {
            for (int symbol = 0; symbol < alphabet.Count; symbol++)
            {
                Transition transition = new Transition(state, symbol, trapState);
                _ = dfa.Add(transition);
            }
        }
        return dfa;
    }
}
