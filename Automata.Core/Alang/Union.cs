namespace Automata.Core.Alang;

public class Union(AlangExpr left, AlangExpr right) : InfixBinary(left, right) 
{
   
    public static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Difference.Parse(ref cursor);
        if (cursor.TryConsume(Chars.Union))
        {
            AlangExpr right = Union.Parse(ref cursor); // Recursive call for repeated
            //if (right is EmptySetExpr)
            //  cursor.ThrowMissingRightOperand(Chars.Union);

            return new Union(left, right);
        }
        return left;
    
      
    }

    public override int Precedence => 1;


    public override string ExpressionString => $"{Param(Left, this)}{Chars.Union}{Param(Right, this)}";

}
