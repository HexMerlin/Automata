using System.Diagnostics;

namespace Automata.Core;

/// <summary>
/// Common base class for all Finite state automata.
/// </summary>
[DebuggerDisplay("{ToCanonicalString(),nq}")]
public abstract class Fsa
{
    #region Data

    /// <summary>
    /// Alphabet used by the FSA.
    /// </summary>
    public Alphabet Alphabet { get; }

    #endregion Data

    /// <summary>
    /// Initializes a new instance with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet used by the automaton.</param>
    public Fsa(Alphabet alphabet)  
        => Alphabet = alphabet;

    /// <summary>
    /// Indicates whether the FSA is epsilon-free (lacks epsilon transitions).
    /// </summary>
    public abstract bool IsEpsilonFree { get; }

    /// <summary>
    /// Indicates whether the FSA accepts ϵ - the empty sting . 
    /// </summary>
    public abstract bool AcceptsEpsilon { get; }

    /// <summary>
    /// Indicates whether the FSA has an initial state.
    /// </summary>
    /// <returns><see langword="true"/> <c>iff</c> the FSA has at least one initial state.</returns>
    public abstract bool HasInitialState { get; }

    /// <summary>
    /// Final states of the FSA.
    /// </summary>
    public abstract IReadOnlyCollection<int> FinalStates { get; }

    /// <summary>
    /// Indicates whether the specified state is an initial state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is an initial state.</returns>
    public abstract bool IsInitial(int state);

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is a final state.</returns>
    public abstract bool IsFinal(int state);

    /// <summary>
    /// Transitions of the FSA.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Transition"/>.</returns>
    public abstract IReadOnlyCollection<Transition> Transitions();

    /// <summary>
    /// Epsilon transitions of the FSA.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="EpsilonTransition"/>.</returns>
    public abstract IReadOnlyCollection<EpsilonTransition> EpsilonTransitions();

    /// <summary>
    /// Returns a canonical string representation of the MFA's data.
    /// Used by unit tests and for debugging. 
    /// </summary>
    public abstract string ToCanonicalString();
}
