namespace Automata.Core.Operations;
public static partial class Ops
{
    /// <summary>
    /// Minimizes the DFA using Brzozowski's algorithm.
    /// </summary>
    /// <param name="source">The deterministic finite automaton to minimize.</param>
    /// <returns>A new minimal DFA.</returns>
    internal static Dfa Minimal(Dfa source)
    {
        Dfa reversed = Ops.Deterministic(Ops.Reversal(source));
        return Ops.Deterministic(Ops.Reversal(reversed));
    }
}