namespace Automata.Core.Alang;

/// <summary>
/// Represents a difference expression in the <c>Alang</c> grammar.
/// </summary>
/// <param name="left">The left-hand side expression of the difference.</param>
/// <param name="right">The right-hand side expression of the difference.</param>
public class Difference(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{
    /// <summary>
    /// Parses the rule <c>Difference</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Intersection.Parse(ref cursor);

        if (cursor.TryConsume(Chars.Difference))
        {
            cursor.ShouldBeRightOperand(Chars.Difference);
            AlangExpr right = Difference.Parse(ref cursor);  // Recursive call for repeated
            return new Difference(left, right);
        }
        return left;
    }

    /// <inheritdoc/>
    public override int Precedence => 2;

    /// <inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Difference}{Param(Right, this)}";
}
