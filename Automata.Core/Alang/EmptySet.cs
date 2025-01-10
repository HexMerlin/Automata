namespace Automata.Core.Alang;


public class EmptySet : AlangExpr
{
    public EmptySet() : base() { }

    ///<inheritdoc/>
    public override int Precedence => 7;

    ///<inheritdoc/>
    public override string AlangExpressionString => "()";
}
