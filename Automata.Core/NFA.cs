using System.Runtime.CompilerServices;
using Automata.Core.Alphabets;
using Automata.Core.Operations;

namespace Automata.Core;

/// <summary>
/// Represents a nondeterministic finite automaton (NFA).
/// </summary>
/// <remarks>
/// States are represented simply as integers (<see langword="int"/>), which essentially are just unique IDs.
/// <para>
/// NFAs are defined mainly by two sets of transitions (symbolic and epsilon), which are kept separate for performance.
/// </para>
/// <para>
/// In addition, there are two sets defining the initial states and final states respectively.
/// </para>
/// <para>
/// NFAs (in contrast to DFAs) can have multiple initial states.
/// </para>
/// </remarks>
public class Nfa : IFsa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the NFA.
    /// </summary>
    public MutableAlphabet Alphabet { get; }

    private readonly SortedSet<Transition> symbolicTransitions = new();
    private readonly SortedSet<EpsilonTransition> epsilonTransitions = new();

    private readonly HashSet<int> initialStates = [];
    private readonly HashSet<int> finalStates = [];

    /// <summary>
    /// Gets a upper bound for the maximum state number in the NFA.
    /// </summary>
    /// <remarks>
    /// This values denotes an upper bound for the state numbers in the NFA.
    /// The actual maximum state number may be lower (but not higher), since we do not keep track of removed states for performance reasons.
    /// </remarks>
    public int MaxState { get; private set; } = Constants.InvalidState;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Nfa"/> class with an empty alphabet.
    /// </summary>
    public Nfa() : this(new MutableAlphabet()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Nfa"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the NFA.</param>
    public Nfa(MutableAlphabet alphabet) => this.Alphabet = alphabet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Nfa"/> class from a DFA.
    /// </summary>
    /// <param name="dfa">The DFA to create an NFA from.</param>
    /// <param name="applyReverseOperation">If <see langword="true"/>, the NFA is reversed.</param>
    internal Nfa(Dfa dfa, bool applyReverseOperation = false) : this(dfa.Alphabet)
    {
        if (applyReverseOperation)
        {
            UnionWith(dfa.SymbolicTransitions().Select(t => t.Reverse()));
            SetInitial(dfa.FinalStates);
            SetFinal(dfa.InitialState);
        }
        else
        {
            UnionWith(dfa.SymbolicTransitions());
            SetInitial(dfa.InitialState);
            SetFinal(dfa.FinalStates);
        }
        this.MaxState = dfa.MaxState;
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="Nfa"/> class to accept a set of sequences.
    /// </summary>
    /// <param name="sequences">The sequences to add to the NFA.</param>
    public Nfa(IEnumerable<IEnumerable<string>> sequences) : this() => UnionWith(sequences);

    /// <summary>
    /// Indicates whether the NFA is empty.
    /// </summary>
    public bool IsEmpty => initialStates.Count == 0;

    /// <summary>
    /// Indicates whether the NFA is epsilon-free.
    /// </summary>
    public bool IsEpsilonFree => epsilonTransitions.Count == 0;

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The set of transitions from the given state.</returns>
    public SortedSet<Transition> Transitions(int fromState)
        => symbolicTransitions.GetViewBetween(Transition.MinTrans(fromState), Transition.MaxTrans(fromState));

    /// <summary>
    /// Returns the transitions from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The transitions from the given state on the given symbol.</returns>
    public SortedSet<Transition> Transitions(int fromState, int symbol)
        => symbolicTransitions.GetViewBetween(Transition.MinTrans(fromState, symbol), Transition.MaxTrans(fromState, symbol));

    /// <summary>
    /// Returns the states reachable from the given states on the given symbol, including epsilon closures.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The states reachable from the given states on the given symbol, including epsilon closures.</returns>
    public IntSet ReachableStates(IEnumerable<int> fromStates, int symbol)
    {
        HashSet<int> intermediateStates = fromStates.ToHashSet();
        ReachableStatesOnEpsilonInPlace(intermediateStates);
        HashSet<int> toStates = intermediateStates.SelectMany(s => ReachableStatesOnSingleSymbol(s, symbol)).ToHashSet();
        ReachableStatesOnEpsilonInPlace(toStates);
        return new(toStates);
    }

    /// <summary>
    /// Returns the states reachable from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>The states reachable from the given state on the given symbol.</returns>
    public IEnumerable<int> ReachableStatesOnSingleSymbol(int fromState, int symbol)
        => Transitions(fromState, symbol).Select(t => t.ToState);

    /// <summary>
    /// Returns the states reachable from the given state on a single epsilon transition.
    /// If the input state has an epsilon loop on itself, it will be included in the result.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The states reachable from the given state on a single epsilon transition.</returns>
    public IEnumerable<int> ReachableStatesOnSingleEpsilon(int fromState)
        => epsilonTransitions.GetViewBetween(EpsilonTransition.MinTrans(fromState), EpsilonTransition.MaxTrans(fromState)).Select(t => t.ToState);

    /// <summary>
    /// Extends the provided set of states with their epsilon closure in place.
    /// </summary>
    /// <remarks>Epsilon closure is all reachable states on epsilon transitions.</remarks>
    /// <param name="fromStates">The set of states to extend.</param>
    public void ReachableStatesOnEpsilonInPlace(HashSet<int> fromStates)
    {
        var queue = new Queue<int>(fromStates);
        while (queue.Count != 0)
        {
            int state = queue.Dequeue();
            IEnumerable<int> newStates = ReachableStatesOnSingleEpsilon(state);
            foreach (var newState in newStates)
                if (fromStates.Add(newState))
                    queue.Enqueue(newState);
        }
    }

    /// <summary>
    /// Returns the set of symbols that can be used to transition directly from the given states.
    /// </summary>
    /// <param name="fromStates">The states from which to start.</param>
    /// <returns>The set of symbols that can be used to transition directly from the given states.</returns>
    public IntSet AvailableSymbols(IEnumerable<int> fromStates)
        => new(fromStates.SelectMany(s => Transitions(s)).Select(t => t.Symbol));

    /// <summary>
    /// Adds a symbolic (non-epsilon) transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(Transition transition)
    {
        MaxState = Math.Max(MaxState, Math.Max(transition.FromState, transition.ToState));
        symbolicTransitions.Add(transition);
    }

    /// <summary>
    /// Adds an epsilon transition to the NFA.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void Add(EpsilonTransition transition)
    {
        MaxState = Math.Max(MaxState, Math.Max(transition.FromState, transition.ToState));
        epsilonTransitions.Add(transition);
    }

    /// <summary>
    /// Adds multiple symbolic transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void UnionWith(IEnumerable<Transition> transitions)
    {
        foreach (Transition transition in transitions)
            Add(transition);
    }

    /// <summary>
    /// Adds multiple epsilon transitions to the NFA.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void UnionWith(IEnumerable<EpsilonTransition> transitions)
    {
        foreach (EpsilonTransition transition in transitions)
            Add(transition);
    }

    /// <summary>
    /// Adds a sequence of symbols to be accepted by the NFA.
    /// </summary>
    /// <remarks>
    /// Any missing symbols in the alphabet will be added to the alphabet.
    /// </remarks>
    /// <param name="sequence">The sequence of symbols to add.</param>
    public void UnionWith(IEnumerable<string> sequence)
    {
        int fromState = MaxState;
        SetInitial(++fromState); // create a new initial state

        foreach (string symbol in sequence)
            Add(new Transition(fromState, Alphabet.GetOrAdd(symbol), ++fromState));
        SetFinal(fromState);
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

    /// <summary>
    /// Converts the NFA to a DFA.
    /// </summary>
    /// <returns>A DFA representing the NFA.</returns>
    public Dfa ToDfa() => Determinize.ToDfa(this);

    /// <summary>
    /// Converts the NFA to a minimized DFA.
    /// </summary>
    /// <returns>A minimized DFA representing the NFA.</returns>
    public Dfa ToMinimizedDFA() => Determinize.ToDfa(this).Minimized();

    /// <summary>
    /// Converts the NFA to a CFA.
    /// </summary>
    /// <returns>A CFA representing the NFA.</returns>
    public Cfa ToCfa() => new(this);

    /// <summary>
    /// Adds or removes a state from a set based on a condition.
    /// </summary>
    /// <param name="condition">If <c>true</c>, the state is added; otherwise, it is removed.</param>
    /// <param name="state">The state to add or remove.</param>
    /// <param name="set">The set to modify.</param>
    private void IncludeIf(bool condition, int state, HashSet<int> set)
    {
        if (condition) set.Add(UpdateMaxState(state));
        else set.Remove(state);
    }

    /// <summary>
    /// Adds or removes multiple states from a set based on a condition.
    /// </summary>
    /// <param name="condition">If <c>true</c>, the states are added; otherwise, they are removed.</param>
    /// <param name="states">The states to add or remove.</param>
    /// <param name="set">The set to modify.</param>
    private void IncludeIf(bool condition, IEnumerable<int> states, HashSet<int> set)
    {
        if (condition)
        {
            foreach (int state in states)
                set.Add(UpdateMaxState(state));
        }
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

    IAlphabet IFsa.Alphabet => Alphabet;

    /// <summary>
    /// Gets the symbolic transitions of the NFA.
    /// </summary>
    /// <returns>An enumerable of symbolic transitions.</returns>
    IEnumerable<Transition> IFsa.SymbolicTransitions() => symbolicTransitions;

    /// <summary>
    /// Gets the epsilon transitions of the NFA.
    /// </summary>
    /// <returns>An enumerable of epsilon transitions.</returns>
    IEnumerable<EpsilonTransition> IFsa.EpsilonTransitions() => epsilonTransitions;

    /// <summary>
    /// Updates the maximum state number if the provided state is greater.
    /// </summary>
    /// <param name="state">The state to compare.</param>
    /// <returns>The same state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int UpdateMaxState(int state)
    {
        MaxState = Math.Max(MaxState, state);
        return state;
    }
}
