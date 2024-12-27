using System.Collections;
using System.Transactions;
using System.Xml.Schema;
using Automata.Core.TransitionSets;

namespace Automata.Core;

/// <summary>
/// Represents a deterministic finite automaton (DFA).
/// </summary>
/// <remarks>A DFA is always deterministic and epsilon free. </remarks>
public class Dfa : DeterministicTransitions, IFsa
{
    #region Data
    /// <summary>
    /// Gets the alphabet used by the DFA.
    /// </summary>
    public Alphabet Alphabet { get; }

    /// <summary>
    /// Gets or sets the initial state of the DFA.
    /// </summary>
    public int InitialState { get; private set; } = Constants.InvalidState; //no initial state

    private readonly HashSet<int> finalStates = [];
    #endregion Data

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with an empty alphabet.
    /// </summary>
    public Dfa() : this(new Alphabet()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    public Dfa(Alphabet alphabet) => Alphabet = alphabet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class with the specified alphabet, transitions, initial state, and final states.
    /// </summary>
    /// <param name="alphabet">The alphabet used by the DFA.</param>
    /// <param name="transitions">The transitions of the DFA.</param>
    /// <param name="initialState">The initial state of the DFA.</param>
    /// <param name="finalStates">The final states of the DFA.</param>
    public Dfa(Alphabet alphabet, IEnumerable<SymbolicTransition> transitions, int initialState, IEnumerable<int> finalStates) : this(alphabet)
    {
        SetInitial(initialState);
        this.finalStates.UnionWith(finalStates);
        this.UnionWith(transitions);
    }

    /// <summary>
    /// Sets the initial state of the DFA.
    /// </summary>
    /// <param name="state">The state to set as the initial state.</param>
    public void SetInitial(int state) => InitialState = state;

    /// <summary>
    /// Sets the specified state as a final state or removes it from the final states.
    /// </summary>
    /// <param name="state">The state to set or remove as a final state.</param>
    /// <param name="final">If <c>true</c>, the state is added to the final states; otherwise, it is removed.</param>
    public void SetFinal(int state, bool final = true)
    {
        if (final)
            finalStates.Add(state);
        else
            finalStates.Remove(state);
    }

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
    public bool IsFinal(int state) => finalStates.Contains(state);

    /// <summary>
    /// Gets a value indicating whether the DFA is epsilon-free.
    /// </summary>
    public bool EpsilonFree => true;

    /// <summary>
    /// Gets the final states of the DFA.
    /// </summary>
    public IReadOnlySet<int> FinalStates => finalStates;

    /// <summary>
    /// Gets the transitions of the DFA.
    /// </summary>
    public IEnumerable<SymbolicTransition> SymbolicTransitions() => orderByFromState;

    /// <summary>
    /// Gets the epsilon transitions of the DFA, which is always empty.
    /// </summary>
    public IEnumerable<EpsilonTransition> EpsilonTransitions() => [];

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <remarks>Uses Brzozowski's algorithm</remarks>
    /// <returns>A minimized DFA.</returns>
    public Dfa Minimized()
    {
        Dfa reversed = Reversed().ToDfa();
        return reversed.Reversed().ToDfa();
    }

    /// <summary>
    /// Reverses the DFA.
    /// </summary>
    /// <returns>An NFA representing the reversed DFA.</returns>
    public Nfa Reversed() => new(this, applyReverseOperation: true);

    /// <summary>
    /// Converts the DFA to an NFA.
    /// </summary>
    /// <returns>An NFA representing the DFA.</returns>
    public Nfa ToNFA() => new(this);

    /// <summary>
    /// Indicates whether the DFA accepts the given sequence of symbols.
    /// </summary>
    /// <param name="sequence">The sequence of symbols to check.</param>
    /// <returns><see langref="true"/> <c>iff</c> the DFA accepts the sequence.</returns>
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

    


}
