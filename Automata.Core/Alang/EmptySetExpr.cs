namespace Automata.Core.Alang;


public class EmptySetExpr : AlangExpr
{
    public EmptySetExpr() : base() { }

    ///<inheritdoc/>
    public override int Precedence => 7;

    ///<inheritdoc/>
    public override string AlangExpressionString => "()";
}
