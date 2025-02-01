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
    /// Initializes a new instance of the <see cref="DfaReverseLookup"/> class using the specified DFA.
    /// </summary>
    /// <param name="dfa">The DFA to initialize the reverse lookup from.</param>
    public DfaReverseLookup(Dfa dfa) : this(dfa.Transitions().ToArray()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DfaReverseLookup"/> class using the specified transitions.
    /// </summary>
    /// <param name="transitions">Array of unique transitions to initialize the reverse lookup from.</param>
    private DfaReverseLookup(Transition[] transitions)
    {
        if (transitions.Length == 0)
            return;

        Array.Sort(transitions, ComparerByToState());

        this.toStateSymbol_FromStates = new Dictionary<(int, int), int[]>(transitions.Length);
        this.toState_Symbols = new Dictionary<int, int[]>(transitions.Length);

        int start = 0;
        (int toState, int symbol) = (transitions[0].ToState, transitions[0].Symbol);

        var symbolsList = new List<int> { symbol }; // Track symbols for the current toState

        for (int i = 1; i < transitions.Length; i++)
        {
            var transition = transitions[i];

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
                toStateSymbol_FromStates[(toState, symbol)] = ExtractFromStates(transitions, start, i);
                start = i;
            }

            (toState, symbol) = (transition.ToState, transition.Symbol);
        }

        // Store last segments
        toStateSymbol_FromStates[(toState, symbol)] = ExtractFromStates(transitions, start, transitions.Length);
        toState_Symbols[toState] = symbolsList.ToArray();
    }

    /// <summary>
    /// Gets the array of from-states for the specified to-state and symbol.
    /// The returned states are unique and ordered.
    /// </summary>
    /// <param name="toState">The destination state.</param>
    /// <param name="symbol">The transition symbol.</param>
    /// <returns>Array of from-states.</returns>
    public int[] FromStates(int toState, int symbol) => toStateSymbol_FromStates[(toState, symbol)];

    /// <summary>
    /// Gets the array of symbols for the specified to-state.
    /// The returned symbols are unique and ordered.
    /// </summary>
    /// <param name="toState">The destination state.</param>
    /// <returns>Array of symbols.</returns>
    public int[] Symbols(int toState) => toState_Symbols[toState];

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

    /// <summary>
    /// A comparer that compares transitions by their to states.
    /// </summary>
    /// <returns>A comparer that compares transitions in reversed order: {ToState, Symbol, FromState}.</returns>
    private static Comparer<Transition> ComparerByToState() => Comparer<Transition>.Create((t1, t2) =>
    {
        int c = t1.ToState.CompareTo(t2.ToState);
        if (c != 0) return c;

        c = t1.Symbol.CompareTo(t2.Symbol);
        if (c != 0) return c;

        return t1.FromState.CompareTo(t2.FromState);
    });
}

