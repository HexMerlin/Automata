namespace Automata.Core.Operations;
public static partial class Ops
{
    /// <summary>
    /// Prefix closure of a given automaton, making all states final.
    /// </summary>
    /// <param name="mfa">The <see cref="Mfa"/> to transform.</param>
    /// <remarks>
    /// The prefix closure is an automaton that accepts all prefixes (including ε) of the language recognized by the original automaton.
    /// This transformation makes every state in the original MFA a final state in the resulting DFA.
    ///
    /// <para>Properties of the prefix closure:</para>
    /// <list type="bullet">
    /// <item>All states are final.</item>
    /// <item>Retains graphical identity: the same alphabet, transitions, and state IDs as the original MFA.</item>
    /// <item>Retains determinism but not necessarily minimalism. For example, the prefix closure of <c>a+</c> is <c>a*</c>,
    /// which can be represented with a single-state MFA.</item>
    /// </list>
    /// </remarks>
    /// <returns>A new <see cref="Dfa"/> representing the prefix closure of the input <see cref="Mfa"/>.</returns>
    public static Dfa PrefixClosure(this Mfa mfa)
    {
        Dfa dfa = new(mfa);
        dfa.SetFinal(Enumerable.Range(0, mfa.StateCount));
        return dfa;
    }

}
