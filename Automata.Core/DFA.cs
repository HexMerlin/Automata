namespace Automata.Core;

/// <summary>
/// Represents a deterministic finite automaton (DFA).
/// </summary>
/// <remarks>A DFA is always deterministic and epsilon free. </remarks>
public class DFA : IFsa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the DFA.
    /// </summary>
    public Alphabet Alphabet { get; }

    private readonly Dictionary<StateSymbolPair, int> transitions = [];

    /// <summary>
    /// Gets or sets the initial state of the DFA.
    /// </summary>
    public int InitialState { get; private set; } = Constants.InvalidState; //no initial state

    private readonly HashSet<int> finalStates = [];
    #endregion Data

    /// <summary>
    /// Initializes a new instance of the <see cref="DFA"/> class with an empty alphabet.
    /// </summary>
    public DFA() : this(new Alphabet()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DFA"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    public DFA(Alphabet alphabet) => Alphabet = alphabet;

    /// <summary>
    /// Initializes a new instance of the <see cref="DFA"/> class with the specified alphabet, transitions, initial state, and final states.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    /// <param name="transitions">The transitions of the DFA.</param>
    /// <param name="initialState">The initial state of the DFA.</param>
    /// <param name="finalStates">The final states of the DFA.</param>
    public DFA(Alphabet alphabet, IEnumerable<SymbolicTransition> transitions, int initialState, IEnumerable<int> finalStates) : this(alphabet)
    {
        SetInitial(initialState);
        this.finalStates.UnionWith(finalStates);
        foreach (SymbolicTransition transition in transitions)
            AddTransition(transition);
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
    public IEnumerable<SymbolicTransition> Transitions
        => transitions.Select(kvp => new SymbolicTransition(Split(kvp.Key).Item1, Split(kvp.Key).Item2, kvp.Value));

    /// <summary>
    /// Gets a value indicating whether the DFA is epsilon-free.
    /// </summary>
    public bool EpsilonFree => true;

    /// <summary>
    /// Gets the epsilon transitions of the DFA, which is always empty.
    /// </summary>
    public IEnumerable<EpsilonTransition> EpsilonTransitions => [];

    /// <summary>
    /// Adds a transition to the DFA.
    /// </summary>
    /// <remarks>If a transition with the same from-state and the same symbol already exists, that transition will be replaced.</remarks>
    /// <param name="transition">The transition to add.</param>
    public void AddTransition(SymbolicTransition transition)
    {
        StateSymbolPair key = Merge(transition.FromState, transition.Symbol);
        transitions[key] = transition.ToState;
    }

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <remarks>Uses Brzozowski's algorithm</remarks>
    /// <returns>A minimized DFA.</returns>
    public DFA Minimized()
    {
        DFA reversed = Reversed().ToDFA();
        return reversed.Reversed().ToDFA();
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
    /// Merges two signed integers into a single signed 64-bit value.
    /// </summary>
    /// <param name="a">The first signed 32-bit integer.</param>
    /// <param name="b">The second signed 32-bit integer.</param>
    /// <returns>A signed 64-bit value representing the merged integers.</returns>
    public static StateSymbolPair Merge(int a, int b) => ((StateSymbolPair)a << 32) | ((StateSymbolPair)b & 0xFFFFFFFFL);

    /// <summary>
    /// Splits a signed 64-bit value into two signed 32-bit integers.
    /// </summary>
    /// <param name="value">The signed 64-bit value to split.</param>
    /// <returns>A tuple containing the two signed 32-bit integers.</returns>
    public static (int, int) Split(StateSymbolPair value) => ((int)(value >> 32), (int)(value & 0xFFFFFFFFL));
}
