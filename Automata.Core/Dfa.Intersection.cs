using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Core.TransitionSets;

namespace Automata.Core;
public static class DfaIntersection
{
    public static Dfa Intersection(this Dfa a, Dfa b)
    {
        Dfa dfa = new();

        Queue<long> stateQueue = new();
        Dictionary<long, int> longStateToDfaState = new();

        long fromStateLong = Merge(a.InitialState, b.InitialState);
        stateQueue.Enqueue(fromStateLong);

        int maxState = 0; //the initial state of the DFA
        longStateToDfaState[fromStateLong] = maxState; //add the initial state (0) to the map
        dfa.SetInitial(maxState); //set as the initial state of the DFA

        while (stateQueue.Count > 0)
        {
            fromStateLong = stateQueue.Dequeue();
            int fromState = longStateToDfaState[fromStateLong];
            (int qA, int qB) = Split(fromStateLong);
            if (a.IsFinal(qA) && b.IsFinal(qB))
                dfa.SetFinal(fromState);

            SortedSet<SymbolicTransition> transitionsA = a.Transitions(qA);
            SortedSet<SymbolicTransition> transitionsB = b.Transitions(qB);
            foreach (SymbolicTransition tA in transitionsA)
            {
                string symbolAsString = a.Alphabet[tA.Symbol]; //get the symbol as a string, since we deal with different alphabets
                int symB = b.Alphabet[symbolAsString]; //get the symbol in B's alphabet, if exists
                if (symB == Constants.InvalidSymbolIndex)
                    continue; //symbol is not in B's alphabet so skip it
                SymbolicTransition tB = transitionsB.Transition(qB, symB);
                if (tB.IsInvalid)
                    continue; //no corresponding transition in B, so skip it

                long toStateLong = Merge(tA.ToState, tB.ToState);
                if (!longStateToDfaState.TryGetValue(toStateLong, out int toState))
                {
                    toState = ++maxState; //create a new state
                    longStateToDfaState[toStateLong] = toState; //add the new state to the map
                    stateQueue.Enqueue(toStateLong); //enqueue the new state for processing
                }
                int symbol = dfa.Alphabet.GetOrAdd(symbolAsString);
                dfa.Add(new SymbolicTransition(fromState, symbol, toState));
            }
        }
        return dfa;
    }

    /// <summary>
    /// Merges two signed integers into a single signed 64-bit value.
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
