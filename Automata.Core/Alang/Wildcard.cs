namespace Automata.Core.Alang;
public class Wildcard : AlangExpr
{
    ///<inheritdoc/>
    public override int Precedence => 7;

    ///<inheritdoc/>
    public override string AlangExpressionString => Chars.Wildcard.ToString();
}
