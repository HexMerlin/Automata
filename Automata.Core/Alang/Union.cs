namespace Automata.Core.Alang;

public class Union(AlangRegex left, AlangRegex right) : InfixBinary(left, right)
{
    /// <summary>
    /// Parses the rule <c>Union</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangRegex Parse(ref AlangCursor cursor)
    {
        AlangRegex left = Difference.Parse(ref cursor);
        if (cursor.TryConsume(Chars.Union))
        {
            cursor.ShouldBeRightOperand(Chars.Union);
            AlangRegex right = Union.Parse(ref cursor); // Recursive call for repeated
            return new Union(left, right);
        }
        return left;
    }

    ///<inheritdoc/>
    public override int Precedence => 1;

    ///<inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Union}{Param(Right, this)}";
}
