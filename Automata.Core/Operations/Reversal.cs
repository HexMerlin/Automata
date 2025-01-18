namespace Automata.Core.Operations;

public static partial class Ops
{

    /// <summary>
    /// Creates a new NFA that recognizes the reversal of the language accepted by the given automaton.
    /// </summary>
    /// <returns>A new NFA representing the reversed automaton.</returns>
    public static Nfa Reversal(this FsaDet source)
        => new(source, applyReverseOperation: true);

}