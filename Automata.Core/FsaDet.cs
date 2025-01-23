using System.Diagnostics.Metrics;

namespace Automata.Core;

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
    /// Range of states in this automaton.
    /// <para>Any value from <see cref="Range.End"/> and higher is guaranteed to be free and not used by the automaton.</para>
    /// </summary>
    public abstract Range StateRange { get; }

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
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fromState"/> is negative.</exception>
    /// <returns>A <see cref="StateView"/> representing the state view from the specified state.</returns>
    public abstract StateView State(int fromState);

    /// <summary>
    /// Returns the state reachable from the given state on the given symbol.
    /// </summary>
    /// <param name="fromState">The state origin of the transition.</param>
    /// <param name="symbol">Symbol for the transition.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fromState"/> is negative.</exception>
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
    /// Epsilon transitions of the MFA, which is always empty.
    /// </summary>
    /// <returns>An empty collection of <see cref="EpsilonTransition"/>.</returns>
    public override IReadOnlyCollection<EpsilonTransition> EpsilonTransitions() => Array.Empty<EpsilonTransition>();

    /// <summary>
    /// Indicates whether the automaton accepts the given sequence of symbols as integers.
    /// </summary>
    /// <param name="sequence">Sequence of symbols to check.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the automaton accepts the sequence.
    /// </returns>
    /// <remarks>
    /// The automaton processes each symbol in the sequence, transitioning between states according to its transition function.
    /// If the automaton reaches a final state after processing all symbols, the sequence is accepted.
    /// </remarks>
    public bool Accepts(IEnumerable<int> sequence)
    {
        int state = InitialState;
        foreach (int symbol in sequence)
        {
            if (state == Constants.InvalidState)
                break;

            state = Transition(state, symbol);
        }
        return IsFinal(state);
    }


    /// <summary>
    /// Indicates whether the automaton accepts the given sequence of symbols as strings.
    /// </summary>
    /// <param name="sequence">Sequence of symbols to check.</param>
    /// <returns>
    /// <see langword="true"/> <c>iff</c> the automaton accepts the sequence.
    /// </returns>
    /// <remarks>
    /// The automaton processes each symbol in the sequence, transitioning between states according to its transition function.
    /// If the automaton reaches a final state after processing all symbols, the sequence is accepted.
    /// </remarks>
    public bool Accepts(IEnumerable<string> sequence)
        => Accepts(sequence.Select(symbol => Alphabet[symbol]));

    /// <summary>
    /// Sequence of states visited by the automaton for the given input sequence as integers.
    /// </summary>
    /// <param name="sequence">Sequence of input symbols.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of state indices representing the path taken by the automaton.
    /// </returns>
    /// <remarks>
    /// The method yields each state as it is visited. The result sequence may be partial, if the entire input is not accepted by this automaton.
    /// For a complete path, the number of states is sequence.Length + 1, with the last state being a final state.
    /// </remarks>
    public IEnumerable<int> StatePath(IEnumerable<int> sequence)
    {
        int state = InitialState;
        if (state == Constants.InvalidState)
            yield break;

        yield return state;
        
        foreach (int symbol in sequence)
        {
            state = Transition(state, symbol);

            if (state == Constants.InvalidState)
                yield break;

            yield return state;
        }
        
    }

    /// <summary>
    /// Sequence of states visited by the automaton for the given input sequence as string.
    /// </summary>
    /// <param name="sequence">Sequence of input symbols.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of state indices representing the path taken by the automaton.
    /// </returns>
    /// <remarks>
    /// The method yields each state as it is visited. The result sequence may be partial, if the entire input is not accepted by this automaton.
    /// </remarks>
    public IEnumerable<int> StatePath(IEnumerable<string> sequence)
        => StatePath(sequence.Select(symbol => Alphabet[symbol]));

    #endregion Accessors
}
