using Automata.Core.Alphabets;

namespace Automata.Core;

/// <summary>
/// Represents a finite state automaton (FSA) interface.
/// </summary>
public interface IFsa
{
    /// <summary>
    /// Gets the alphabet used by the FSA.
    /// </summary>
    IAlphabet Alphabet { get; }

    /// <summary>
    /// Indicates whether the FSA is empty, meaning it has no states or transitions.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Indicates whether the FSA is epsilon-free.
    /// </summary>
    bool IsEpsilonFree { get; }

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
    IEnumerable<Transition> SymbolicTransitions();

    /// <summary>
    /// Gets the epsilon transitions of the FSA.
    /// </summary>
    IEnumerable<EpsilonTransition> EpsilonTransitions();
}
