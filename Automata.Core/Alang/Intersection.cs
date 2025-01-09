namespace Automata.Core.Alang;

public class Intersection(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{

    public static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Concatenation.Parse(ref cursor);
        if (cursor.TryConsume(Chars.Intersection))
        {
            AlangExpr right = Intersection.Parse(ref cursor); // Recursive call for repeated

            return new Intersection(left, right);

        }
        return left;

    }

    ///<inheritdoc/>
    public override int Precedence => 3;

    ///<inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Intersection}{Param(Right, this)}";
}
