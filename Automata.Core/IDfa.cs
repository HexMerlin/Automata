namespace Automata.Core;

/// <summary>
/// Common interface for deterministic finite automata, such as <see cref="Dfa"/> and <see cref="Cfa"/>.
/// </summary>
public interface IDfa : IFsa
{
    /// <summary>
    /// Initial state of the deterministic automaton.
    /// </summary>
    int InitialState { get; }

    /// <summary>
    /// Read-only view of the state from the specified state.
    /// </summary>
    /// <param name="fromState">State from which to get the state view.</param>
    /// <returns>A <see cref="StateView"/> representing the state view from the specified state.</returns>
    StateView State(int fromState);
}
