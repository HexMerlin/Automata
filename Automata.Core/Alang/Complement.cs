namespace Automata.Core.Alang;

public class Complement(AlangExpr operand) : PostfixUnary(operand)
{
    ///<inheritdoc/>
    public override int Precedence => 5;

    ///<inheritdoc/>
    public override string AlangExpressionString => $"{Param(Operand, this)}{Chars.Complement}";

}