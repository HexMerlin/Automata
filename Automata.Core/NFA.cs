namespace Automata.Core;

/// <summary>
/// Represents a nondeterministic finite automaton (NFA).
/// </summary>
public class NFA : IFsa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the NFA.
    /// </summary>
    public Alphabet Alphabet { get; }

    private readonly NonEpsilonTransitionSet nonEpsilonTransitions;
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
            AddAll(dfa.Transitions.Select(t => t.Reverse()));
            SetInitial(dfa.FinalStates);
            SetFinal(dfa.InitialState);
        }
        else
        {
            AddAll(dfa.Transitions);
            SetInitial(dfa.InitialState);
            SetFinal(dfa.FinalStates);
        }
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="NFA"/> class from a set of sequences.
    /// </summary>
    /// <param name="sequences">The sequences to add to the NFA.</param>
    public NFA(IEnumerable<IEnumerable<string>> sequences) : this() => AddAll(sequences);

    /// <summary>
    /// Initializes a new instance of the <see cref="NFA"/> class with the specified parameters.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the NFA.</param>
    /// <param name="nonEpsilonTransitions">The non-epsilon transitions of the NFA.</param>
    /// <param name="epsilonTransitions">The epsilon transitions of the NFA.</param>
    /// <param name="initialStates">The initial states of the NFA.</param>
    /// <param name="finalStates">The final states of the NFA.</param>
    private NFA(Alphabet alphabet, IEnumerable<Transition> nonEpsilonTransitions, IEnumerable<EpsilonTransition> epsilonTransitions, IEnumerable<int> initialStates, IEnumerable<int> finalStates)
    {
        this.Alphabet = alphabet;
        this.nonEpsilonTransitions = new NonEpsilonTransitionSet(nonEpsilonTransitions);
        this.epsilonTransitions = new EpsilonTransitionSet(epsilonTransitions);
        this.initialStates = new SortedSet<int>(initialStates);
        this.finalStates = new SortedSet<int>(finalStates);
    }


    /// <summary>
    /// Adds a non-epsilon transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(Transition transition) => nonEpsilonTransitions.Add(transition);


    /// <summary>
    /// Adds an epsilon transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(EpsilonTransition transition) => epsilonTransitions.Add(transition);
 

    /// <summary>
    /// Adds multiple non-epsilon transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void AddAll(IEnumerable<Transition> transitions) => nonEpsilonTransitions.AddAll(transitions);
    
    /// <summary>
    /// Adds multiple epsilon transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void AddAll(IEnumerable<EpsilonTransition> transitions) => epsilonTransitions.AddAll(transitions);

    /// <summary>
    /// Adds a sequence of symbols to the NFA.
    /// </summary>
    /// <remarks>
    /// Any missing symbols in the alphabet will be added to the alphabet.
    /// </remarks>
    /// <param name="sequence">The sequence of symbols to add.</param>
    public void Add(IEnumerable<string> sequence)
    {
        int maxState = FindMaxState();

        if (initialStates.Count == 0)
            SetInitial(++maxState);

        int fromState = initialStates.Min;

        foreach (string symbol in sequence)
        {
            Transition transition = new Transition(fromState, Alphabet.GetOrAdd(symbol), ++maxState);
            nonEpsilonTransitions.Add(transition);
            fromState = maxState;
        }
        finalStates.Add(maxState);
    }

    /// <summary>
    /// Adds a set of sequences to the NFA.
    /// </summary>
    /// <remarks>
    /// Any missing symbols in the alphabet will be added to the alphabet.
    /// </remarks>
    /// <param name="sequences">The sequences to add to the NFA.</param>
    public void AddAll(IEnumerable<IEnumerable<string>> sequences)
    {
        foreach (IEnumerable<string> sequence in sequences)
            Add(sequence);
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

    /// <summary>
    /// Sets the specified state as an initial state or removes it from the initial states.
    /// </summary>
    /// <param name="state">The state to set or remove as an initial state.</param>
    /// <param name="initial">If <c>true</c>, the state is added to the initial states; otherwise, it is removed.</param>
    public void SetInitial(int state, bool initial = true)
    {
        if (initial)
            initialStates.Add(state);
        else
            initialStates.Remove(state);
    }

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">The state to set or remove as a final state.</param>
    /// <param name="final">If <c>true</c>, the state is added to the final states; otherwise, it is removed.</param>
    public void SetFinal(int state, bool final = true)
    {
        if (final)
            finalStates.Add(state);
        else
            finalStates.Remove(state);
    }

    /// <summary>
    /// Sets the specified states as initial states or removes them from the initial states.
    /// </summary>
    /// <param name="states">The states to set or remove as initial states.</param>
    /// <param name="initial">If <c>true</c>, the states are added to the initial states; otherwise, they are removed.</param>
    public void SetInitial(IEnumerable<int> states, bool initial = true)
    {
        if (initial)
            initialStates.UnionWith(states);
        else
            initialStates.ExceptWith(states);
    }

    /// <summary>
    /// Sets the specified states as final states or removes them from the final states.
    /// </summary>
    /// <param name="states">The states to set or remove as final states.</param>
    /// <param name="final">If <c>true</c>, the states are added to the final states; otherwise, they are removed.</param>
    public void SetFinal(IEnumerable<int> states, bool final = true)
    {
        if (final)
            finalStates.UnionWith(states);
        else
            finalStates.ExceptWith(states);
    }

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
    public bool EpsilonFree => nonEpsilonTransitions.Count == 0;

    IEnumerable<Transition> IFsa.Transitions => nonEpsilonTransitions.Transitions;

    IEnumerable<EpsilonTransition> IFsa.EpsilonTransitions => epsilonTransitions.Transitions;

    /// <summary>
    /// Converts the NFA to a minimized DFA.
    /// </summary>
    /// <returns>A minimized DFA representing the NFA.</returns>
    public DFA ToMinimizedDFA() => ToDFA().Minimized();

    /// <summary>
    /// Finds the maximum state in the NFA.
    /// </summary>
    /// <returns>The maximum state in the NFA.</returns>
    public int FindMaxState()
    {
        static int Max(int a, int b) => a > b ? a : b;
        int maxInitial = initialStates.Count > 0 ? initialStates.Max : -1;
        int maxFinal = finalStates.Count > 0 ? finalStates.Max : -1;
        return Max(Max(nonEpsilonTransitions.MaxState, epsilonTransitions.MaxState), Max(maxInitial, maxFinal));
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
        epsilonTransitions.TraverseOnEpsilonMultipleInPlace(intermediateStates);
        HashSet<int> toStates = intermediateStates.SelectMany(s => nonEpsilonTransitions.TraverseOnSymbol(s, symbol)).ToHashSet();
        epsilonTransitions.TraverseOnEpsilonMultipleInPlace(toStates);
        return new IntSet(toStates);
    }

    /// <summary>
    /// Converts a NFA to a DFA.
    /// </summary>
    /// <remarks>Uses the Subset Construction algorithm.</remarks>
    /// <returns>A DFA representing the NFA.</returns>
    public DFA ToDFA()
    {
        List<Transition> dfaTransitions = new();
        HashSet<int> dfaFinalStates = new();

        int dfaMaxState = -1;
        Dictionary<IntSet, int> stateSetToDfaState = new();
        Queue<IntSet> queue = new();

        HashSet<int> initialStates = new HashSet<int>(InitialStates);
        epsilonTransitions.TraverseOnEpsilonMultipleInPlace(initialStates);
        int dfaInitialState = GetOrAddState(new IntSet(initialStates)); //adds initial state to dfa

        while (queue.Count > 0)
        {
            IntSet fromState = queue.Dequeue();
            IntSet symbols = nonEpsilonTransitions.GetAvailableSymbols(fromState);
            int dfaFromState = GetOrAddState(fromState);

            foreach (int symbol in symbols)
            {
                IntSet toState = GetToStates(fromState, symbol);
                int dfaToState = GetOrAddState(toState);
                dfaTransitions.Add(new Transition(dfaFromState, symbol, dfaToState));
            }
        }
        return new DFA(Alphabet, dfaTransitions, dfaInitialState, dfaFinalStates);

        int GetOrAddState(IntSet state)
        {
            if (!stateSetToDfaState.TryGetValue(state, out int dfaState))
            {
                dfaState = ++dfaMaxState; //create a new state in DFA
                stateSetToDfaState[state] = dfaState;
                queue.Enqueue(state);
            }
            if (state.Overlaps(finalStates))
                dfaFinalStates.Add(dfaState);
            return dfaState;
        }
    }

}
