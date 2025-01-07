namespace Automata.Core.Alang;

public abstract class PostfixUnary : AlangExpr
{
    public AlangExpr Operand { get; }

    public PostfixUnary(AlangExpr operand) : base()
        => Operand = operand;


    public static AlangExpr ParsePostfix(ref AlangCursor cursor)
    {
        AlangExpr expr = ParsePrimary(ref cursor);

        while (true)
        {
            char match = cursor.TryConsumeAny(Chars.Option, Chars.KleeneStar, Chars.KleenePlus, Chars.Complement);
            if (match == Chars.Invalid) //no more postfix operators
                return expr;

            expr = match switch
            {
                Chars.Option => new Option(expr),
                Chars.KleeneStar => new KleeneStar(expr),
                Chars.KleenePlus => new KleenePlus(expr),
                Chars.Complement => new Complement(expr),
                _ => throw new InvalidOperationException("Invalid postfix operator")
            };
        }
    }
}
