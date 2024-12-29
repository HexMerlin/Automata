using System.Collections.Frozen;
using System.Diagnostics;
using Automata.Core.Alphabets;
using Automata.Core.TransitionSets;

namespace Automata.Core;

/// <summary>
/// Canonical Finite-state Automaton (CFA).
/// </summary>
/// <remarks>
/// The <see cref="Cfa"/> is the most optimized automaton representation, characterized by:
/// <list type="number">
/// <item><c>Deterministic</c> and <c>Minimal</c>: The least possible states and transitions (Similarly to a minimized DFA).</item>
/// <item>Canonical alphabet: Reduced, contiguous, and lexicographically ordered.</item>
/// <item>Canonical states and transitions: contiguously indexed, optimized, and fully ordered.</item>
/// <item><c>Immutable</c>: Guarantees structural and behavioral invariance.</item>
/// <item>Performance-optimized for efficient read-only operations.</item>
/// </list>
/// For any language, the <see cref="Cfa"/> is unique, embodying its minimal deterministic automaton in canonical form.
/// <para>Any two <see cref="Cfa"/> instances accepting the same language are identical.</para>
/// </remarks>
public class Cfa : ImmutableTransitions, IFsa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the CFA.
    /// </summary>
    public IAlphabet Alphabet { get; }

    /// <summary>
    /// The initial state. Always <c>0</c> for a non-empty <see cref="Cfa"/>. 
    /// <para>For an empty <see cref="Cfa"/>, the initial state is <see cref="Constants.InvalidState"/>.</para>
    /// </summary>
    public int InitialState { get; }

    /// <summary>
    /// Gets the final states of the CFA.
    /// </summary>
    public readonly FrozenSet<int> FinalStates;

    #endregion Data

    /// <summary>
    /// Initializes a new instance of the <see cref="Cfa"/> class from an existing FSA.
    /// </summary>
    /// <param name="fsa">The finite state automaton to convert.</param>
    public Cfa(IFsa fsa) : this(Convert(fsa)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cfa"/> class with the specified parameters.
    /// </summary>
    /// <param name="p">A tuple containing the canonical alphabet, transitions, initial state, and final states.</param>
    private Cfa((CanonicalAlphabet alphabet, IEnumerable<Transition> transitions, int initialState, FrozenSet<int> finalStates) p) : base(p.transitions)
    {
        Alphabet = p.alphabet;
        InitialState = p.initialState;
        FinalStates = p.finalStates;
        Debug.Assert(InitialState <= 0); //0 if non-empty, Constants.InvalidState if empty
    }

    ///<inheritdoc/>
    public bool IsEmpty => InitialState == Constants.InvalidState;

    /// <summary>
    /// Indicates whether the DFA is epsilon-free. Always returns <see langword="true"/>.
    /// </summary>
    public bool IsEpsilonFree => true;

    /// <summary>
    /// Indicates whether the specified state is the initial state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is the initial state; otherwise, <c>false</c>.</returns>
    public bool IsInitial(int state) => state == InitialState;

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is a final state; otherwise, <c>false</c>.</returns>
    public bool IsFinal(int state) => FinalStates.Contains(state);

    /// <summary>
    /// Gets the transitions of the DFA.
    /// </summary>
    public IEnumerable<Transition> SymbolicTransitions() => this;

    /// <summary>
    /// Gets the epsilon transitions of the DFA, which is always empty.
    /// </summary>
    public IEnumerable<EpsilonTransition> EpsilonTransitions() => Array.Empty<EpsilonTransition>();

    /// <summary>
    /// Converts an FSA to a canonical form.
    /// </summary>
    /// <param name="fsa">The finite state automaton to convert.</param>
    /// <returns>A tuple containing the canonical alphabet, transitions, initial state, and final states.</returns>
    private static (CanonicalAlphabet alphabet, Transition[] transitions, int initialState, FrozenSet<int> finalStates) Convert(IFsa fsa)
    {
        Dfa minDfa = ToMinimizedDfa(fsa);
        if (minDfa.IsEmpty)
            return (CanonicalAlphabet.Empty, Array.Empty<Transition>(), Constants.InvalidState, FrozenSet<int>.Empty);
       
        CanonicalAlphabet alphabet = new CanonicalAlphabet(minDfa.SymbolicTransitions().Select(t => minDfa.Alphabet[t.Symbol]));
        (Transition[] transitions, int initialState, FrozenSet<int> finalStates) = Convert(minDfa, alphabet);
        return (alphabet, transitions, initialState, finalStates);
    }

    /// <summary>
    /// Converts a minimized DFA to a canonical form.
    /// </summary>
    /// <param name="minDfa">The minimized DFA to convert.</param>
    /// <param name="cAlphabet">The canonical alphabet to use for the conversion.</param>
    /// <returns>A tuple containing the transitions, initial state, and final states of the canonical form.</returns>
    private static (Transition[] transitions, int initialState, FrozenSet<int> finalStates) Convert(Dfa minDfa, CanonicalAlphabet cAlphabet)
    {
        Dictionary<int, int> dStateToCStateMap = new();
        int cMaxState = 0;
        int dFromState = minDfa.InitialState;
        dStateToCStateMap.Add(dFromState, cMaxState);
        Queue<int> dStateQueue = new();
        dStateQueue.Enqueue(dFromState);
        List<Transition> cTransitions = new();

        while (dStateQueue.Count > 0)
        {
            dFromState = dStateQueue.Dequeue();
            int cFromState = dStateToCStateMap[dFromState];

            foreach (Transition dTransition in minDfa.Transitions(dFromState).OrderBy(t => minDfa.Alphabet[t.Symbol], CanonicalAlphabet.CanonicalStringComparer))
            {
                int dToState = dTransition.ToState;
                if (!dStateToCStateMap.TryGetValue(dToState, out int cToState))
                {
                    cToState = ++cMaxState;
                    dStateToCStateMap.Add(dToState, cToState);
                    dStateQueue.Enqueue(dToState);
                }
                int cSymbol = cAlphabet[minDfa.Alphabet[dTransition.Symbol]];
                cTransitions.Add(new Transition(cFromState, cSymbol, cToState));
            }
        }

        int cInitialState = dStateToCStateMap[minDfa.InitialState];
        FrozenSet<int> cFinalStates = minDfa.FinalStates.Select(d => dStateToCStateMap[d]).ToFrozenSet();

        return (cTransitions.ToArray(), cInitialState, cFinalStates);
    }

    /// <summary>
    /// Converts an FSA to a minimized DFA.
    /// </summary>
    /// <param name="fsa">The finite state automaton to convert.</param>
    /// <returns>A minimized DFA.</returns>
    private static Dfa ToMinimizedDfa(IFsa fsa) => fsa switch
    {
        Dfa dfa => dfa.Minimized(),
        Nfa nfa => nfa.ToDfa().Minimized(),
        _ => throw new ArgumentException("Unsupported automaton type", nameof(fsa))
    };

}

