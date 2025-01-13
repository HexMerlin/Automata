namespace Automata.Core.Operations;

/// <summary>
/// Provides operations for finite automata.
/// </summary>
public static partial class Ops
{
    /// <summary>
    /// Complement of a given MFA.
    /// </summary>
    /// <param name="mfa">A <see cref="Mfa"/> to complement.</param>
    /// <returns>A new deterministic finite automaton (DFA) representing the complement of the input MFA.</returns>
    public static Dfa Complement(Mfa mfa)
    {
        Alphabet alphabet = new(mfa.Alphabet);

        int initialState = mfa.InitialState;

        // Notice: The line below is also adding a new final 'trap state' to the DFA, hence the +1.
        var finalStates = Enumerable.Range(0, mfa.StateCount + 1).Except(mfa.FinalStates);
        int trapState = mfa.StateCount;

        Dfa dfa = new Dfa(alphabet, mfa.Transitions(), initialState, finalStates);

        // Iterate over all states in DFA (including the trap state) and add transitions to the trap state for all missing symbols in the alphabet.
        // Consequently, the new trap state will have a self-loop for all symbols in the alphabet.
        for (int state = 0; state < mfa.StateCount + 1; state++)
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
