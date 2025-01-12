using Automata.Core.Operations;

namespace Automata.Core;

/// <summary>
/// Static class for conversion between different types of finite state automata.
/// <para>· If the input is already in the desired format, the input object is returned as is.</para>
/// <para>· if the input is not in the desired format, a new object of the specified type is returned.</para>
/// </summary>
public static class Converter
{
    /// <summary>
    /// Returns the <paramref name="fsa"/> as a <see cref="Nfa"/>.
    /// <para>· If <paramref name="fsa"/> is already a <see cref="Nfa"/>, the same object is returned.</para>
    /// <para>· Otherwise, a conversion to a new <see cref="Nfa"/> if performed.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <returns><paramref name="fsa"/> as a <see cref="Nfa"/>.</returns>
    public static Nfa AsNfa(this IFsa fsa) => fsa switch
    {
        Nfa nfa => nfa,
        Dfa dfa => new Nfa(dfa),
        Cfa cfa => new Nfa(cfa),
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };

    ///// <summary>
    ///// Returns the <paramref name="fsa"/> as a deterministic <see cref="Dfa"/>.
    ///// <para>· If <paramref name="fsa"/> is already a <see cref="Dfa"/>, the same object is returned.</para>
    ///// <para>· Otherwise, a conversion to a new <see cref="Dfa"/> if performed.</para>
    ///// </summary>
    ///// <remarks>Use only if a <see cref="Dfa"/> is explicitly needed. </remarks>
    ///// <param name="fsa">Finite state automaton to convert.</param>
    ///// <returns><paramref name="fsa"/> as a <see cref="IDfa"/>.</returns>
    //public static Dfa AsDfa(this IFsa fsa) => fsa switch
    //{
    //    Nfa nfa => Ops.Deterministic(nfa),
    //    Dfa dfa => dfa,
    //    Cfa cfa => cfa,
    //    _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    //};

    /// <summary>
    /// Returns the <paramref name="fsa"/> as a deterministic automaton <see cref="IDfa"/>.
    /// <para>· If <paramref name="fsa"/> is already a <see cref="IDfa"/>, the same object is returned.</para>
    /// <para>· Otherwise, a conversion to a new <see cref="IDfa"/> if performed.</para>
    /// </summary>
    /// <remarks>
    /// Effective conversion: NFA → DFA, DFA → same, CFA → same. 
    /// </remarks>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <returns><paramref name="fsa"/> as a <see cref="IDfa"/>.</returns>
    public static IDfa AsIDfa(this IFsa fsa) => fsa switch
    {
        Nfa nfa => Ops.Deterministic(nfa),
        Dfa dfa => dfa,
        Cfa cfa => cfa,
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };

    /// <summary>
    /// Returns the <paramref name="fsa"/> as a <see cref="Cfa"/>.
    /// <para>· If <paramref name="fsa"/> is already a <see cref="Cfa"/>, the same object is returned.</para>
    /// <para>· Otherwise, a conversion to a new <see cref="Cfa"/> if performed.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <returns><paramref name="fsa"/> as a <see cref="Cfa"/>.</returns>
    public static Cfa AsCfa(this IFsa fsa) => fsa switch
    {
        Nfa nfa => new Cfa(Ops.Deterministic(nfa)),
        Dfa dfa => new Cfa(dfa),
        Cfa cfa => cfa,
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };
}
