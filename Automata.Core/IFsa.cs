namespace Automata.Core;

/// <summary>
/// Finite state automaton (FSA) interface.
/// </summary>
public interface IFsa
{
    /// <summary>
    /// Alphabet used by the FSA.
    /// </summary>
    Alphabet Alphabet { get; }

    /// <summary>
    /// Indicates whether the language of the FSA is the empty language (∅).
    /// This means the FSA does not accept anything, including the empty string (ϵ).
    /// </summary>
    bool IsEmptyLanguage { get; }

    /// <summary>
    /// Indicates whether the FSA accepts ϵ - the empty sting . 
    /// </summary>
    bool AcceptsEpsilon { get; }

    /// <summary>
    /// Indicates whether the FSA is epsilon-free (lacks epsilon transitions).
    /// </summary>
    bool IsEpsilonFree { get; }

    /// <summary>
    /// Final states of the FSA.
    /// </summary>
    IReadOnlyCollection<int> FinalStates { get; }

    /// <summary>
    /// Indicates whether the specified state is an initial state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is an initial state.</returns>
    bool IsInitial(int state);

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">State to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the specified state is a final state.</returns>
    bool IsFinal(int state);

    /// <summary>
    /// Transitions of the FSA.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Transition"/>.</returns>
    IReadOnlyCollection<Transition> Transitions();

    /// <summary>
    /// Epsilon transitions of the FSA.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="EpsilonTransition"/>.</returns>
    IReadOnlyCollection<EpsilonTransition> EpsilonTransitions();
}
