using System.Runtime.CompilerServices;

namespace Automata.Core;

/// <summary>
/// Deterministic finite automaton (DFA).
/// </summary>
/// <remarks>
/// A DFA is a finite state machine that accepts or rejects finite sequences of symbols.
/// <para>· A DFA cannot contain epsilon transitions</para> 
/// <para>· Any mutation of a DFA is guaranteed to preserve its deterministic property.</para>
/// <para>· Mutation can make certain states unreachable (contain <c>Dead states</c>).</para>
/// </remarks>
public class Dfa : IDfa
{
    #region Data
    /// <summary>
    /// Alphabet used by the DFA.
    /// </summary>
    public Alphabet Alphabet { get; }

    private readonly SortedSet<Transition> orderByFromState = new();

    /// <summary>
    /// Initial state of the DFA.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="Constants.InvalidState"/> if the DFA has no initial state.
    /// </remarks>
    public int InitialState { get; private set; } = Constants.InvalidState;

    private readonly HashSet<int> finalStates = new();

    /// <summary>
    /// Upper limit for the maximum state number in the DFA. 
    /// <para>A value (<see cref="MaxState"/> + 1) is guaranteed to be an unused state number.</para>
    /// </summary>
    /// <remarks>
    /// This value represents an upper limit for state numbers in the DFA.
    /// The actual maximum state number may be lower, as removed states are not tracked for performance reasons.
    /// </remarks>
    public int MaxState { get; private set; } = Constants.InvalidState;
    #endregion Data

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the DFA.</param>
    public Dfa(Alphabet alphabet) => Alphabet = alphabet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet, transitions, initial state, and final states.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the DFA.</param>
    /// <param name="transitions">Transitions of the DFA.</param>
    /// <param name="initialState">Initial state of the DFA.</param>
    /// <param name="finalStates">Final states of the DFA.</param>
    internal Dfa(Alphabet alphabet, IEnumerable<Transition> transitions, int initialState, IEnumerable<int> finalStates) : this(alphabet)
    {
        SetInitial(initialState);
        SetFinal(finalStates);
        this.finalStates.UnionWith(finalStates);
        UnionWith(transitions);
    }

    /// <summary>
    /// Indicates whether the language of the DFA is the empty language (∅). This means the DFA does not accept anything, including the empty string (ϵ).
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> only if either; the DFA has no states, or the initial state is not a final state.
    /// </remarks>
    public bool IsEmptyLanguage => InitialState == Constants.InvalidState || finalStates.Count == 0;


    /// <summary>
    /// Indicates whether the DFA accepts ϵ - the empty sting. 
    /// <para>Returns <see langword="true"/> <c>iff</c> an InitialState exists and it is also a final state.</para>
    /// </summary>
    public bool AcceptsEpsilon => IsFinal(InitialState);

    /// <summary>
    /// Indicates whether the DFA is epsilon-free. Always returns <see langword="true"/>.
    /// </summary>
    public bool IsEpsilonFree => true;

    /// <summary>
    /// Sets the initial state of the DFA, updating the maximum state number if necessary.
    /// </summary>
    /// <param name="state">State to set as the initial state.</param>
    public void SetInitial(int state) => InitialState = UpdateMaxState(state);

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">State to set or remove as a final state.</param>
    /// <param name="final">
    /// If <see langword="true"/>, the state is added to the final states; otherwise, it is removed.
    /// </param>
    public void SetFinal(int state, bool final = true) => IncludeIf(final, state, finalStates);

    /// <summary>
    /// Sets the specified states as final states or removes them from the final states.
    /// </summary>
    /// <param name="states">States to set or remove as final states.</param>
    /// <param name="final">
    /// If <see langword="true"/>, the states are added to the final states; otherwise, they are removed.
    /// </param>
    public void SetFinal(IEnumerable<int> states, bool final = true) => IncludeIf(final, states, finalStates);

    /// <summary>
    /// Indicates whether the specified state is the initial state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the specified state is the initial state.
    /// </returns>
    public bool IsInitial(int state) => state == InitialState;

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the specified state is a final state.
    /// </returns>
    public bool IsFinal(int state) => finalStates.Contains(state);

    /// <summary>
    /// Final states of the DFA.
    /// </summary>
    public IReadOnlyCollection<int> FinalStates => finalStates;

