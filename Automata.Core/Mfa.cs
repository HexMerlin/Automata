using System.Diagnostics;
using System.Text;

namespace Automata.Core;

/// <summary>
/// Minimal Finite-state Automaton (MFA).
/// </summary>
/// <remarks>
/// <see cref="Mfa"/> is the most optimized automaton representation, characterized by:
/// <list type="number">
/// <item><c>Deterministic</c> and <c>Minimal</c>: The least possible states and transitions.</item>
/// <item>Minimal memory footprint: Uses a contiguous memory block for data, with minimal overhead.</item>
/// <item>Performance-optimized for efficient readonly operations.</item>
/// <item>Immutable: Guarantees structural and behavioral invariance.</item>
/// <item>Contiguous states: States are in range [0..MaxState].</item>
/// <item>Initial state is always <c>0</c> for a non-empty <see cref="Mfa"/>.</item>
/// <item>Canonical topology: States and transitions are canonically ordered.</item>
/// <item>Two <see cref="Mfa"/>s recognizing the same language are guaranteed to be identical.</item>
/// </list>
/// </remarks>
public class Mfa : FsaDet, IEquatable<Mfa>
{
    #region Data

    /// <summary>
    /// Number of states in the MFA.
    /// </summary>
    public int StateCount { get; }

    private readonly Transition[] transitions;

    /// <summary>
    /// Final states of the MFA.
    /// </summary>
    private readonly int[] finalStates;

    #endregion Data

    #region Constructors

