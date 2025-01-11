namespace Automata.Core.Alang;
/// <summary>
/// Represents a wildcard expression in the <c>Alang</c> (Automata language).
/// </summary>
/// <remarks>
/// A wildcard that matches any atom in the Alphabet.
/// </remarks>
public class Wildcard : AlangExpr
{
    ///<inheritdoc/>
    public override int Precedence => 7;

    ///<inheritdoc/>
    public override string AlangExpressionString => Chars.Wildcard.ToString();
}