    /// <summary>
    /// Adds a transition to the DFA, ensuring it remains deterministic.
    /// </summary>
    /// <remarks>
    /// If adding the transition would introduce nondeterminism (i.e., a transition with the same from-state and symbol already exists), the new transition will not be added.
    /// </remarks>
    /// <param name="transition">Transition to add.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the transition was added.
    /// </returns>
    public bool Add(Transition transition)
    {
        if (ReachableState(transition.FromState, transition.Symbol) != Constants.InvalidState)
            return false; // Cannot add; would introduce nondeterminism
        MaxState = Math.Max(MaxState, Math.Max(transition.FromState, transition.ToState));
        return orderByFromState.Add(transition);
    }

    /// <summary>
    /// Adds all provided transitions that are currently not present in the set.
    /// </summary>
    /// <param name="transitions">Transitions to add.</param>
    public void UnionWith(IEnumerable<Transition> transitions)
    {
        foreach (Transition transition in transitions)
            Add(transition);
    }

    /// <summary>
    /// Returns the transition from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">State from which to start.</param>
    /// <param name="symbol">Symbol to transition on.</param>
    /// <returns>
    /// The transition from the given state on the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.
    /// </returns>
    public Transition Transition(int fromState, int symbol)
        => orderByFromState.GetViewBetween(
            Core.Transition.MinTrans(fromState, symbol),
            Core.Transition.MaxTrans(fromState, symbol)
        ).FirstOrDefault(Core.Transition.Invalid);

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">State from which to start.</param>
    /// <returns>Set of transitions from the given state.</returns>
    public SortedSet<Transition> Transitions(int fromState)
        => orderByFromState.GetViewBetween(
            Core.Transition.MinTrans(fromState),
            Core.Transition.MaxTrans(fromState)
        );

    /// <summary>
    /// Returns a <see cref="StateView"/> of the given state.
    /// </summary>
    /// <param name="fromState">From state.</param>
    /// <remarks>
    /// This method provides a read-only view of the state transitions from the specified state.
    /// It is primarily for compatibility with contiguous memory representations like <see cref="Mfa"/>.
    /// When possible, use <see cref="Transitions(int)"/>, which avoids memory allocation and has less overhead.
    /// </remarks>
    /// <returns>A <see cref="StateView"/> for the given state.</returns>
    public StateView State(int fromState)
        => new StateView(fromState, Transitions(fromState).ToArray());

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">State from which to start.</param>
    /// <param name="symbol">Symbol to transition on.</param>
    /// <returns>
    /// The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.
    /// </returns>
    public int ReachableState(int fromState, int symbol)
        => orderByFromState.GetViewBetween(
            Core.Transition.MinTrans(fromState, symbol),
            Core.Transition.MaxTrans(fromState, symbol)
        ).FirstOrDefault(Core.Transition.Invalid).ToState;


    /// <summary>
    /// Indicates whether the DFA accepts the given sequence of symbols.
    /// </summary>
    /// <param name="sequence">Sequence of symbols to check.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the DFA accepts the sequence.
    /// </returns>
    /// <remarks>
    /// The DFA processes each symbol in the sequence, transitioning between states according to its transition function.
    /// If the DFA reaches a final state after processing all symbols, the sequence is accepted.
    /// </remarks>
    public bool Accepts(IEnumerable<string> sequence)
    {
        int state = InitialState;
        foreach (string symbol in sequence)
        {
            int symbolIndex = Alphabet[symbol];
            if (symbolIndex == Constants.InvalidSymbolIndex)
                return false;

            state = ReachableState(state, symbolIndex);
            if (state == Constants.InvalidState)
                return false;
        }
        return IsFinal(state);
    }

    /// <summary>
    /// Conditionally includes or excludes a state in a set based on the provided condition.
    /// </summary>
    /// <param name="condition">If <see langword="true"/>, the state is added; otherwise, it is removed.</param>
    /// <param name="state">State to include or exclude.</param>
    /// <param name="set">Set to modify.</param>
    private void IncludeIf(bool condition, int state, HashSet<int> set)
    {
        if (condition)
            set.Add(UpdateMaxState(state));
        else
            set.Remove(state);
    }

