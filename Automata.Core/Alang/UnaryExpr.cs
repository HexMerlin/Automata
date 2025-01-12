namespace Automata.Core.Alang;

/// <summary>
/// Postfix unary expression in the Alang language.
/// </summary>
/// <remarks>
/// Base class for all postfix unary operations in Alang expressions,
/// such as option (?), Kleene star (*), Kleene plus (+), and complement (~).
/// </remarks>
/// <param name="operand">Operand of the postfix unary expression.</param>
public abstract class UnaryExpr(AlangRegex operand) : AlangRegex()
{
    /// <summary>
    /// Operand of the postfix unary expression.
    /// </summary>
    public AlangRegex Operand { get; } = operand;

    /// <summary>
    /// Parses the rule <c>UnaryExpr</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">Cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangRegex ParseUnaryRegex(ref AlangCursor cursor)
    {
        cursor.ShouldNotBeOperator();
        AlangRegex regex = ParsePrimaryRegex(ref cursor);

        while (true)
        {
            char match = cursor.TryConsumeAny(Chars.Option, Chars.KleeneStar, Chars.KleenePlus, Chars.Complement);
            if (match == Chars.Invalid) // none of the postfix operators found: return primary expression
                return regex;

            regex = match switch
            {
                Chars.Option => new Option(regex),
                Chars.KleeneStar => new KleeneStar(regex),
                Chars.KleenePlus => new KleenePlus(regex),
                Chars.Complement => new Complement(regex),
                _ => throw new InvalidOperationException("Should never be reached. Only for completeness.")
            };
        }
    }
}
