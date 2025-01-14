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
    public bool IsEmptyLanguage => InitialState == Constants.InvalidState;

  
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


   
}
