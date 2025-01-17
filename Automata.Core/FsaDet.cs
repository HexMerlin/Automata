﻿namespace Automata.Core;

/// <summary>
/// Common base class for deterministic automata, such as <see cref="Dfa"/> and <see cref="Mfa"/>.
/// </summary>
public abstract class FsaDet : Fsa
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the automaton.</param>
    public FsaDet(Alphabet alphabet) : base(alphabet) { }

    #endregion Constructors

    #region Accessors

    /// <summary>
    /// Indicates whether the MFA is epsilon-free. Always returns <see langword="true"/>.
    /// </summary>
    public override bool IsEpsilonFree => true;

    /// <summary>
    /// Initial state of the deterministic automaton.
    /// </summary>
    public abstract int InitialState { get; }

    /// <summary>
    /// Upper limit for the maximum state number in the DFA. 
    /// <para>A value (<see cref="MaxState"/> + 1) is guaranteed to be an unused state number.</para>
    /// </summary>
    public abstract int MaxState { get; }
    
    /// <summary>
    /// Indicates whether the automaton accepts ϵ - the empty sting. 
    /// <para>Returns <see langword="true"/> <c>iff</c> an InitialState exists and it is also a final state.</para>
    /// </summary>
    public override bool AcceptsEpsilon => IsFinal(InitialState);

    /// <summary>
    /// Number of transitions in the automaton.
    /// </summary>
    public abstract int TransitionCount { get; }

    /// <summary>
    /// Indicates whether the automaton has an initial state.
    /// </summary>
    /// <returns><see langword="true"/> <c>iff</c> MFA has an initial state.</returns>
    public override bool HasInitialState => InitialState != Constants.InvalidState;

    /// <summary>
    /// Indicates whether the specified state is the initial state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is the initial state.</returns>
    public override bool IsInitial(int state) => state == InitialState;

    /// <summary>
    /// Returns a readonly view of the specified state.
    /// </summary>
    /// <param name="fromState">State from which to get the state view.</param>
    /// <returns>A <see cref="StateView"/> representing the state view from the specified state.</returns>
    public abstract StateView State(int fromState);

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <param name="symbol">Symbol for the transition.</param>
    /// <returns>
    /// The state reachable from the given state on the given symbol. If no such transition exists, <see cref="Constants.InvalidState"/> is returned.
    /// </returns>
    /// <seealso cref="TryTransition(int, int, out int)"/>
    public abstract int Transition(int fromState, int symbol);

    /// <summary>
    /// Tries to get the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <param name="symbol">Symbol for the transition.</param>
    /// <param name="toState">The reachable state, or <see cref="Constants.InvalidState"/> if no state is reachable.</param>
    /// <returns><see langword="true"/> <c>iff</c> a reachable state exists.</returns>
    /// <seealso cref="Transition(int, int)"/>
    public bool TryTransition(int fromState, int symbol, out int toState)
        => (toState = Transition(fromState, symbol)) != Constants.InvalidState;

    /// <summary>
    /// Gets the epsilon transitions of the MFA, which is always empty.
    /// </summary>
    /// <returns>An empty collection of <see cref="EpsilonTransition"/>.</returns>
    public override IReadOnlyCollection<EpsilonTransition> EpsilonTransitions() => Array.Empty<EpsilonTransition>();

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

            if (!TryTransition(state, symbolIndex, out state))
                return false;

        }
        return IsFinal(state);
    }

    #endregion Accessors
}
