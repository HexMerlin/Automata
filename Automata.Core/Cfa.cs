using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics;
using Automata.Core.Operations;

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
/// <item>Performance-optimized for efficient read-only operations. Minimal memory footprint.</item>
/// </list>
/// <para>Key attribute for CFAs:</para>
/// For any language, there exists exactly one specific <see cref="Cfa"/>.
/// <para>Any two automata that accept the same language will, when converted, yield two <see cref="Cfa"/> that are precisely identical in all aspects.</para>
/// </remarks>
public partial class Cfa : IEquatable<Cfa>, IEnumerable<Transition>, IDfa
{
    #region Data
    /// <summary>
    /// Alphabet used by the CFA.
    /// </summary>
    public CanonicalAlphabet Alphabet { get; }

    private readonly Transition[] transitions;

    /// <summary>
    /// Initial state. Always <c>0</c> for a non-empty <see cref="Cfa"/>. 
    /// <para>For an empty <see cref="Cfa"/>, the initial state is <see cref="Constants.InvalidState"/>.</para>
    /// </summary>
    public int InitialState { get; }

    /// <summary>
    /// Final states of the CFA.
    /// </summary>
    public readonly FrozenSet<int> FinalStates;

    /// <summary>
    /// Number of states in the CFA.
    /// </summary>
    public readonly int StateCount;

    #endregion Data

