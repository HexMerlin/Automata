namespace Automata;

/// <summary>
/// Represents a deterministic finite automaton (DFA).
/// </summary>
public class DFA : IFsa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the DFA.
    /// </summary>
    public Alphabet Alphabet { get; }

    private readonly SortedDictionary<long, int> transitions;

    /// <summary>
    /// Gets or sets the initial state of the DFA.
    /// </summary>
    public int InitialState { get; private set; } = -1;

    private readonly SortedSet<int> finalStates;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DFA"/> class with an empty alphabet.
    /// </summary>
    public DFA() : this(new Alphabet())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DFA"/> class with the specified alphabet, transitions, initial state, and final states.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    /// <param name="transitions">The transitions of the DFA.</param>
    /// <param name="initialState">The initial state of the DFA.</param>
    /// <param name="finalStates">The final states of the DFA.</param>
    public DFA(Alphabet alphabet, IEnumerable<Transition> transitions, int initialState, IEnumerable<int> finalStates)
    {
        this.Alphabet = alphabet;
        this.transitions = new();
        SetInitial(initialState);
        this.finalStates = new(finalStates);
        foreach (Transition transition in transitions)
        {
            AddTransition(transition);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DFA"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    public DFA(Alphabet alphabet)
    {
        this.Alphabet = alphabet;
        this.transitions = [];
        this.finalStates = [];
    }

    /// <summary>
    /// Sets the initial state of the DFA.
    /// </summary>
    /// <param name="state">The state to set as the initial state.</param>
    private void SetInitial(int state) => InitialState = state;

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">The state to set or remove as a final state.</param>
    /// <param name="final">If <c>true</c>, the state is added to the final states; otherwise, it is removed.</param>
    private void SetFinal(int state, bool final = true)
    {
        if (final)
            finalStates.Add(state);
        else
            finalStates.Remove(state);
    }

    /// <summary>
    /// Indicates whether the specified state is the initial state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is the initial state; otherwise, <c>false</c>.</returns>
    public bool IsInitial(int state) => state == InitialState;

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is a final state; otherwise, <c>false</c>.</returns>
    public bool IsFinal(int state) => finalStates.Contains(state);

    /// <summary>
    /// Gets the final states of the DFA.
    /// </summary>
    public IReadOnlySet<int> FinalStates => finalStates;

    /// <summary>
    /// Gets the transitions of the DFA.
    /// </summary>
    public IEnumerable<Transition> Transitions
        => transitions.Select(kvp => new Transition(Split(kvp.Key).Item1, Split(kvp.Key).Item2, kvp.Value));

    /// <summary>
    /// Gets a value indicating whether the DFA is epsilon-free.
    /// </summary>
    public bool EpsilonFree => true;

    /// <summary>
    /// Gets the epsilon transitions of the DFA, which is always empty.
    /// </summary>
    public IEnumerable<EpsilonTransition> EpsilonTransitions => Enumerable.Empty<EpsilonTransition>();

    /// <summary>
    /// Adds a transition to the DFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void AddTransition(Transition transition)
    {
        long key = Merge(transition.FromState, transition.Symbol);
        transitions.Add(key, transition.ToState);
    }

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <returns>A minimized DFA.</returns>
    public DFA Minimized()
    {
        DFA reverseDfa = Reversed().ToDFA();
        return reverseDfa.Reversed().ToDFA();
    }

    /// <summary>
    /// Reverses the DFA.
    /// </summary>
    /// <returns>An NFA representing the reversed DFA.</returns>
    public NFA Reversed() => new(this, applyReverseOperation: true);

    /// <summary>
    /// Converts the DFA to an NFA.
    /// </summary>
    /// <returns>An NFA representing the DFA.</returns>
    public NFA ToNFA() => new(this);

    /// <summary>
    /// Merges two integers into a single long value.
    /// </summary>
    /// <param name="a">The first integer.</param>
    /// <param name="b">The second integer.</param>
    /// <returns>A long value representing the merged integers.</returns>
    private static long Merge(int a, int b) => (long)a << 32 | (uint)b;

    /// <summary>
    /// Splits a long value into two integers.
    /// </summary>
    /// <param name="value">The long value to split.</param>
    /// <returns>A tuple containing the two integers.</returns>
    private static (int, int) Split(long value) => ((int)(value >> 32), (int)value);
}
