namespace Automata.Core.Alang;
public class Wildcard : AlangExpr
{
    public override int Precedence => 7;
    public override string ExpressionString => Chars.Wildcard.ToString();
}
