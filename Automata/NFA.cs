namespace Automata;

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

    private readonly SortedSet<Transition> nonEpsilonTransitions;
    private readonly SortedSet<Transition> nonEpsilonTransitions_ByToState;

    private readonly SortedSet<EpsilonTransition> epsilonTransitions;
    private readonly SortedSet<EpsilonTransition> epsilonTransitions_ByToState;

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
    public NFA(Alphabet alphabet) : this(alphabet, Enumerable.Empty<Transition>(), Enumerable.Empty<EpsilonTransition>(), Enumerable.Empty<int>(), Enumerable.Empty<int>())
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
    public NFA(IEnumerable<IEnumerable<string>> sequences) : this()
    {
        AddAll(sequences);
    }

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
        this.nonEpsilonTransitions = new SortedSet<Transition>(nonEpsilonTransitions);
        this.nonEpsilonTransitions_ByToState = new SortedSet<Transition>(nonEpsilonTransitions, Transition.CompareByToState());
        this.epsilonTransitions = new SortedSet<EpsilonTransition>(epsilonTransitions); ;
        this.epsilonTransitions_ByToState = new SortedSet<EpsilonTransition>(epsilonTransitions, EpsilonTransition.CompareByToState());
        this.initialStates = new SortedSet<int>(initialStates);
        this.finalStates = new SortedSet<int>(finalStates);
    }


    /// <summary>
    /// Adds a non-epsilon transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(Transition transition)
    {
        nonEpsilonTransitions.Add(transition);
        nonEpsilonTransitions_ByToState.Add(transition);
    }

    /// <summary>
    /// Adds an epsilon transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(EpsilonTransition transition)
    {
        epsilonTransitions.Add(transition);
        epsilonTransitions_ByToState.Add(transition);
    }

    /// <summary>
    /// Adds multiple non-epsilon transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void AddAll(IEnumerable<Transition> transitions)
    {
        foreach (var transition in transitions)
        {
            nonEpsilonTransitions.Add(transition);
            nonEpsilonTransitions_ByToState.Add(transition);
        }
    }

    /// <summary>
    /// Adds multiple epsilon transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void AddAll(IEnumerable<EpsilonTransition> transitions)
    {
        foreach (var transition in transitions)
        {
            epsilonTransitions.Add(transition);
            epsilonTransitions_ByToState.Add(transition);
        }
    }

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
        foreach (var sequence in sequences)
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
    /// Gets the non-epsilon transitions of the NFA.
    /// </summary>
    public IReadOnlySet<Transition> Transitions => nonEpsilonTransitions;

    /// <summary>
    /// Gets the epsilon transitions of the NFA.
    /// </summary>
    public IReadOnlySet<EpsilonTransition> EpsilonTransitions => epsilonTransitions;

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
    public bool EpsilonFree => EpsilonTransitions.Count == 0;

    IEnumerable<Transition> IFsa.Transitions => Transitions;

    IEnumerable<EpsilonTransition> IFsa.EpsilonTransitions => EpsilonTransitions;

    /// <summary>
    /// Finds the maximum state in the NFA.
    /// </summary>
    /// <returns>The maximum state in the NFA.</returns>
    public int FindMaxState()
    {
        static int Max(int a, int b) => a > b ? a : b;

        int maxTransition = Max(nonEpsilonTransitions.Max.FromState, nonEpsilonTransitions_ByToState.Max.ToState);
        int maxEpsilonTransition = Max(epsilonTransitions.Max.FromState, epsilonTransitions_ByToState.Max.ToState);
        int maxInitial = initialStates.Count > 0 ? initialStates.Max : -1;
        int maxFinal = finalStates.Count > 0 ? finalStates.Max : -1;
        return Max(Max(maxTransition, maxEpsilonTransition), Max(maxInitial, maxFinal));
    }

    /// <summary>
    /// Returns the states reachable from the given state on the given symbol (epsilon transitions are ignored).
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The states reachable from the given state on the given symbol.</returns>
    private IEnumerable<int> TraverseOnSymbol(int fromState, int symbol)
        => nonEpsilonTransitions.GetViewBetween(new Transition(fromState, symbol, int.MinValue), new Transition(fromState, symbol, int.MaxValue)).Select(t => t.ToState);

    /// <summary>
    /// Returns the states reachable from the given state on a single epsilon transition.
    /// If the input state has an epsilon loop on itself, it will be included in the result.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The states reachable from the given state on a single epsilon transition.</returns>
    private IEnumerable<int> TraverseOnEpsilonOneStep(int fromState)
        => epsilonTransitions.GetViewBetween(new EpsilonTransition(fromState, int.MinValue), new EpsilonTransition(fromState, int.MaxValue)).Select(t => t.ToState);

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given states.
    /// Epsilon transitions are ignored.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    private IntSet GetAvailableSymbols(IEnumerable<int> fromStates)
        => new IntSet(fromStates.SelectMany(s => nonEpsilonTransitions.GetViewBetween(new Transition(s, int.MinValue, int.MinValue), new Transition(s, int.MaxValue, int.MaxValue)).Select(t => t.Symbol)));

    /// <summary>
    /// Extends the set of states with their epsilon closure in place.
    /// </summary>
    /// <param name="fromStates">The set of states to extend.</param>
    private void ExtendWithEpsilonClosureInPlace(HashSet<int> fromStates)
    {
        var queue = new Queue<int>(fromStates);

        while (queue.Count != 0)
        {
            int state = queue.Dequeue();
            var newStates = TraverseOnEpsilonOneStep(state);
            foreach (var newState in newStates)
            {
                if (fromStates.Add(newState))
                    queue.Enqueue(newState);
            }
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
        ExtendWithEpsilonClosureInPlace(intermediateStates);
        HashSet<int> toStates = intermediateStates.SelectMany(s => TraverseOnSymbol(s, symbol)).ToHashSet();
        ExtendWithEpsilonClosureInPlace(toStates);
        return new IntSet(toStates);
    }

    /// <summary>
    /// Converts the NFA to a DFA.
    /// </summary>
    /// <returns>A DFA representing the NFA.</returns>
    public DFA ToDFA()
    {
        List<Transition> dfaTransitions = new();
        HashSet<int> dfaFinalStates = new();

        int dfaMaxState = -1;
        Dictionary<IntSet, int> stateSetToDfaState = new();
        Queue<IntSet> queue = new();

        HashSet<int> initialStates = new HashSet<int>(this.initialStates);
        ExtendWithEpsilonClosureInPlace(initialStates);
        int dfaInitialState = GetOrAddState(new IntSet(initialStates)); //adds initial state to dfa


        while (queue.Count > 0)
        {
            IntSet fromState = queue.Dequeue();
            IntSet symbols = GetAvailableSymbols(fromState.Members);
            int dfaFromState = GetOrAddState(fromState);

            foreach (int symbol in symbols.Members)
            {
                IntSet toState = GetToStates(fromState.Members, symbol);
                int dfaToState = GetOrAddState(toState);
                dfaTransitions.Add(new Transition(dfaFromState, symbol, dfaToState));
            }
        }
        return new DFA(this.Alphabet, dfaTransitions, dfaInitialState, dfaFinalStates);

        int GetOrAddState(IntSet state)
        {
            if (!stateSetToDfaState.TryGetValue(state, out int dfaState))
            {
                dfaState = ++dfaMaxState; //create a new state in DFA
                stateSetToDfaState[state] = dfaState;
                queue.Enqueue(state);
            }
            if (state.Members.Overlaps(finalStates))
                dfaFinalStates.Add(dfaState);
            return dfaState;
        }
    }

    /// <summary>
    /// Converts the NFA to a minimized DFA.
    /// </summary>
    /// <returns>A minimized DFA representing the NFA.</returns>
    public DFA ToMinimizedDFA() => ToDFA().Minimized();
}
