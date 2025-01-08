﻿using System.Runtime.CompilerServices;

namespace Automata.Core;

/// <summary>
/// Represents a deterministic finite automaton (DFA).
/// </summary>
/// <remarks>
/// A DFA is a finite state machine that accepts or rejects finite sequences of symbols.
/// It is always deterministic and does not contain epsilon transitions.
/// </remarks>
public class Dfa : IDfa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the DFA.
    /// </summary>
    public MutableAlphabet Alphabet { get; }

    private readonly SortedSet<Transition> orderByFromState = new();

    /// <summary>
    /// Gets the initial state of the DFA.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="Constants.InvalidState"/> if the DFA has no initial state.
    /// </remarks>
    public int InitialState { get; private set; } = Constants.InvalidState;

    private readonly HashSet<int> finalStates = new();

    /// <summary>
    /// Gets an upper bound for the maximum state number in the DFA.
    /// </summary>
    /// <remarks>
    /// This value represents an upper limit for state numbers in the DFA.
    /// The actual maximum state number may be lower, as removed states are not tracked for performance reasons.
    /// </remarks>
    public int MaxState { get; private set; } = Constants.InvalidState;
    #endregion Data

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with an empty alphabet.
    /// </summary>
    public Dfa() : this(new MutableAlphabet()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    public Dfa(MutableAlphabet alphabet) => Alphabet = alphabet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet, transitions, initial state, and final states.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    /// <param name="transitions">The transitions of the DFA.</param>
    /// <param name="initialState">The initial state of the DFA.</param>
    /// <param name="finalStates">The final states of the DFA.</param>
    internal Dfa(MutableAlphabet alphabet, IEnumerable<Transition> transitions, int initialState, IEnumerable<int> finalStates) : this(alphabet)
    {
        SetInitial(initialState);
        SetFinal(finalStates);
        this.finalStates.UnionWith(finalStates);
        UnionWith(transitions);
    }

    /// <summary>
    /// Gets a value indicating whether the DFA is empty. A DFA is considered empty if it has no initial state.
    /// </summary>
    public bool IsEmpty => InitialState == Constants.InvalidState;

    /// <summary>
    /// Indicates whether the DFA is epsilon-free. Always returns <see langword="true"/>.
    /// </summary>
    public bool IsEpsilonFree => true;

    /// <summary>
    /// Sets the initial state of the DFA, updating the maximum state number if necessary.
    /// </summary>
    /// <param name="state">The state to set as the initial state.</param>
    public void SetInitial(int state) => InitialState = UpdateMaxState(state);

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">The state to set or remove as a final state.</param>
    /// <param name="final">
    /// If <see langword="true"/>, the state is added to the final states; otherwise, it is removed.
    /// </param>
    public void SetFinal(int state, bool final = true) => IncludeIf(final, state, finalStates);

    /// <summary>
    /// Sets the specified states as final states or removes them from the final states.
    /// </summary>
    /// <param name="states">The states to set or remove as final states.</param>
    /// <param name="final">
    /// If <see langword="true"/>, the states are added to the final states; otherwise, they are removed.
    /// </param>
    public void SetFinal(IEnumerable<int> states, bool final = true) => IncludeIf(final, states, finalStates);

    /// <summary>
    /// Indicates whether the specified state is the initial state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>
    /// <see langword="true"/> if the specified state is the initial state; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsInitial(int state) => state == InitialState;

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>
    /// <see langword="true"/> if the specified state is a final state; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsFinal(int state) => finalStates.Contains(state);

    /// <summary>
    /// Gets the final states of the DFA.
    /// </summary>
    public IReadOnlySet<int> FinalStates => finalStates;

    /// <summary>
    /// Adds a transition to the DFA, ensuring it remains deterministic.
    /// </summary>
    /// <remarks>
    /// If adding the transition would introduce nondeterminism (i.e., a transition with the same from-state and symbol already exists), the new transition will not be added.
    /// </remarks>
    /// <param name="transition">The transition to add.</param>
    /// <returns>
    /// <see langword="true"/> if the transition was added; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Add(Transition transition)
    {
        if (ReachableState(transition.FromState, transition.Symbol) != Constants.InvalidState)
            return false; // Cannot add; would introduce nondeterminism
        return orderByFromState.Add(UpdateMaxState(transition));
    }

    /// <summary>
    /// Adds all provided transitions that are currently not present in the set.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void UnionWith(IEnumerable<Transition> transitions)
    {
        foreach (Transition transition in transitions)
            Add(transition);
    }

    /// <summary>
    /// Returns the transition from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
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
    /// <param name="fromState">The state from which to start.</param>
    /// <returns>The set of transitions from the given state.</returns>
    public SortedSet<Transition> Transitions(int fromState)
        => orderByFromState.GetViewBetween(
            Core.Transition.MinTrans(fromState),
            Core.Transition.MaxTrans(fromState)
        );

    /// <summary>
    /// Returns a <see cref="StateView"/> of the given state.
    /// </summary>
    /// <param name="fromState">The from state.</param>
    /// <remarks>
    /// This method provides a read-only view of the state transitions from the specified state.
    /// It is primarily for compatibility with contiguous memory representations like <see cref="Cfa"/>.
    /// When possible, use <see cref="Transitions(int)"/>, which avoids memory allocation and has less overhead.
    /// </remarks>
    /// <returns>A <see cref="StateView"/> for the given state.</returns>
    public StateView State(int fromState)
        => new StateView(fromState, Transitions(fromState).ToArray());

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state from which to start.</param>
    /// <param name="symbol">The symbol to transition on.</param>
    /// <returns>
    /// The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.
    /// </returns>
    public int ReachableState(int fromState, int symbol)
        => orderByFromState.GetViewBetween(
            Core.Transition.MinTrans(fromState, symbol),
            Core.Transition.MaxTrans(fromState, symbol)
        ).FirstOrDefault(Core.Transition.Invalid).ToState;

    /// <inheritdoc/>
    IAlphabet IFsa.Alphabet => Alphabet;

    /// <summary>
    /// Gets the transitions of the DFA.
    /// </summary>
    /// <returns>An enumerable collection of transitions.</returns>
    public IEnumerable<Transition> SymbolicTransitions() => orderByFromState;

    /// <summary>
    /// Gets the epsilon transitions of the DFA, which is always empty.
    /// </summary>
    /// <returns>An empty enumerable collection of <see cref="EpsilonTransition"/>.</returns>
    public IEnumerable<EpsilonTransition> EpsilonTransitions() => Array.Empty<EpsilonTransition>();

    /// <summary>
    /// Creates a new NFA that recognizes the reverse of the language accepted by this DFA.
    /// </summary>
    /// <returns>An NFA representing the reversed DFA.</returns>
    public Nfa Reversed() => new(this, applyReverseOperation: true);

    /// <summary>
    /// Minimizes the DFA using Brzozowski's algorithm.
    /// </summary>
    /// <remarks>
    /// This algorithm involves reversing the DFA, determinizing it, reversing it again, and determinizing it once more.
    /// </remarks>
    /// <returns>A minimized DFA.</returns>
    public Dfa Minimized()
    {
        Dfa reversed = Reversed().ToDfa();
        return reversed.Reversed().ToDfa();
    }

    /// <summary>
    /// Converts the DFA to a canonical finite automaton (CFA).
    /// </summary>
    /// <returns>A CFA representing the DFA.</returns>
    public Cfa ToCFA() => new(this);

    /// <summary>
    /// Converts the DFA to a nondeterministic finite automaton (NFA).
    /// </summary>
    /// <returns>An NFA representing the DFA.</returns>
    public Nfa ToNFA() => new(this);

    /// <summary>
    /// Indicates whether the DFA accepts the given sequence of symbols.
    /// </summary>
    /// <param name="sequence">The sequence of symbols to check.</param>
    /// <returns>
    /// <see langword="true"/> if the DFA accepts the sequence; otherwise, <see langword="false"/>.
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
    /// <param name="state">The state to include or exclude.</param>
    /// <param name="set">The set to modify.</param>
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
    /// <param name="states">The states to include or exclude.</param>
    /// <param name="set">The set to modify.</param>
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
    /// <param name="state">The state to compare and potentially update.</param>
    /// <returns>The input state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int UpdateMaxState(int state)
    {
        MaxState = Math.Max(MaxState, state);
        return state;
    }

    /// <summary>
    /// Updates the maximum state number based on the from and to states of the provided transition.
    /// </summary>
    /// <param name="transition">The transition to evaluate.</param>
    /// <returns>The input transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Transition UpdateMaxState(Transition transition)
    {
        MaxState = Math.Max(MaxState, Math.Max(transition.FromState, transition.ToState));
        return transition;
    }
}
