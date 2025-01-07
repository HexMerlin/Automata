namespace Automata.Core.Alang;

public class Difference(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{
    public static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Intersection.Parse(ref cursor);
        if (cursor.TryConsume(Chars.Difference))
        {
            AlangExpr right = Difference.Parse(ref cursor);  // Recursive call for repeated
            if (right.IsEmpty)
                throw new FormatException($"Expected expression after {Chars.Difference}: {cursor.ToString()}");
            return new Difference(left, right);
        }
        return left;
    }
      
    public override int Precedence => 2;

    public override string ExpressionString => $"{Param(Left, this)}{Chars.Difference}{Param(Right, this)}";
}
