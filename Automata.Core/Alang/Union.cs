namespace Automata.Core.Alang;

public class Union(AlangExpr left, AlangExpr right) : InfixBinary(left, right) 
{
   
    public static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Difference.Parse(ref cursor);
        if (cursor.TryConsume(Chars.Union))
        {
            AlangExpr right = Union.Parse(ref cursor); // Recursive call for repeated
    
            return new Union(left, right);
        }
        return left;
    
      
    }

    ///<inheritdoc/>
    public override int Precedence => 1;

    ///<inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Union}{Param(Right, this)}";

}
