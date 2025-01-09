namespace Automata.Core.Alang;

/// <summary>
/// Represents a postfix unary expression in the Alang language.
/// </summary>
/// <remarks>
/// This abstract class serves as the base class for all postfix unary operations in Alang expressions,
/// such as option (?), Kleene star (*), Kleene plus (+), and complement (~).
/// </remarks>
public abstract class PostfixUnary : AlangExpr
{
    /// <summary>
    /// Gets the operand of the postfix unary expression.
    /// </summary>
    public AlangExpr Operand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostfixUnary"/> class with the specified operand.
    /// </summary>
    /// <param name="operand">The operand of the postfix unary expression.</param>
    public PostfixUnary(AlangExpr operand) : base()
        => Operand = operand;

    /// <summary>
    /// Parses a postfix unary expression from the specified cursor.
    /// </summary>
    /// <param name="cursor">A reference to the <see cref="AlangCursor"/> from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed postfix unary expression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid postfix operator is encountered.</exception>
    /// <remarks>
    /// This method first parses a primary expression from the cursor, and then repeatedly attempts to consume any postfix operators
    /// (option '?', Kleene star '*', Kleene plus '+', complement '~') that follow the primary expression.
    /// For any postfix operator found, it wraps the current expression in the corresponding <see cref="PostfixUnary"/> subclass.
    /// If no postfix operator is found, the primary expression is returned as is.
    /// </remarks>
    public static AlangExpr ParsePostfix(ref AlangCursor cursor)
    {
        AlangExpr expr = ParsePrimary(ref cursor);

        while (true)
        {
            char match = cursor.TryConsumeAny(Chars.Option, Chars.KleeneStar, Chars.KleenePlus, Chars.Complement);
            if (match == Chars.Invalid) // no more postfix operators
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
