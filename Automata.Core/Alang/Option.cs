namespace Automata.Core.Alang;

public class Option(AlangExpr operand) : PostfixUnary(operand)
{
    public override int Precedence => 5;

    public override string ExpressionString => $"{Param(Operand, this)}{Chars.Option}";

}
