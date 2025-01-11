namespace Automata.Core.Alang;

public class Union(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{
    /// <summary>
    /// Parses the rule <c>Union</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Difference.Parse(ref cursor);
        if (cursor.TryConsumeUnionOperator())
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
