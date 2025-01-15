namespace Automata.Core.Operations;

public static partial class Ops
{
    /// <summary>
    /// Computes the difference between two deterministic finite automata.
    /// </summary>
    /// <param name="minuend">The automaton from which to subtract.</param>
    /// <param name="subtrahend">The automaton whose language will be subtracted from the minuend.</param>
    /// <returns>
    /// A new <see cref="IDfa"/> representing the language of the minuend except the language of the subtrahend.
    /// </returns>
    public static IDfa Difference(IDfa minuend, Mfa subtrahend)
    {
        if (ReferenceEquals(minuend, subtrahend))
            throw new ArgumentException("Operands must not be the same instance.");

        // Subtract empty language returns the minuend
        if (subtrahend.IsEmptyLanguage)
            return minuend;

        var subtrahendComplement = Complement(subtrahend);

        var difference = Intersection(minuend, subtrahendComplement);

        return difference;
    }
}

