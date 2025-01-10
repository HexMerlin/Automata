namespace Automata.Core.Alang;

/// <summary>
/// Represents a postfix unary expression in the Alang language.
/// </summary>
/// <remarks>
/// This abstract class serves as the base class for all postfix unary operations in Alang expressions,
/// such as option (?), Kleene star (*), Kleene plus (+), and complement (~).
/// </remarks>
public abstract class UnaryExpr : AlangExpr
{
    /// <summary>
    /// Gets the operand of the postfix unary expression.
    /// </summary>
    public AlangExpr Operand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnaryExpr"/> class with the specified operand.
    /// </summary>
    /// <param name="operand">The operand of the postfix unary expression.</param>
    public UnaryExpr(AlangExpr operand) : base()
        => Operand = operand;


    /// <summary>
    /// Parses the rule <c>UnaryExpr</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangExpr ParseUnaryExpr(ref AlangCursor cursor)
    {
        AlangExpr expr = ParsePrimaryExpr(ref cursor);

        while (true)
        {
            char match = cursor.TryConsumeAny(Chars.Option, Chars.KleeneStar, Chars.KleenePlus, Chars.Complement);
            if (match == Chars.Invalid) // none of the postfix operators found: return primary expression
                return expr;

            expr = match switch
            {
                Chars.Option => new Option(expr),
                Chars.KleeneStar => new KleeneStar(expr),
                Chars.KleenePlus => new KleenePlus(expr),
                Chars.Complement => new Complement(expr),
                _ => throw new InvalidOperationException("Should never be reached. Only for completeness.")
            };
        }
    }
}
