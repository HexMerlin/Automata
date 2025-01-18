namespace Automata.Core.Operations;

public static partial class Ops
{
    /// <summary>
    /// Intersection of two deterministic finite automata.
    /// The resulting automaton accepts only the strings that are accepted by both input automata.
    /// </summary>
    /// <param name="a">The first finite automaton.</param>
    /// <param name="b">The second finite automaton.</param>
    /// <returns>
    /// An new <see cref="Mfa"/> representing the intersection of the two input automata.
    /// </returns>
    public static Mfa Intersection(this FsaDet a, FsaDet b)
    {
        if (ReferenceEquals(a, b))
            throw new ArgumentException("Operands must not be the same instance.");
        if (!a.HasInitialState || ! b.HasInitialState)
            return Mfa.CreateEmpty(a.Alphabet); // Return an empty DFA if either automaton is empty

        Dfa dfa = new(a.Alphabet);

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

            var stateATrans = stateA.Transitions;

            foreach (Transition tA in stateA.Transitions)
            {
                // Indexes for the transition symbol can be different in a's and b's alphabets. 
                // To handle, we extract the symbol as a string, and use that for cross-alphabet lookup.
                string symbolAsString = a.Alphabet[tA.Symbol];

                // Get the symbol in B's alphabet, if it exists
                if (!b.Alphabet.TryGetIndex(symbolAsString, out int symB))
                    continue; // Symbol is not in B's alphabet, so skip it

                if (! stateB.TryTransition(symB, out int bToState))
                    continue; // No corresponding transition in B, so skip it

                long toStateLong = Merge(tA.ToState, bToState);
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
        return dfa.AsMfa();
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
