namespace Automata.Core.Alang;

public class Difference(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{
    public static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Intersection.Parse(ref cursor);
        if (cursor.TryConsume(Chars.Difference))
        {
            AlangExpr right = Difference.Parse(ref cursor);  // Recursive call for repeated

            return new Difference(left, right);
        }
        return left;
    }

    ///<inheritdoc/>
    public override int Precedence => 2;

    ///<inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Difference}{Param(Right, this)}";
}
