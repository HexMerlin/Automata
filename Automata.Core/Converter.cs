using Automata.Core.Operations;

namespace Automata.Core;

/// <summary>
/// Static class for conversion between different types of finite state automata.
/// </summary>
public static class Converter
{
    /// <summary>
    /// Returns <paramref name="fsa"/> as a <see cref="Nfa"/>.
    /// <para>Return the same instance if it already is of the desired type.</para>
    /// <para>Otherwise, a new object of the desired type is created.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <returns><paramref name="fsa"/> as a <see cref="Nfa"/>.</returns>
    public static Nfa AsNfa(this Fsa fsa) => fsa switch
    {
        Nfa nfa => nfa,
        Dfa dfa => new Nfa(dfa),
        Mfa mfa => new Nfa(mfa),
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };

    /// <summary>
    /// Returns <paramref name="fsa"/> as a deterministic automaton (<see cref="FsaDet"/>).
    /// <para>Return the same instance if it already is of the desired type.</para>
    /// <para>Otherwise, a new object of the desired type is created.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <remarks>
    /// Effective conversion: NFA → DFA, DFA → DFA, MFA → MFA. 
    /// </remarks>
    /// <returns><paramref name="fsa"/> as a <see cref="FsaDet"/>.</returns>
    public static FsaDet AsDeterministic(this Fsa fsa) => fsa switch
    {
        Nfa nfa => Ops.Deterministic(nfa),
        Dfa dfa => dfa,
        Mfa mfa => mfa,
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };

    /// <summary>
    /// Returns <paramref name="fsa"/> as a <see cref="Mfa"/>.
    /// <para>Return the same instance if it already is of the desired type.</para>
    /// <para>Otherwise, a new object of the desired type is created.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <param name="enforceNew">If <see langword="true"/>, a new <see cref="Mfa"/> is created even if <paramref name="fsa"/> is already a <see cref="Nfa"/>. Default: <see langword="false"/></param>
    /// <returns><paramref name="fsa"/> as a <see cref="Mfa"/>.</returns>
    public static Mfa AsMfa(this Fsa fsa) => fsa switch
    {
        Nfa nfa => new Mfa(Ops.Deterministic(nfa)),
        Dfa dfa => new Mfa(dfa),
        Mfa mfa => mfa,
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };
}
