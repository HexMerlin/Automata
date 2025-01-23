using System.Runtime.CompilerServices;
using System.Text;

namespace Automata.Core;

/// <summary>
/// Nondeterministic finite automaton (NFA).
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
public class Nfa : Fsa
{
    #region Data
  
    private readonly SortedSet<Transition> transitions;
    private readonly SortedSet<EpsilonTransition> epsilonTransitions;

    private readonly HashSet<int> initialStates;
    private readonly HashSet<int> finalStates;

    /// <summary>
    /// Upper bound for the maximum state number in the NFA.
    /// </summary>
    /// <remarks>
    /// This values denotes an upper bound for the state numbers in the NFA.
    /// The actual maximum state number may be lower (but not higher), since we do not keep track of removed states for performance reasons.
    /// </remarks>
    public int MaxState { get; private set; } = Constants.InvalidState;

    #endregion Data

    #region Constructors

    /// <summary>
    /// Initializes a new empty instance of the <see cref="Nfa"/>.
    /// </summary>
    /// <param name="alphabet">Alphabet to use for the NFA.</param>
    public Nfa(Alphabet alphabet) : base(alphabet)
    {
        transitions = [];
        epsilonTransitions = [];
        initialStates = [];
        finalStates = [];
    }

    /// <summary>
    /// Initializes a new clone of given <see cref="Nfa"/> with a new cloned alphabet.
    /// </summary>
    /// <param name="nfa">NFA to clone.</param>
    public Nfa(Nfa nfa) : base(nfa.Alphabet)
    {
        this.transitions = [.. nfa.transitions];
        this.epsilonTransitions = [.. nfa.epsilonTransitions];
        this.initialStates = [.. nfa.initialStates];
        this.finalStates = [.. nfa.finalStates];
        this.MaxState = nfa.MaxState;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Nfa"/> class from a Deterministic automaton.
    /// </summary>
    /// <param name="fsaDet">A deterministic automaton to create an NFA from.</param>
    /// <param name="applyReverseOperation">If <see langword="true"/>, the NFA is reversed.</param>
    internal Nfa(FsaDet fsaDet, bool applyReverseOperation = false) : base(fsaDet.Alphabet)
    {
        if (applyReverseOperation)
        {
            this.transitions = [.. fsaDet.Transitions().Select(t => t.Reverse())];
            this.initialStates = [.. fsaDet.FinalStates];
            this.finalStates = fsaDet.HasInitialState ? [fsaDet.InitialState] : [];
        }
        else
        {
            this.transitions = [.. fsaDet.Transitions()];
            this.initialStates = fsaDet.HasInitialState ? [fsaDet.InitialState] : [];
            this.finalStates = [.. fsaDet.FinalStates];

        }
        this.epsilonTransitions = [];
        this.MaxState = fsaDet.MaxState;
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="Nfa"/> class to accept a set of sequences.
    /// </summary>
    /// <param name="sequences">Sequences to add to the NFA.</param>
    public Nfa(IEnumerable<IEnumerable<string>> sequences) : this(new Alphabet()) => UnionWith(sequences);

    #endregion Constructors

    #region Accessors

    /// <summary>
    /// Indicates whether the specified state is an initial state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is an initial state; otherwise, <see langword="false"/>.</returns>
    public override bool IsInitial(int state) => initialStates.Contains(state);

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is a final state; otherwise, <see langword="false"/>.</returns>
    public override bool IsFinal(int state) => finalStates.Contains(state);

    /// <summary>
    /// Indicates whether the NFA accepts ϵ - the empty sting. 
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> <c>iff</c> the NFA has a final state 
    /// that is either an initial state 
    /// or can be reached from an initial state on only epsilon transitions.
    /// </remarks>
    public override bool AcceptsEpsilon
    {
        get
        {
            if (initialStates.Overlaps(finalStates))
                return true;
            else
            {
                HashSet<int> reachable = [.. initialStates];
                ReachableStatesOnEpsilonInPlace(reachable);
                return reachable.Overlaps(finalStates);
            }
        }
    }

    /// <summary>
    /// Indicates whether the NFA has any initial states.
    /// </summary>
    /// <returns><see langword="true"/> <c>iff</c> the NFA has at least one initial state.</returns>
    public override bool HasInitialState => initialStates.Count > 0;

    /// <summary>
    /// Indicates whether the NFA is epsilon-free.
    /// </summary>
    public override bool IsEpsilonFree => epsilonTransitions.Count == 0;


    /// <summary>
    /// Initial states of the NFA.
    /// </summary>
    public IReadOnlySet<int> InitialStates => initialStates;

    /// <summary>
    /// Final states of the NFA.
    /// </summary>
    public override IReadOnlyCollection<int> FinalStates => finalStates;

    /// <summary>
    /// Returns the set of transitions from the given state.
    /// </summary>
    /// <param name="fromState">State from which to start.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fromState"/> is negative.</exception>
    /// <returns>Set of transitions from the given state.</returns>
    public SortedSet<Transition> Transitions(int fromState)
        => transitions.GetViewBetween(Transition.MinTrans(fromState.ShouldNotBeNegative()), Transition.MaxTrans(fromState));

    /// <summary>
    /// Returns the transitions from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">State from which to start.</param>
    /// <param name="symbol">Symbol to transition on.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fromState"/> or <paramref name="symbol"/> is negative.</exception>
    /// <returns>Transitions from the given state on the given symbol.</returns>
    public SortedSet<Transition> Transitions(int fromState, int symbol)
        => transitions.GetViewBetween(Transition.MinTrans(fromState.ShouldNotBeNegative(), symbol.ShouldNotBeNegative()), Transition.MaxTrans(fromState, symbol));

    /// <summary>
    /// Transitions of the DFA.
    /// </summary>
    /// <returns>A collection of transitions.</returns>
    public override IReadOnlyCollection<Transition> Transitions() => transitions;

    /// <summary>
    /// Epsilon transitions of the DFA, which is always empty.
    /// </summary>
    /// <returns>A collection of <see cref="EpsilonTransition"/>.</returns>
    public override IReadOnlyCollection<EpsilonTransition> EpsilonTransitions() => epsilonTransitions;

    /// <summary>
    /// Returns the states reachable from the given states on the given symbol, including epsilon closures.
    /// </summary>
    /// <param name="fromStates">States from which to start.</param>
    /// <param name="symbol">Symbol to transition on.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="symbol"/> is negative.</exception>
    /// <returns>States reachable from the given states on the given symbol, including epsilon closures.</returns>
    public IntSet ReachableStates(IEnumerable<int> fromStates, int symbol)
    {
        symbol.ShouldNotBeNegative();
        HashSet<int> intermediateStates = fromStates.ToHashSet();
        ReachableStatesOnEpsilonInPlace(intermediateStates);
        HashSet<int> toStates = intermediateStates.SelectMany(s => ReachableStatesOnSingleSymbol(s, symbol)).ToHashSet();
        ReachableStatesOnEpsilonInPlace(toStates);
        return new(toStates);
    }

    /// <summary>
    /// Returns the states reachable from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">State from which to start.</param>
    /// <param name="symbol">Symbol to transition on.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fromState"/> or <paramref name="symbol"/> is negative.</exception>
    /// <returns>States reachable from the given state on the given symbol.</returns>
    public IEnumerable<int> ReachableStatesOnSingleSymbol(int fromState, int symbol)
        => Transitions(fromState, symbol).Select(t => t.ToState);

    /// <summary>
    /// Returns the states reachable from the given state on a single epsilon transition.
    /// If the input state has an epsilon loop on itself, it will be included in the result.
    /// </summary>
    /// <param name="fromState">State from which to start.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fromState"/> is negative.</exception>
    /// <returns>States reachable from the given state on a single epsilon transition.</returns>
    public IEnumerable<int> ReachableStatesOnSingleEpsilon(int fromState)
        => epsilonTransitions.GetViewBetween(EpsilonTransition.MinTransSearchKey(fromState.ShouldNotBeNegative()), EpsilonTransition.MaxTransSearchKey(fromState)).Select(t => t.ToState);

    /// <summary>
    /// Extends the provided set of states with their epsilon closure in place.
    /// </summary>
    /// <remarks>Epsilon closure is all reachable states on epsilon transitions.</remarks>
    /// <param name="fromStates">Set of states to extend.</param>
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
    /// <param name="fromStates">States from which to start.</param>
    /// <returns>Set of symbols that can be used to transition directly from the given states.</returns>
    public IntSet AvailableSymbols(IEnumerable<int> fromStates)
        => new(fromStates.SelectMany(s => Transitions(s)).Select(t => t.Symbol));

    #endregion Accessors

    #region Mutators

    /// <summary>
    /// Adds a symbolic (non-epsilon) transition to the NFA.
    /// </summary>
    /// <param name="transition">Transition to add.</param>
    public void Add(Transition transition)
    {
        MaxState = Math.Max(MaxState, Math.Max(transition.FromState, transition.ToState));
        transitions.Add(transition);
    }

    /// <summary>
    /// Adds an epsilon transition to the NFA.
    /// </summary>
    /// <param name="transition">Transition to add.</param>
    public void Add(EpsilonTransition transition)
    {
        MaxState = Math.Max(MaxState, Math.Max(transition.FromState, transition.ToState));
        epsilonTransitions.Add(transition);
    }

    /// <summary>
    /// Adds multiple symbolic transitions to the NFA.
    /// </summary>
    /// <param name="transitions">Transitions to add.</param>
    public void UnionWith(IEnumerable<Transition> transitions)
    {
        MaxState = transitions.Any() ? Math.Max(MaxState, transitions.Max(t => Math.Max(t.FromState, t.ToState))) : MaxState;
        this.transitions.UnionWith(transitions);
    }

    /// <summary>
    /// Adds multiple epsilon transitions to the NFA.
    /// </summary>
    /// <param name="transitions">Transitions to add.</param>
    public void UnionWith(IEnumerable<EpsilonTransition> transitions)
    {
        MaxState = MaxState = transitions.Any() ? Math.Max(MaxState, transitions.Max(t => Math.Max(t.FromState, t.ToState))) : MaxState;
        epsilonTransitions.UnionWith(transitions);
    }

    /// <summary>
    /// Adds a sequence of symbols to be accepted by the NFA.
    /// </summary>
    /// <remarks>
    /// Any missing symbols in the alphabet will be added to the alphabet.
    /// </remarks>
    /// <param name="sequence">Sequence of symbols to add.</param>
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
    /// <param name="sequences">Sequences to add to the NFA.</param>
    public void UnionWith(IEnumerable<IEnumerable<string>> sequences)
    {
        foreach (IEnumerable<string> sequence in sequences)
            UnionWith(sequence);
    }

    /// <summary>
    /// Adds or removes a state from a set based on a condition.
    /// </summary>
    /// <param name="condition">If <see langword="true"/>, the state is added; otherwise, it is removed.</param>
    /// <param name="state">State to add or remove.</param>
    /// <param name="set">Set to modify.</param>
    /// <exception cref="ArgumentException">Thrown when the set contains negative states.</exception>
    private void IncludeIff(bool condition, int state, HashSet<int> set)
    {
        if (state < 0)
            throw new ArgumentException("Negative states are not allowed.");
        if (condition)
        {
            set.Add(VerifyAndUpdateMaxState(state));
        }
        else set.Remove(state);
    }

    /// <summary>
    /// Adds or removes multiple states from a set based on a condition.
    /// </summary>
    /// <param name="condition">If <see langword="true"/>, the states are added; otherwise, they are removed.</param>
    /// <param name="states">States to add or remove.</param>
    /// <param name="set">Set to modify.</param>
    /// <exception cref="ArgumentException">Thrown when the set contains negative states.</exception>
    private void IncludeIff(bool condition, IEnumerable<int> states, HashSet<int> set)
    {
        if (states.Any(s => s < 0))
            throw new ArgumentException("Negative states are not allowed.");
        if (condition)
        {
            foreach (int state in states)
                set.Add(VerifyAndUpdateMaxState(state));
        }
        else set.ExceptWith(states);
    }

    /// <summary>
    /// Asserts that the state is non-negative and updates the maximum state number if the provided state is greater.
    /// </summary>
    /// <param name="state">State to compare and potentially update.</param>
    /// <returns>The input state.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="state"/> is negative.</exception>
    /// <remarks>
    /// This method ensures that the state is valid (non-negative) and updates the <see cref="MaxState"/> if the provided state exceeds the current maximum state.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int VerifyAndUpdateMaxState(int state)
    {
        state.ShouldNotBeNegative();
        MaxState = Math.Max(MaxState, state);
        return state;
    }

    /// <summary>
    /// Sets the specified state as an initial state or removes it from the initial states.
    /// </summary>
    /// <param name="state">State to set or remove as an initial state.</param>
    /// <param name="initial">If <see langword="true"/>, the state is added to the initial states; otherwise, it is removed.</param>
    public void SetInitial(int state, bool initial = true) => IncludeIff(initial, state, initialStates);

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">State to set or remove as a final state.</param>
    /// <param name="final">If <see langword="true"/>, the state is added to the final states; otherwise, it is removed.</param>
    public void SetFinal(int state, bool final = true) => IncludeIff(final, state, finalStates);

    /// <summary>
    /// Sets the specified states as initial states or removes them from the initial states.
    /// </summary>
    /// <param name="states">States to set or remove as initial states.</param>
    /// <param name="initial">If <see langword="true"/>, the states are added to the initial states; otherwise, they are removed.</param>
    public void SetInitial(IEnumerable<int> states, bool initial = true) => IncludeIff(initial, states, initialStates);

    /// <summary>
    /// Sets the specified states as final states or removes them from the final states.
    /// </summary>
    /// <param name="states">States to set or remove as final states.</param>
    /// <param name="final">If <see langword="true"/>, the states are added to the final states; otherwise, they are removed.</param>
    public void SetFinal(IEnumerable<int> states, bool final = true) => IncludeIff(final, states, finalStates);

    /// <summary>
    /// Clears all initial states.
    /// </summary>
    public void ClearInitialStates() => initialStates.Clear();

    /// <summary>
    /// Clears all final states.
    /// </summary>
    public void ClearFinalStates() => finalStates.Clear();

    /// <summary>
    /// Clears all states and transitions. The resulting NFA will be equivalent to the empty language (∅).
    /// </summary>
    /// <remarks>
    /// The alphabet is not cleared.
    /// </remarks>
    public void ClearAll()
    {
        initialStates.Clear();
        finalStates.Clear();
        transitions.Clear();
        epsilonTransitions.Clear();
        MaxState = Constants.InvalidState;
    }

    #endregion Mutators

    #region Misc Accessors

    /// <summary>
    /// Returns a canonical string representation of the DFA's data.
    /// Used by unit tests and for debugging. 
    /// </summary>
    public override string ToCanonicalString()
    {
        StringBuilder sb = new();
        sb.Append($"I#= {InitialStates.Count}");
        if (initialStates.Count > 0)
            sb.Append($": [{string.Join(", ", initialStates)}]");

        sb.Append($", F#={finalStates.Count}");

        if (finalStates.Count > 0)
            sb.Append($": [{string.Join(", ", finalStates)}]");

        sb.Append($", T#={transitions.Count + epsilonTransitions.Count}");
        if (transitions.Count > 0 || epsilonTransitions.Count > 0)
        {
            sb.Append(": [");
            if (transitions.Count > 0)
                sb.AppendJoin(", ", transitions.Select(t => $"{t.FromState}->{t.ToState} {Alphabet[t.Symbol]}"));
            
            if (epsilonTransitions.Count > 0)
            {
                if (transitions.Count > 0) sb.Append(", ");
                sb.AppendJoin(", ", epsilonTransitions.Select(t => $"{t.FromState}->{t.ToState} {EpsilonTransition.Epsilon}"));
            }
            sb.Append(']');
        }
        return sb.ToString();
    }

    #endregion Misc Accessors
}
