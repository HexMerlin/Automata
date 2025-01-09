namespace Automata.Core.Alang;


public class EmptySetExpr : AlangExpr
{
    public EmptySetExpr() : base() { }
    
    public override int Precedence => 7;

    public override string ExpressionString => "()";
}
