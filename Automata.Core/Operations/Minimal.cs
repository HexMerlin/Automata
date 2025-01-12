namespace Automata.Core.Operations;
public static partial class Ops
{
    /// <summary>
    /// Minimizes the DFA using Brzozowski's algorithm.
    /// </summary>
    /// <param name="dfa">The deterministic finite automaton to minimize.</param>
    /// <returns>A new minimal DFA.</returns>
    internal static Dfa Minimal(Dfa dfa)
    {
        Dfa reversed = Deterministic(dfa.Reversed());
        return Deterministic(reversed.Reversed());
    }
}