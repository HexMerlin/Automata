namespace Automata.Core.Operations;

public static partial class Ops
{
    /// <summary>
    /// Computes the difference between two deterministic finite automata (DFAs).
    /// </summary>
    /// <param name="minuend">The DFA from which to subtract.</param>
    /// <param name="subtrahend">The DFA whose language will be subtracted from the minuend.</param>
    /// <returns>
    /// A new <see cref="Dfa"/> representing the language of the minuend DFA minus the language of the subtrahend DFA.
    /// </returns>
    /// <remarks>
    /// This operation effectively removes all strings recognized by the subtrahend DFA from the minuend DFA.
    /// </remarks>
    public static Dfa Difference(IDfa minuend, Mfa subtrahend)
    {
        var subtrahendComplement = Complement(subtrahend);

        var difference = Intersection(minuend, subtrahendComplement);

        return difference;
    }
}

