namespace Automata.Core.Alang;

/// <summary>
/// Represents a concatenation operation in the Alang grammar.
/// </summary>
/// <param name="left">The left operand of the concatenation.</param>
/// <param name="right">The right operand of the concatenation.</param>
public class Concatenation(AlangRegex left, AlangRegex right) : BinaryRegex(left, right)
{
    /// <summary>
    /// Parses the rule <c>Concatenation</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    public static AlangRegex Parse(ref AlangCursor cursor)
    {
        AlangRegex left = UnaryRegex.ParseUnaryRegex(ref cursor);
        if (cursor.IsExpressionStart)
        {
            AlangRegex right = Concatenation.Parse(ref cursor); // Recursive call for repeated
            return new Concatenation(left, right);
        }
        return left;
    }

    /// <inheritdoc/>
    public override int Precedence => 4;

    /// <inheritdoc/>
    public override string AlangExpressionString
    {
        get
        {
            string left = Param(Left, this);
            string right = Param(Right, this);
            bool insertSpace = Chars.IsSymbolChar(left.Last()) && Chars.IsSymbolChar(right.First());
            return insertSpace
                ? $"{left} {right}" // Space required between two Symbols
                : left + right;
        }
    }
}
