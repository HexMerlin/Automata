using Automata.Core.Operations;

namespace Automata.Core;

/// <summary>
/// Static class for conversion between different types of finite state automata.
/// </summary>
public static class Converter
{
    /// <summary>
    /// Returns <paramref name="fsa"/> as a <see cref="Nfa"/>.
    /// <para>A new <see cref="Nfa"/> is created if either: <paramref name="fsa"/> is not a <see cref="Nfa"/> or <paramref name="enforceNew"/> = <see langword="true"/>.</para>
    /// <para>Otherwise, the same object is returned.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <param name="enforceNew">If <see langword="true"/>, a new <see cref="Nfa"/> is created even if <paramref name="fsa"/> is already a <see cref="Nfa"/>. Default: <see langword="false"/></param>
    /// <returns><paramref name="fsa"/> as a <see cref="Nfa"/>.</returns>
    public static Nfa AsNfa(this IFsa fsa, bool enforceNew = false) => fsa switch
    {
        Nfa nfa => enforceNew ? new Nfa(nfa) : nfa,
        Dfa dfa => new Nfa(dfa),
        Mfa mfa => new Nfa(mfa),
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };

    /// <summary>
    /// Returns <paramref name="fsa"/> as a deterministic automaton (<see cref="IDfa"/>).
    /// <para>A new <see cref="IDfa"/> is created if either: <paramref name="fsa"/> is not a <see cref="IDfa"/> or <paramref name="enforceNew"/> = <see langword="true"/>.</para>
    /// <para>Otherwise, the same object is returned.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <param name="enforceNew">If <see langword="true"/>, a new <see cref="IDfa"/> is created even if <paramref name="fsa"/> is already a <see cref="Nfa"/>. Default: <see langword="false"/></param>
    /// <remarks>
    /// Effective conversion: NFA → DFA, DFA → DFA, MFA → MFA. 
    /// </remarks>
    /// <returns><paramref name="fsa"/> as a <see cref="IDfa"/>.</returns>
    public static IDfa AsIDfa(this IFsa fsa) => fsa switch
    {
        Nfa nfa => Ops.Deterministic(nfa),
        Dfa dfa => dfa,
        Mfa mfa => mfa,
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };

    /// <summary>
    /// Returns <paramref name="fsa"/> as a <see cref="Mfa"/>.
    /// <para>A new <see cref="Mfa"/> is created if either: <paramref name="fsa"/> is not a <see cref="Mfa"/> or <paramref name="enforceNew"/> = <see langword="true"/>.</para>
    /// <para>Otherwise, the same object is returned.</para>
    /// </summary>
    /// <param name="fsa">Finite state automaton to convert.</param>
    /// <param name="enforceNew">If <see langword="true"/>, a new <see cref="Mfa"/> is created even if <paramref name="fsa"/> is already a <see cref="Nfa"/>. Default: <see langword="false"/></param>
    /// <returns><paramref name="fsa"/> as a <see cref="Mfa"/>.</returns>
    public static Mfa AsMfa(this IFsa fsa) => fsa switch
    {
        Nfa nfa => new Mfa(Ops.Deterministic(nfa)),
        Dfa dfa => new Mfa(dfa),
        Mfa mfa => mfa,
        _ => throw new NotSupportedException($"Unexpected Fsa type: {fsa.GetType()}")
    };
}
