namespace Automata.Core.Alang;

public class Intersection(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{

    public static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Concatenation.Parse(ref cursor);
        if (cursor.TryConsume(Chars.Intersection))
        {
            AlangExpr right = Intersection.Parse(ref cursor); // Recursive call for repeated
            //if (right is EmptySetExpr)
            //   cursor.ThrowMissingRightOperand(Chars.Intersection);

            return new Intersection(left, right);

        }
        return left;

    }

    public override int Precedence => 3;

    public override string ExpressionString => $"{Param(Left, this)}{Chars.Intersection}{Param(Right, this)}";
}
