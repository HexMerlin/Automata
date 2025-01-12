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
    /// Final states of the IDfa.
    /// </summary>
    IReadOnlySet<int> FinalStates { get; }

    /// <summary>
    /// Upper limit for the maximum state number in the DFA. 
    /// <para>A value (<see cref="MaxState"/> + 1) is guaranteed to be an unused state number.</para>
    /// </summary>
    public int MaxState { get; }

    /// <summary>
    /// Read-only view of the state from the specified state.
    /// </summary>
    /// <param name="fromState">State from which to get the state view.</param>
    /// <returns>A <see cref="StateView"/> representing the state view from the specified state.</returns>
    StateView State(int fromState);
}
