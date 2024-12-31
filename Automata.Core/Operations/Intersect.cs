namespace Automata.Core.Operations;
/// <summary>
/// Provides static methods for performing operations on finite automata, such as computing the intersection of two automata.
/// </summary>
public static class Intersect
{
    /// <summary>
    /// Computes the intersection of two canonical finite automata (CFAs).
    /// The resulting automaton accepts only the strings that are accepted by both input automata.
    /// </summary>
    /// <param name="a">The first canonical finite automaton.</param>
    /// <param name="b">The second canonical finite automaton.</param>
    /// <returns>
    /// A deterministic finite automaton representing the intersection of the two input automata.
    /// </returns>
    public static Dfa Intersection(Cfa a, Cfa b)
    {
        Dfa dfa = new();

        Queue<long> stateQueue = new();
        Dictionary<long, int> longStateToDfaState = new();

        long fromStateLong = Merge(a.InitialState, b.InitialState);
        stateQueue.Enqueue(fromStateLong);

        int maxState = 0; // The initial state of the DFA
        longStateToDfaState[fromStateLong] = maxState; // Add the initial state (0) to the map
        dfa.SetInitial(maxState); // Set as the initial state of the DFA

        while (stateQueue.Count > 0)
        {
            fromStateLong = stateQueue.Dequeue();
            int fromState = longStateToDfaState[fromStateLong];
            (int qA, int qB) = Split(fromStateLong);

            // If both states are final in their respective automata, mark the combined state as final
            if (a.IsFinal(qA) && b.IsFinal(qB))
                dfa.SetFinal(fromState);

            StateView stateA = a.State(qA);
            StateView stateB = b.State(qB);
            foreach (Transition tA in stateA.Transitions)
            {
                // Get the symbol as a string, since we deal with different alphabets
                string symbolAsString = a.Alphabet[tA.Symbol];

                // Get the symbol in B's alphabet, if it exists
                int symB = b.Alphabet[symbolAsString];
                if (symB == Constants.InvalidSymbolIndex)
                    continue; // Symbol is not in B's alphabet, so skip it

                Transition tB = stateB.Transition(symB);
                if (tB.IsInvalid)
                    continue; // No corresponding transition in B, so skip it

                long toStateLong = Merge(tA.ToState, tB.ToState);
                if (!longStateToDfaState.TryGetValue(toStateLong, out int toState))
                {
                    toState = ++maxState; // Create a new state
                    longStateToDfaState[toStateLong] = toState; // Add the new state to the map
                    stateQueue.Enqueue(toStateLong); // Enqueue the new state for processing
                }
                int symbol = dfa.Alphabet.GetOrAdd(symbolAsString);
                dfa.Add(new Transition(fromState, symbol, toState));
            }
        }
        return dfa;
    }

    /// <summary>
    /// Merges two signed 32-bit integers into a single signed 64-bit value.
    /// </summary>
    /// <param name="a">The first signed 32-bit integer.</param>
    /// <param name="b">The second signed 32-bit integer.</param>
    /// <returns>A signed 64-bit value representing the merged integers.</returns>
    private static long Merge(int a, int b) => ((long)a << 32) | ((long)b & 0xFFFFFFFFL);

    /// <summary>
    /// Splits a signed 64-bit value into two signed 32-bit integers.
    /// </summary>
    /// <param name="value">The signed 64-bit value to split.</param>
    /// <returns>A tuple containing the two signed 32-bit integers.</returns>
    private static (int, int) Split(long value) => ((int)(value >> 32), (int)(value & 0xFFFFFFFFL));
}
