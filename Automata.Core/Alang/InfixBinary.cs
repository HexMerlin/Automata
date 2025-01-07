namespace Automata.Core.Alang;

public abstract class InfixBinary : AlangExpr
{
    public AlangExpr Left { get; }
    public AlangExpr Right { get; }

    public InfixBinary(AlangExpr left, AlangExpr right) : base()
    {
        Left = left;
        Right = right;
    }

}
