namespace Automata.Core.Alang;

public class Concatenation(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{
    public static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = PostfixUnary.ParsePostfix(ref cursor);
        if (cursor.IsExpressionStart)
        {
            AlangExpr right = Concatenation.Parse(ref cursor); // Recursive call for repeated
            return right is EmptySetExpr
                    ? left
                    : new Concatenation(left, right);
        }
        return left;
    }

    public override int Precedence => 4;

    public override string ExpressionString
    {
        get
        {
            var left = Param(Left, this);
            var right = Param(Right, this);
            bool insertSpace = Chars.IsAtomChar(left.Last()) && Chars.IsAtomChar(right.First());
            return insertSpace
                ? $"{left} {right}" // Space required between two atoms
                : left + right;
        }
    }
 
}
