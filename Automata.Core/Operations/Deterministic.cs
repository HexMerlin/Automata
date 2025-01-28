namespace Automata.Core.Operations;
public static partial class Ops
{
    /// <summary>
    /// Determinization of an NFA representation to a DFA.
    /// </summary>
    /// <remarks>Uses the Powerset Construction algorithm (a.k.a. Subset Construction algorithm).
    /// </remarks>
    /// <param name="nfa">The input nondeterministic finite automaton.</param>
    /// <returns>A new DFA equivalent to the NFA.</returns>
    internal static Dfa Deterministic(Nfa nfa)
    {
        List<Transition> dfaTransitions = [];
        HashSet<int> dfaFinalStates = [];

        int dfaMaxState = Constants.InvalidState;
        Dictionary<IntSet, int> stateSetToDfaState = [];
        Queue<IntSet> queue = new();

        HashSet<int> initialStates = [.. nfa.InitialStates];
        nfa.ReachableStatesOnEpsilonInPlace(initialStates);
        int dfaInitialState = GetOrAddState(new IntSet(initialStates)); //add initial NFA states to dfa as a single state

        while (queue.Count > 0)
        {
            IntSet fromState = queue.Dequeue();
            IntSet symbols = nfa.AvailableSymbols(fromState);
            int dfaFromState = GetOrAddState(fromState);

            foreach (int symbol in symbols)
            {
                IntSet toState = nfa.ReachableStates(fromState, symbol);
                int dfaToState = GetOrAddState(toState);
                dfaTransitions.Add(new Transition(dfaFromState, symbol, dfaToState));
            }
        }
        return new Dfa(nfa.Alphabet, dfaInitialState, dfaFinalStates, dfaTransitions);

        int GetOrAddState(IntSet combinedState)
        {
            if (!stateSetToDfaState.TryGetValue(combinedState, out int dfaState))
            {
                dfaState = ++dfaMaxState; //create a new state in DFA
                stateSetToDfaState[combinedState] = dfaState;
                queue.Enqueue(combinedState);
            }
            if (combinedState.Overlaps(nfa.FinalStates))
                dfaFinalStates.Add(dfaState);
            return dfaState;
        }
    }
}
