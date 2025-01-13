using System.Diagnostics;
using Automata.Core.Operations;

namespace Automata.Core;

/// <summary>
/// Minimal Finite-state Automaton (MFA).
/// </summary>
/// <remarks>
/// <see cref="Mfa"/> is the most optimized automaton representation, characterized by:
/// <list type="number">
/// <item><c>Deterministic</c> and <c>Minimal</c>: The least possible states and transitions.</item>
/// <item>Contiguous states: States are in range [0..MaxState] </item>
/// <item>Initial state is always <c>0</c></item>
/// <item>Minimal memory footprint: Uses a contiguous memory block for data, with minimal overhead.</item>
/// <item>Performance-optimized for efficient read-only operations.</item>
/// <item>Immutable: Guarantees structural and behavioral invariance.</item>
/// </list>
/// </remarks>
public partial class Mfa : /*IEnumerable<Transition>,*/ IDfa
{
    #region Data

    /// <summary>
    /// Alphabet used by the MFA.
    /// </summary>
    public Alphabet Alphabet { get; }

    private readonly Transition[] transitions;

    /// <summary>
    /// The state number with the highest value.
    /// </summary>
    /// <returns>The maximum state number, or <c>-1</c> if the MFA is empty.</returns>
    public int MaxState { get; }

    /// <summary>
    /// Final states of the MFA.
    /// </summary>
    public readonly int[] finalStates;

    #endregion Data


    /// <summary>
    /// Initial state. Always <c>0</c> for a non-empty <see cref="Mfa"/>. 
    /// <para>For an empty <see cref="Mfa"/>, the initial state is <see cref="Constants.InvalidState"/>.</para>
    /// </summary>
    public int InitialState => StateCount > 0 ? 0 : Constants.InvalidState;

    /// <summary>
    /// Number of states in the MFA.
    /// </summary>
    public int StateCount => MaxState + 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mfa"/> class from an existing <see cref="Dfa"/>.
    /// </summary>
    /// <param name="dfa">A DFA to create from.</param>
    public Mfa(Dfa dfa) 
    {
        this.Alphabet = dfa.Alphabet;
        Dfa minDfa = Ops.Minimal(dfa);       
       
        Dictionary<int, int> dfaToMfaStateMap = new();
        SortedSet<Transition> transitionSet = new();
        int maxState = Constants.InvalidState;

        int initialState = GetOrAddMfaState(minDfa.InitialState);
        Debug.Assert(initialState == 0, "The initial state of a MFA should be 0.");
       
        foreach (Transition t in minDfa.Transitions())
            transitionSet.Add(new Transition(GetOrAddMfaState(t.FromState), t.Symbol, GetOrAddMfaState(t.ToState)));
        
        this.transitions = transitionSet.ToArray();
        this.finalStates = minDfa.FinalStates.Select(GetOrAddMfaState).OrderBy(s => s).ToArray();

        int GetOrAddMfaState(int dfaState)
        {
            if (!dfaToMfaStateMap.TryGetValue(dfaState, out int mfaState))
            {
                mfaState = dfaToMfaStateMap.Count;
                dfaToMfaStateMap[dfaState] = mfaState;
                maxState = Math.Max(maxState, mfaState);
            }
            return mfaState;
        }
    }

    /// <summary>
    /// Final states of the MFA.
    /// </summary>
    public IReadOnlyCollection<int> FinalStates => finalStates;

    /// <summary>
    /// Indicates whether the MFA is empty.
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
    /// Gets the transitions of the DFA.
    /// </summary>
    /// <returns>An collection of transitions.</returns>
    public IReadOnlyCollection<Transition> Transitions() => transitions;

    /// <summary>
    /// Gets the epsilon transitions of the DFA, which is always empty.
    /// </summary>
    /// <returns>An empty collection of <see cref="EpsilonTransition"/>.</returns>
    public IReadOnlyCollection<EpsilonTransition> EpsilonTransitions() => Array.Empty<EpsilonTransition>();

   
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
        for (int i = 0; i < transitions.Length; i++)
            hash.Add(transitions[i]);
        return hash.ToHashCode();
    }

}

