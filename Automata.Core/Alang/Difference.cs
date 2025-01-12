namespace Automata.Core.Alang;

/// <summary>
/// Represents a difference expression in the <c>Alang</c> grammar.
/// </summary>
/// <param name="left">The left-hand side expression of the difference.</param>
/// <param name="right">The right-hand side expression of the difference.</param>
public class Difference(AlangRegex left, AlangRegex right) : InfixBinary(left, right)
{
    /// <summary>
    /// Parses the rule <c>Difference</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangRegex Parse(ref AlangCursor cursor)
    {
        AlangRegex left = Intersection.Parse(ref cursor);

        if (cursor.TryConsume(Chars.Difference))
        {
            cursor.ShouldBeRightOperand(Chars.Difference);
            AlangRegex right = Difference.Parse(ref cursor);  // Recursive call for repeated
            return new Difference(left, right);
        }
        return left;
    }

    /// <inheritdoc/>
    public override int Precedence => 2;

    /// <inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Difference}{Param(Right, this)}";
}
