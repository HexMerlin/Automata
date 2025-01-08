namespace Automata.Core;

/// <summary>
/// A common interface for deterministic finite automata, such as <see cref="Dfa"/> and <see cref="Cfa"/>.
/// </summary>
public interface IDfa : IFsa
{
    /// <summary>
    /// Gets the initial state of the deterministic automaton.
    /// </summary>
    int InitialState { get; }

    /// <summary>
    /// Gets a read-only view of the state from the specified state.
    /// </summary>
    /// <param name="fromState">The state from which to get the state view.</param>
    /// <returns>A <see cref="StateView"/> representing the state view from the specified state.</returns>
    StateView State(int fromState);
}
