namespace Automata.Core;

/// <summary>
/// Provides a reverse lookup for a DFA.
/// <para>Specifically:</para>
/// <para>- Get the set of states that can transition to a given state with a given symbol.</para>
/// <para>- Get the set of symbols that can transition to a given state.</para>
/// </summary>
public class DfaReverseLookup
{
    /// <summary>
    /// Dictionary maps a pair of (toState, symbol) to an array of unique ordered set of from-states.
    /// </summary>
    private readonly Dictionary<(int, int), int[]> toStateSymbol_FromStates;

    /// <summary>
    /// Dictionary maps a toState to an array of unique ordered set of symbols.
    /// </summary>
    private readonly Dictionary<int, int[]> toState_Symbols;

    /// <summary>
    /// All states in the DFA after trimming.
    /// </summary>
    public ISet<int> AllStates { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DfaReverseLookup"/> class using the specified DFA.
    /// </summary>
    /// <remarks>Initially, this method perform a trim operation on the DFA that removes states that are not both accessible and co-accessible.</remarks>
    /// <param name="dfa">The DFA to initialize the reverse lookup from.</param>
    public DfaReverseLookup(Dfa dfa)
    {
        (AllStates, Transition[] reverseTransitions) = dfa.Trim();
      
        if (reverseTransitions.Length == 0)
            return;
      
        this.toStateSymbol_FromStates = new Dictionary<(int, int), int[]>(reverseTransitions.Length);
        this.toState_Symbols = new Dictionary<int, int[]>(reverseTransitions.Length);

        int start = 0;
        (int toState, int symbol) = (reverseTransitions[0].ToState, reverseTransitions[0].Symbol);

        var symbolsList = new List<int> { symbol }; // Track symbols for the current toState

        for (int i = 1; i < reverseTransitions.Length; i++)
        {
            var transition = reverseTransitions[i];

            if (transition.ToState != toState) // New toState encountered
            {
                toState_Symbols[toState] = symbolsList.ToArray(); // Store previous toState's symbols
                symbolsList.Clear(); // Reset and add first symbol of new toState
                symbolsList.Add(transition.Symbol);
            }
            else if (transition.Symbol != symbol) // Only add symbol when it's unique
            {
                symbolsList.Add(transition.Symbol);
            }

            if (transition.ToState != toState || transition.Symbol != symbol) // New (toState, symbol) pair
            {
                toStateSymbol_FromStates[(toState, symbol)] = ExtractFromStates(reverseTransitions, start, i);
                start = i;
            }

            (toState, symbol) = (transition.ToState, transition.Symbol);
        }

        // Store last segments
        toStateSymbol_FromStates[(toState, symbol)] = ExtractFromStates(reverseTransitions, start, reverseTransitions.Length);
        toState_Symbols[toState] = symbolsList.ToArray();
    }

    /// <summary>
    /// Gets the array of symbols for the specified to-state.
    /// The returned symbols are unique and ordered.
    /// </summary>
    /// <param name="toState">The destination state.</param>
    /// <returns>Array of symbols.</returns>
    public int[] Symbols(int toState) => toState_Symbols.TryGetValue(toState, out int[]? symbols) ? symbols : [];

    /// <summary>
    /// The set of states that can transition to the specified state with the specified symbol.
    /// </summary>
    /// <param name="toState">The destination state.</param>
    /// <param name="symbol">The transition symbol.</param>
    /// <returns>Set of from-states.</returns>
    public IntSet FromStates(int toState, int symbol)
    {
        if (toStateSymbol_FromStates.TryGetValue((toState, symbol), out int[]? fromStateArray))
            return new IntSet(fromStateArray);
        return new IntSet([]);
    }

    /// <summary>
    /// The set of states that can transition to any of the specified to-states with the specified symbol.
    /// </summary>
    /// <param name="toStates">Set of destination states.</param>
    /// <param name="symbol">The transition symbol.</param>
    /// <returns>Set of from-states.</returns>
    public IntSet FromStates(IntSet toStates, int symbol)
    {
        HashSet<int> fromStates = new HashSet<int>();
        foreach (int toState in toStates)
            if (toStateSymbol_FromStates.TryGetValue((toState, symbol), out int[]? fromStateArray))
                fromStates.UnionWith(fromStateArray);
        return new IntSet(fromStates);
    }

    /// <summary>
    /// Adds the symbols for the specified to-states to the provided set of symbols.
    /// </summary>
    /// <param name="toStates">Set of destination states.</param>
    /// <param name="symbols">Set of symbols to add to.</param>
    public void AddSymbols(IntSet toStates, HashSet<int> symbols)
    {
        foreach (int toState in toStates)
            if (toState_Symbols.TryGetValue(toState, out int[]? symbolArray))
                symbols.UnionWith(symbolArray);
    }

    /// <summary>
    /// Extracts the 'FromState' values from a segment of transitions.
    /// </summary>
    /// <param name="transitions">Array of transitions.</param>
    /// <param name="start">Start index of the segment.</param>
    /// <param name="end">End index of the segment.</param>
    /// <returns>Array of from-states.</returns>
    private static int[] ExtractFromStates(Transition[] transitions, int start, int end)
    {
        int[] result = new int[end - start];
        for (int i = start; i < end; i++)
            result[i - start] = transitions[i].FromState;
        return result;
    }

}

