namespace Automata;

/// <summary>
/// Represents a finite state automaton (FSA) interface.
/// </summary>
public interface IFsa
{
    /// <summary>
    /// Gets the alphabet used by the FSA.
    /// </summary>
    Alphabet Alphabet { get; }

    /// <summary>
    /// Gets a value indicating whether the FSA is epsilon-free.
    /// </summary>
    bool EpsilonFree { get; }

    /// <summary>
    /// Indicates whether the specified state is an initial state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is an initial state; otherwise, <c>false</c>.</returns>
    bool IsInitial(int state);

    /// <summary>
    /// Indicates whether the specified state is a final state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns><c>true</c> if the specified state is a final state; otherwise, <c>false</c>.</returns>
    bool IsFinal(int state);

    /// <summary>
    /// Gets the transitions of the FSA.
    /// </summary>
    IEnumerable<Transition> Transitions { get; }

    /// <summary>
    /// Gets the epsilon transitions of the FSA.
    /// </summary>
    IEnumerable<EpsilonTransition> EpsilonTransitions { get; }
}
