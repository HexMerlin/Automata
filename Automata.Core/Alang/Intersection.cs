namespace Automata.Core.Alang;

/// <summary>
/// Represents an intersection expression in the Alang grammar specification.
/// </summary>
/// <param name="left">The left operand of the intersection expression.</param>
/// <param name="right">The right operand of the intersection expression.</param>
public class Intersection(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{
    /// <summary>
    /// Parses the rule <c>Difference</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Concatenation.Parse(ref cursor);
        if (cursor.TryConsumeIntersectionOperator())
        {
            AlangExpr right = Intersection.Parse(ref cursor); // Recursive call for repeated
            return new Intersection(left, right);
        }
        return left;
    }

    /// <inheritdoc/>
    public override int Precedence => 3;

    /// <inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Intersection}{Param(Right, this)}";
}
