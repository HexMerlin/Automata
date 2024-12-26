namespace Automata.Core;

/// <summary>
/// Represents a nondeterministic finite automaton (NFA).
/// </summary>
/// <remarks>States are represented simply as integers (<see langword="int"/>), which essentially are just unique IDs.
/// <para>NFAs are defined mainly by two sets of transitions (symbolic and epsilon), which are kept separate for performance</para>
/// <para>In addition, there are two sets defining the initial states and final states respectively.</para>
/// <para>NFAs (in contrast to DFAs) can have multiple initial states.</para>
/// </remarks>
public class NFA : IFsa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the NFA.
    /// </summary>
    public Alphabet Alphabet { get; }

    private readonly SymbolicTransitionSet symbolicTransitions;
    private readonly EpsilonTransitionSet epsilonTransitions;
 
    private readonly SortedSet<int> initialStates;
    private readonly SortedSet<int> finalStates;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="NFA"/> class with an empty alphabet.
    /// </summary>
    public NFA() : this(new Alphabet())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NFA"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the NFA.</param>
    public NFA(Alphabet alphabet) : this(alphabet, [], [], [], [])
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NFA"/> class from a DFA.
    /// </summary>
    /// <param name="dfa">The DFA to convert to an NFA.</param>
    /// <param name="applyReverseOperation">Iff <c>true</c>, the DFA is reversed.</param>
    internal NFA(DFA dfa, bool applyReverseOperation = false) : this(dfa.Alphabet)
    {
        this.Alphabet = dfa.Alphabet;

        if (applyReverseOperation)
        {
            UnionWith(dfa.Transitions.Select(t => t.Reverse()));
            SetInitial(dfa.FinalStates);
            SetFinal(dfa.InitialState);
        }
        else
        {
            UnionWith(dfa.Transitions);
            SetInitial(dfa.InitialState);
            SetFinal(dfa.FinalStates);
        }
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="NFA"/> class from a set of sequences.
    /// </summary>
    /// <param name="sequences">The sequences to add to the NFA.</param>
    public NFA(IEnumerable<IEnumerable<string>> sequences) : this() => UnionWith(sequences);

    /// <summary>
    /// Initializes a new instance of the <see cref="NFA"/> class with the specified parameters.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the NFA.</param>
    /// <param name="symbolicTransitions">The symbolic (non-epsilon) transitions of the NFA.</param>
    /// <param name="epsilonTransitions">The epsilon transitions of the NFA.</param>
    /// <param name="initialStates">The initial states of the NFA.</param>
    /// <param name="finalStates">The final states of the NFA.</param>
    private NFA(Alphabet alphabet, IEnumerable<SymbolicTransition> symbolicTransitions, IEnumerable<EpsilonTransition> epsilonTransitions, IEnumerable<int> initialStates, IEnumerable<int> finalStates)
    {
        this.Alphabet = alphabet;
        this.symbolicTransitions = new (symbolicTransitions);
        this.epsilonTransitions = new (epsilonTransitions);
        this.initialStates = [.. initialStates];
        this.finalStates = [.. finalStates];
    }

    /// <summary>
    /// Adds a symbolic (= non-epsilon) transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(SymbolicTransition transition) => symbolicTransitions.Add(transition);

    /// <summary>
    /// Adds an epsilon transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(EpsilonTransition transition) => epsilonTransitions.Add(transition);

    /// <summary>
    /// Adds multiple symbolic transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void UnionWith(IEnumerable<SymbolicTransition> transitions) => symbolicTransitions.UnionWith(transitions);
    
    /// <summary>
    /// Adds multiple epsilon transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void UnionWith(IEnumerable<EpsilonTransition> transitions) => epsilonTransitions.UnionWith(transitions);

    /// <summary>
    /// Adds a sequence of symbols to be accepted by the NFA.
    /// </summary>
    /// <remarks>
    /// Any missing symbols in the alphabet will be added to the alphabet.
    /// </remarks>
    /// <param name="sequence">The sequence of symbols to add.</param>
    public void UnionWith(IEnumerable<string> sequence)
    {
        int maxState = MaxState;

        SetInitial(++maxState);

        int fromState = maxState;

        foreach (string symbol in sequence)
        {
            SymbolicTransition transition = new SymbolicTransition(fromState, Alphabet.GetOrAdd(symbol), ++fromState);
            symbolicTransitions.Add(transition);
           
        }
        finalStates.Add(fromState);
    }

    /// <summary>
    /// Adds a set of sequences to be accepted by the NFA.
    /// </summary>
    /// <remarks>
    /// Any missing symbols in the alphabet will be added to the alphabet.
    /// </remarks>
    /// <param name="sequences">The sequences to add to the NFA.</param>
    public void UnionWith(IEnumerable<IEnumerable<string>> sequences)
    {
        foreach (IEnumerable<string> sequence in sequences)
            UnionWith(sequence);
    }

    /// <summary>
    /// Indicates whether the specified state is an initial state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is an initial state; otherwise, <c>false</c>.</returns>
    public bool IsInitial(int state) => initialStates.Contains(state);

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is a final state; otherwise, <c>false</c>.</returns>
    public bool IsFinal(int state) => finalStates.Contains(state);


    private static void IncludeIf(bool condition, int state, SortedSet<int> set)
    {
        if (condition) set.Add(state);
        else set.Remove(state);
    }

    private static void IncludeIf(bool condition, IEnumerable<int> states, SortedSet<int> set)
    {
        if (condition) set.UnionWith(states);
        else set.ExceptWith(states);
    }

    /// <summary>
    /// Sets the specified state as an initial state or removes it from the initial states.
    /// </summary>
    /// <param name="state">The state to set or remove as an initial state.</param>
    /// <param name="initial">If <c>true</c>, the state is added to the initial states; otherwise, it is removed.</param>
    public void SetInitial(int state, bool initial = true) => IncludeIf(initial, state, initialStates);

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">The state to set or remove as a final state.</param>
    /// <param name="final">If <c>true</c>, the state is added to the final states; otherwise, it is removed.</param>
    public void SetFinal(int state, bool final = true) => IncludeIf(final, state, finalStates);

    /// <summary>
    /// Sets the specified states as initial states or removes them from the initial states.
    /// </summary>
    /// <param name="states">The states to set or remove as initial states.</param>
    /// <param name="initial">If <c>true</c>, the states are added to the initial states; otherwise, they are removed.</param>
    public void SetInitial(IEnumerable<int> states, bool initial = true) => IncludeIf(initial, states, initialStates);

    /// <summary>
    /// Sets the specified states as final states or removes them from the final states.
    /// </summary>
    /// <param name="states">The states to set or remove as final states.</param>
    /// <param name="final">If <c>true</c>, the states are added to the final states; otherwise, they are removed.</param>
    public void SetFinal(IEnumerable<int> states, bool final = true) => IncludeIf(final, states, finalStates);

    /// <summary>
    /// Gets the initial states of the NFA.
    /// </summary>
    public IReadOnlySet<int> InitialStates => initialStates;

    /// <summary>
    /// Gets the final states of the NFA.
    /// </summary>
    public IReadOnlySet<int> FinalStates => finalStates;

    /// <summary>
    /// Gets a value indicating whether the NFA is epsilon-free.
    /// </summary>
    public bool EpsilonFree => symbolicTransitions.Count == 0;

    IEnumerable<SymbolicTransition> IFsa.Transitions => symbolicTransitions.Transitions;

    IEnumerable<EpsilonTransition> IFsa.EpsilonTransitions => epsilonTransitions.Transitions;

    /// <summary>
    /// Converts the NFA to a minimized DFA.
    /// </summary>
    /// <returns>A minimized DFA representing the NFA.</returns>
    public DFA ToMinimizedDFA() => ToDFA().Minimized();

    /// <summary>
    /// O(1) retrieval of the minimal state (state with the minimal value) in the NFA.
    /// </summary>
    /// <remarks>If the set is empty, <see cref="Constants.InvalidState"/> is returned</remarks>
    /// <returns>The minimal state in the NFA.</returns>
    public int MinState
    {
        get
        {
            static int Min(int a, int b) => a <= b ? a : b;
            int minInitial = initialStates.Count > 0 ? initialStates.Min : Constants.InvalidState;
            int minFinal = finalStates.Count > 0 ? finalStates.Min : Constants.InvalidState;
            return Min(Min(symbolicTransitions.MinState, epsilonTransitions.MinState), Min(minInitial, minFinal));
       }
    }

    /// <summary>
    /// O(1) retrieval of the maximum state (state with the maximum value) in the NFA.
    /// </summary>
    /// <remarks>If the set is empty, <see cref="Constants.InvalidState"/> is returned</remarks>
    /// <returns>The maximum state in the NFA.</returns>
    public int MaxState
    {
        get
        {
            static int Max(int a, int b) => a >= b ? a : b;
            int maxInitial = initialStates.Count > 0 ? initialStates.Max : Constants.InvalidState;
            int maxFinal = finalStates.Count > 0 ? finalStates.Max : Constants.InvalidState;
            return Max(Max(symbolicTransitions.MaxState, epsilonTransitions.MaxState), Max(maxInitial, maxFinal));
        }
    }

    /// <summary>
    /// Returns the states reachable from the given states on the given symbol, including epsilon closures.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The states reachable from the given states on the given symbol, including epsilon closures.</returns>
    private IntSet GetToStates(IEnumerable<int> fromStates, int symbol)
    {
        HashSet<int> intermediateStates = fromStates.ToHashSet();
        epsilonTransitions.ReachableStatesOnEpsilonInPlace(intermediateStates);
        HashSet<int> toStates = intermediateStates.SelectMany(s => symbolicTransitions.ReachableStates(s, symbol)).ToHashSet();
        epsilonTransitions.ReachableStatesOnEpsilonInPlace(toStates);
        return new IntSet(toStates);
    }

    /// <summary>
    /// Converts a NFA to a DFA.
    /// </summary>
    /// <remarks>Uses the Powerset Construction algorithm (a.k.a. Subset Construction algorithm).</remarks>
    /// <returns>A DFA representing the NFA.</returns>
    public DFA ToDFA()
    {
        List<SymbolicTransition> dfaTransitions = [];
        HashSet<int> dfaFinalStates = [];

        int dfaMaxState = Constants.InvalidState;
        Dictionary<IntSet, int> stateSetToDfaState = [];
        Queue<IntSet> queue = new();

        HashSet<int> initialStates = [.. InitialStates];
        epsilonTransitions.ReachableStatesOnEpsilonInPlace(initialStates);
        int dfaInitialState = GetOrAddState(new IntSet(initialStates)); //add initial NFA states to dfa as a single state

        while (queue.Count > 0)
        {
            IntSet fromState = queue.Dequeue();
            IntSet symbols = symbolicTransitions.GetAvailableSymbols(fromState);
            int dfaFromState = GetOrAddState(fromState);

            foreach (int symbol in symbols)
            {
                IntSet toState = GetToStates(fromState, symbol);
                int dfaToState = GetOrAddState(toState);
                dfaTransitions.Add(new SymbolicTransition(dfaFromState, symbol, dfaToState));
            }
        }
        return new DFA(Alphabet, dfaTransitions, dfaInitialState, dfaFinalStates);

        int GetOrAddState(IntSet combinedState)
        {
            if (!stateSetToDfaState.TryGetValue(combinedState, out int dfaState))
            {
                dfaState = ++dfaMaxState; //create a new state in DFA
                stateSetToDfaState[combinedState] = dfaState;
                queue.Enqueue(combinedState);
            }
            if (combinedState.Overlaps(finalStates))
                dfaFinalStates.Add(dfaState);
            return dfaState;
        }
    }

}