    /// <summary>
    /// Conditionally includes or excludes a collection of states in a set based on the provided condition.
    /// </summary>
    /// <param name="condition">If <see langword="true"/>, the states are added; otherwise, they are removed.</param>
    /// <param name="states">States to include or exclude.</param>
    /// <param name="set">Set to modify.</param>
    private void IncludeIf(bool condition, IEnumerable<int> states, HashSet<int> set)
    {
        if (condition)
        {
            foreach (int state in states)
                set.Add(UpdateMaxState(state));
        }
        else
        {
            set.ExceptWith(states);
        }
    }

    /// <summary>
    /// Updates the maximum state number if the provided state is greater.
    /// </summary>
    /// <param name="state">State to compare and potentially update.</param>
    /// <returns>The input state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int UpdateMaxState(int state)
    {
        MaxState = Math.Max(MaxState, state);
        return state;
    }

    /// <summary>
    /// Gets the transitions of the DFA.
    /// </summary>
    /// <returns>An collection of transitions.</returns>
    public IReadOnlyCollection<Transition> Transitions() => orderByFromState;

    /// <summary>
    /// Gets the epsilon transitions of the DFA, which is always empty.
    /// </summary>
    /// <returns>An empty collection of <see cref="EpsilonTransition"/>.</returns>
    public IReadOnlyCollection<EpsilonTransition> EpsilonTransitions() => Array.Empty<EpsilonTransition>();

    ///<summary>
    /// States of the DFA in breadth-first order, following transitions in lexicographical order of their symbols.
    /// </summary>
    /// <remarks>
    /// This traversal ensures a consistent and well-defined ordering of states.
    /// At each state, transitions are explored in order sorted lexicographically by their symbol strings using <see cref="StringComparer.Ordinal"/>.
    /// Transitions with lexicographically smaller symbols are followed before those with larger symbols,
    /// providing a deterministic sequence of states regardless of the underlying data structures.
    /// </remarks>
    /// <returns>
    /// List of states visited in breadth-first order, starting from the <see cref="InitialState"/>.
    /// </returns>
    internal List<int> StatesInBreadthFirstOrderBySymbol()
    {
        if (InitialState == Constants.InvalidState)
            return [];

        StringComparer comparer = StringComparer.Ordinal;

        HashSet<int> visited = new() { InitialState };
        Queue<int> queue = new();
        List<int> statesInOrder = new();

        queue.Enqueue(InitialState);

        while (queue.Count > 0)
        {
            int state = queue.Dequeue();
            statesInOrder.Add(state);

            foreach (int nextState in Transitions(state)
                .OrderBy(t => Alphabet[t.Symbol], comparer)
                .Select(t => t.ToState))
            {
                if (visited.Add(nextState))
                    queue.Enqueue(nextState);
            }
        }

        return statesInOrder;
    }


    /// <summary>
    /// Maps states to their canonical state numbers in a breadth-first order.
    /// </summary>
    /// <remarks>
    /// This method traverses the DFA in a well-defined breadth-first order and assigns a unique canonical state number to each state.
    /// A BFS traversal starts in the <see cref="InitialState"/> and processes transitions breadth-first, then in lexicographical order of their symbols, ensuring a consistent and well-defined ordering of states.
    /// The initial state is always assigned the canonical state number <c>0</c>.
    /// The resulting map can be used to remap states to their canonical numbers.
    /// </remarks>
    /// <returns>
    /// A dictionary mapping each state to its canonical state number.
    /// </returns>
    internal Dictionary<int, int> StatesToCanonicalStatesMap()
    {
        if (InitialState == Constants.InvalidState)
            return new Dictionary<int, int>();

        StringComparer comparer = StringComparer.Ordinal;

        Dictionary<int, int> stateToCanonicalMap = new();
        Queue<int> queue = new();

        // Enqueue the initial state and add it to the map
        queue.Enqueue(InitialState);
        stateToCanonicalMap.Add(InitialState, stateToCanonicalMap.Count);

        while (queue.Count > 0)
        {
            int state = queue.Dequeue();

            foreach (int toState in Transitions(state)
                .OrderBy(t => Alphabet[t.Symbol], comparer)
                .Select(t => t.ToState))
            {
                if (!stateToCanonicalMap.ContainsKey(toState))
                {
                    queue.Enqueue(toState);
                    stateToCanonicalMap.Add(toState, stateToCanonicalMap.Count);
                }
            }
        }

        return stateToCanonicalMap;
    }




}