    /// <summary>
    /// Initializes a new instance of the <see cref="Cfa"/> class from an existing <see cref="Dfa"/>.
    /// </summary>
    /// <param name="dfa">A DFA to create from.</param>
    public Cfa(Dfa dfa) : this(ConvertDfa(dfa)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cfa"/> class with the specified parameters.
    /// </summary>
    /// <param name="p">A tuple containing the canonical alphabet, transitions, initial state, and final states.</param>
    private Cfa((CanonicalAlphabet alphabet, IEnumerable<Transition> transitions, int initialState, FrozenSet<int> finalStates) p)
    {
        Alphabet = p.alphabet;
        this.transitions = p.transitions.OrderBy(t => t).ToArray();
        this.StateCount = 1 + MaxStateAndAssert(this.transitions);

        InitialState = p.initialState;
        FinalStates = p.finalStates;
        Debug.Assert(InitialState <= 0); //0 if non-empty, Constants.InvalidState if empty
    }

    /// <summary>
    /// Indicates whether the CFA is empty.
    /// </summary>
    public bool IsEmpty => InitialState == Constants.InvalidState;

    /// <summary>
    /// Indicates whether the DFA is epsilon-free. Always returns <see langword="true"/>.
    /// </summary>
    public bool IsEpsilonFree => true;

    /// <summary>
    /// Number of transitions in the automaton.
    /// </summary>
    public int TransitionCount => transitions.Length;

    /// <summary>
    /// Alphabet used by the FSA.
    /// </summary>
    IAlphabet IFsa.Alphabet => Alphabet;

    ///<inheritdoc/>
    IReadOnlySet<int> IDfa.FinalStates => this.FinalStates;

    /// <summary>
    /// The state number with the highest value.
    /// Equivalent to <c><see cref="StateCount"/> - 1</c>.
    /// </summary>
    /// <returns>The maximum state number, or <c>-1</c> if the CFA is empty.</returns>
    public int MaxState => StateCount - 1;

    /// <summary>
    /// Indicates whether the specified state is the initial state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is the initial state.</returns>
    public bool IsInitial(int state) => state == InitialState;

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is a final state.</returns>
    public bool IsFinal(int state) => FinalStates.Contains(state);

    /// <summary>
    /// Returns the transition from the given state with the given symbol.
    /// </summary>
    /// <param name="fromState">Source state.</param>
    /// <param name="symbol">Symbol of the transition.</param>
    /// <returns>
    /// The transition from the given state with the given symbol, or <see cref="Transition.Invalid"/> if no such transition exists.
    /// </returns>
    public Transition Transition(int fromState, int symbol)
    {
        int index = Array.BinarySearch(transitions, new Transition(fromState, symbol, Constants.InvalidState));
        Debug.Assert(index < 0, $"Binary search returned a non-negative index ({index}), which should be impossible given the search key.");
        index = ~index; // Get the insertion point
        return (index < transitions.Length && transitions[index].FromState == fromState && transitions[index].Symbol == symbol)
            ? transitions[index]
            : Core.Transition.Invalid;
    }

    /// <summary>
    /// Returns a <see cref="StateView"/> for the given state.
    /// </summary>
    /// <param name="state">State.</param>
    /// <returns>A <see cref="StateView"/> containing the transitions from the given state.</returns>
    public StateView State(int state) => new(state, transitions);

    /// <summary>
    /// Gets an enumerable collection of the symbolic transitions.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Transition"/>.</returns>
    public IEnumerable<Transition> SymbolicTransitions() => this;

    /// <summary>
    /// Gets an enumerable collection of the epsilon transitions, which is always empty.
    /// </summary>
    /// <returns>An empty enumerable collection of <see cref="EpsilonTransition"/>.</returns>
    public IEnumerable<EpsilonTransition> EpsilonTransitions() => Array.Empty<EpsilonTransition>();



    /// <summary>
    /// Converts an FSA to a canonical form.
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <returns>A tuple containing the canonical alphabet, transitions, initial state, and final states.</returns>
    private static (CanonicalAlphabet alphabet, Transition[] transitions, int initialState, FrozenSet<int> finalStates) ConvertDfa(Dfa dfa)
    {
        Dfa minDfa = Ops.Minimal(dfa);
        if (minDfa.IsEmpty)
            return (CanonicalAlphabet.Empty, Array.Empty<Transition>(), Constants.InvalidState, FrozenSet<int>.Empty);

        CanonicalAlphabet alphabet = new CanonicalAlphabet(minDfa.SymbolicTransitions().Select(t => minDfa.Alphabet[t.Symbol]));
        (Transition[] transitions, int initialState, FrozenSet<int> finalStates) = ConvertWithAlphabet(minDfa, alphabet);
        return (alphabet, transitions, initialState, finalStates);
    }

    /// <summary>
    /// Converts a minimized DFA to a canonical form.
    /// </summary>
    /// <param name="minDfa">Minimized DFA to convert.</param>
    /// <param name="cAlphabet">Canonical alphabet to use for the conversion.</param>
    /// <returns>A tuple containing the transitions, initial state, and final states of the canonical form.</returns>
    private static (Transition[] transitions, int initialState, FrozenSet<int> finalStates) ConvertWithAlphabet(Dfa minDfa, CanonicalAlphabet cAlphabet)
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
    /// Finds the maximum state in the set of transitions.
    /// Also asserts that the transitions are deterministic.
    /// </summary>
    /// <param name="transitions">Transition array.</param>
    /// <returns>Maximum state referenced, or <see cref="Constants.InvalidState"/> if the array is empty.</returns>
    /// <exception cref="ArgumentException">If the transitions are not deterministic.</exception>
    private static int MaxStateAndAssert(Transition[] transitions)
    {
        int maxState = Constants.InvalidState;
        int fromState = Constants.InvalidState;
        int symbol = Constants.InvalidSymbolIndex;

        for (int i = 0; i < transitions.Length; i++)
        {
            Transition t = transitions[i];
            if (t.FromState == fromState && t.Symbol == symbol)
                throw new ArgumentException("The transitions must be deterministic: every (FromState, Symbol)-tuple must be unique.");
            (fromState, symbol, int toState) = t;
            maxState = Math.Max(maxState, Math.Max(fromState, toState));
        }
        return maxState;
    }


    /// <summary>
    /// Indicates language equivalence between two CFAs.
    /// </summary>
    /// <param name="other">Object to compare with this object.</param>
    /// <remarks>Equality means both CFAs accept the same language, but will due to the canonical property, also be identical.</remarks>
    /// <returns><see langword="true"/> <c>iff</c> the current CFA is equal to <paramref name="other"/>.</returns>
    public bool Equals(Cfa? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (InitialState != other.InitialState) return false;
        if (!FinalStates.SetEquals(other.FinalStates)) return false;
        if (!Alphabet.Equals(other.Alphabet)) return false;
        if (!this.SequenceEqual(other)) return false;
        return true;
    }

    ///<inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Cfa);

    /// <summary>
    /// Hash code for the current alphabet.
    /// </summary>
    /// <returns>A hash code for the alphabet.</returns>
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(InitialState);
        hash.Add(FinalStates);
        hash.Add(Alphabet);
        foreach (Transition t in this)
            hash.Add(t);
        return hash.ToHashCode();
    }

    /// <summary>
    /// Indicates whether two specified instances of <see cref="Cfa"/> are equal.
    /// </summary>
    /// <param name="left">First <see cref="Cfa"/> to compare.</param>
    /// <param name="right">Second <see cref="Cfa"/> to compare.</param>
    /// <returns><see langword="true"/> <c>iff</c> the two <see cref="Cfa"/> instances are equal.</returns>
    public static bool operator ==(Cfa left, Cfa right) => left.Equals(right);

    /// <summary>
    /// Indicates whether two specified instances of <see cref="Cfa"/> are not equal.
    /// </summary>
    /// <param name="left">First <see cref="Cfa"/> to compare.</param>
    /// <param name="right">Second <see cref="Cfa"/> to compare.</param>
    /// <returns><see langword="false"/> <c>iff</c> the two <see cref="Cfa"/> instances are not equal.</returns>
    public static bool operator !=(Cfa left, Cfa right) => !left.Equals(right);

    /// <summary>
    /// Returns an enumerator that iterates through the transitions.
    /// </summary>
    /// <returns>An enumerator for the transitions.</returns>
    public IEnumerator<Transition> GetEnumerator() => ((IEnumerable<Transition>)this.transitions).GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the transitions.
    /// </summary>
    /// <returns>An enumerator for the transitions.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.transitions.GetEnumerator();
}