    /// <summary>
    /// Creates a singleton <see cref="Mfa"/> that accepts only the single symbol once.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the MFA.</param>
    /// <param name="singleSymbol">Symbol to be accepted by the MFA.</param>
    public Mfa(string singleSymbol, Alphabet alphabet) : base(alphabet)
    {
        int symbol = alphabet.GetOrAdd(singleSymbol);
        StateCount = 2;
        transitions = [new Transition(InitialState, symbol, MaxState)];
        this.finalStates = [MaxState];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mfa"/> class from an existing <see cref="Dfa"/>.
    /// </summary>
    /// <param name="dfa">A DFA to create from.</param>
    public Mfa(Dfa dfa) : base(dfa.Alphabet)
    {
        dfa = Ops.Minimal(dfa); //make sure the dfa is minimal
        if (dfa.FinalStates.Count == 0)
        {
            StateCount = 0;
            this.transitions = [];
            this.finalStates = [];
            return;
        }
        Dictionary<int, int> dfaStateToMfaStateMap = dfa.StatesToCanonicalStatesMap();
        StateCount = dfaStateToMfaStateMap.Count;
        this.transitions = dfa.Transitions().Select(t => new Transition(dfaStateToMfaStateMap[t.FromState], t.Symbol, dfaStateToMfaStateMap[t.ToState])).ToArray();
        this.finalStates = dfa.FinalStates.Select(s => dfaStateToMfaStateMap[s]).OrderBy(s => s).ToArray();
    }

    /// <summary>
    /// Private constructor
    /// </summary>
    private Mfa(Alphabet alphabet, int stateCount, Transition[] transitions, int[] finalStates) : base(alphabet)
    {
        StateCount = stateCount;
        this.transitions = transitions;
        this.finalStates = finalStates;
    }

    /// <summary>
    /// Creates a <see cref="Mfa"/> that represents the empty language (∅), with a specified alphabet.
    /// The MFA has zero states and zero transitions.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the MFA.</param>
    public static Mfa CreateEmpty(Alphabet alphabet) => new(alphabet, 0, [], []);

    /// <summary>
    /// Returns an automaton that accepts one occurrence of any symbol in the specified alphabet.
    /// It corresponds directly to the "." in Alang expressions.
    /// </summary>
    /// <param name="alphabet">Alphabet containing the set of symbols</param>
    /// <returns>An automaton representing any symbol accepted exactly once.</returns>
    public static Mfa CreateWildcard(Alphabet alphabet)
        => new(alphabet, stateCount: 2, transitions: [.. alphabet.SymbolIndices.Select(symbol => new Transition(0, symbol, 1))], finalStates: [1]);

    #endregion Constructors

    #region Accessors

    /// <summary>
    /// Initial state. Always <c>0</c> for a non-empty <see cref="Mfa"/>. 
    /// <para>For an empty <see cref="Mfa"/>, the initial state is <see cref="Constants.InvalidState"/>.</para>
    /// </summary>
    /// <remarks>
    /// An MFA without an initial state is completely empty (= <see cref="IsEmptyLanguage"/>).
    /// </remarks>
    public override int InitialState => StateCount > 0 ? 0 : Constants.InvalidState;

    /// <summary>
    /// The state number with the highest value.
    /// </summary>
    /// <returns>The maximum state number, or <see cref="Constants.InvalidState"/> (= <c>-1</c>) if the MFA is empty (has no states).</returns>
    public override int MaxState => StateCount - 1;

    /// <summary>
    /// Final states of the MFA.
    /// </summary>
    public override IReadOnlyCollection<int> FinalStates => finalStates;

    /// <summary>
    /// Indicates whether the language of the MFA is the empty language (∅). This means the MFA does not accept anything, including the empty string (ϵ).
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> only if either; the MFA has no states, or the initial state is not a final state.
    /// </remarks>
    public bool IsEmptyLanguage => MaxState == Constants.InvalidState;

    /// <summary>
    /// Number of transitions in the automaton.
    /// </summary>
    public override int TransitionCount => transitions.Length;

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is a final state.</returns>
    public override bool IsFinal(int state) => FinalStates.Contains(state);
    
    /// <summary>
    /// Gets the transitions of the MFA.
    /// </summary>
    /// <returns>An collection of transitions.</returns>
    public override IReadOnlyCollection<Transition> Transitions() => transitions;

    ///<inheritdoc/>
    public override int Transition(int fromState, int symbol)
    {
        int index = Array.BinarySearch(transitions, Core.Transition.MinTrans(fromState, symbol));
        Debug.Assert(index < 0, $"Binary search returned a non-negative index ({index}), which should be impossible given the search key.");
        index = ~index; // Get the insertion point
        return (index < transitions.Length && transitions[index].FromState == fromState && transitions[index].Symbol == symbol)
            ? transitions[index].ToState
            : Constants.InvalidState;
    }

    /// <summary>
    /// Returns a view of the specified state.
    /// </summary>
    /// <param name="fromState">The state origin.</param>
    /// <returns>A <see cref="StateView"/> for the given state.</returns>
    public override StateView State(int fromState) => new(fromState, transitions);
     
    /// <summary>
    /// Indicates whether this MFA represent the exact same language as the specified MFA: <c>Language Equality</c>.
    /// </summary>
    /// <param name="other">MFA to check language equality against.</param>
    /// <remarks>
    /// <c>Language Equality</c> means both MFAs represent the same language. Due to the canonical property, <c>Language Equality</c> also means the MFAs are completely identical (identical states, and identical transition arrays).
    /// <para>The alphabets need however not need to be equal, but every referenced symbol must have the same index in both alphabets.</para>
    /// </remarks>
    /// <returns><see langword="true"/> <c>iff</c> the current Mfa represents the same language as<paramref name="other"/>.</returns>
    public bool LanguageEquals(Mfa other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (!finalStates.AsSpan().SequenceEqual(other.finalStates)) return false;
        if (!transitions.AsSpan().SequenceEqual(other.transitions)) return false;
        if (!Alphabet.Equals(other.Alphabet))
        {
            for (int i = 0; i < transitions.Length; i++) //check that all symbols have the same index in both alphabets
            {
                Transition t = transitions[i];
                if (Alphabet[t.Symbol] != other.Alphabet[t.Symbol])
                    return false;
            }
        }
        return true;
    }

    ///<inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Mfa);

    /// <summary>
    /// Indicates whether this MFA is equal to the specified MFA.
    /// </summary>
    /// <param name="other">MFA to compare with.</param>
    /// <remarks>
    /// This method is similar to <see cref="LanguageEquals(Mfa)"/> but is stricter:
    /// <para>It also requires the alphabets of both MFAs to be equal (not just the referenced symbols).</para>
    /// </remarks>
    /// <seealso cref="LanguageEquals(Mfa)"/>
    /// <returns><see langword="true"/> <c>iff</c> the current Mfa is equal to <paramref name="other"/>.</returns>
    public bool Equals(Mfa? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (!finalStates.AsSpan().SequenceEqual(other.finalStates)) return false;
        if (!transitions.AsSpan().SequenceEqual(other.transitions)) return false;
        if (!Alphabet.Equals(other.Alphabet)) return false;
        return true;
    }

    /// <summary>
    /// Indicates whether two specified instances of <see cref="Mfa"/> are equal.
    /// </summary>
    /// <param name="left">First <see cref="Mfa"/> to compare.</param>
    /// <param name="right">Second <see cref="Mfa"/> to compare.</param>
    /// <returns><see langword="true"/> <c>iff</c> the two <see cref="Mfa"/> instances are equal.</returns>
    public static bool operator ==(Mfa left, Mfa right) => left.Equals(right);

    /// <summary>
    /// Indicates whether two specified instances of <see cref="Mfa"/> are not equal.
    /// </summary>
    /// <param name="left">First <see cref="Mfa"/> to compare.</param>
    /// <param name="right">Second <see cref="Mfa"/> to compare.</param>
    /// <returns><see langword="false"/> <c>iff</c> the two <see cref="Mfa"/> instances are not equal.</returns>
    public static bool operator !=(Mfa left, Mfa right) => !left.Equals(right);

    #endregion Accessors

    #region Misc Accessors

    /// <summary>
    /// Hash code for the current MFA.
    /// </summary>
    /// <returns>A hash code for the MFA.</returns>
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Alphabet);
        hash.Add(InitialState);
        hash.Add(FinalStates);
        for (int i = 0; i < transitions.Length; i++)
            hash.Add(transitions[i]);
        return hash.ToHashCode();
    }

    /// <summary>
    /// Returns a canonical string representation of the MFA's data.
    /// Used by unit tests and for debugging. 
    /// </summary>
    /// <example>
    /// Canonical string for a 2-state MFA, with 2 states, 2 transitions and 1 final state, that accepts the language {a | b}:
    /// <code>
    /// S#=2, F#=1: [1]: T#=2: [0->1 a, 0->1 b]
    /// </code>
    /// </example>
    public override string ToCanonicalString()
    {
        StringBuilder sb = new();
        sb.Append($"S#={StateCount}");
        sb.Append($", F#={finalStates.Length}");

        if (finalStates.Length > 0)
        {
            sb.Append($": [{string.Join(", ", finalStates)}]");
        }
        sb.Append($", T#={transitions.Length}");
        if (transitions.Length > 0)
        {
            sb.Append(": [");
            for (int i = 0; i < transitions.Length; i++)
            {
                Transition t = transitions[i];
                sb.Append($"{t.FromState}->{t.ToState} {Alphabet[t.Symbol]}");
                if (i < transitions.Length - 1) sb.Append(", ");
            }
            sb.Append(']');
        }     
       return sb.ToString();
    }

    #endregion Misc Accessors
}
