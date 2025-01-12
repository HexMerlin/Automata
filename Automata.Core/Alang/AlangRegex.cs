namespace Automata.Core.Alang;

/// <summary>
/// Represents an expression in the <c>Alang</c> (Automata language) used for defining finite-state automata. 
/// <see cref="AlangRegex"/> has a one-to-one correspondence with Finite State Automata.
/// </summary>
/// <remarks>
/// The <c>Alang</c> language is a domain-specific language designed for defining and working with finite-state automata.
/// This class serves as the starting point for parsing <c>Alang</c> expressions with <see cref="AlangRegex.Parse(string)"/>.
/// For more information about the <c>Alang</c> language, see the <see href="https://hexmerlin.github.io/Automata/ALANG.html">Alang Grammar Specification</see>.
/// </remarks>
public abstract class AlangRegex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlangRegex"/> class.
    /// </summary>
    protected AlangRegex() { }

    /// <summary>
    /// Precedence level of this expression according to the Alang grammar specification.
    /// </summary>
    /// <value>
    /// An integer representing the precedence level of the expression.
    /// </value>
    public abstract int Precedence { get; }

    /// <summary>
    /// String representation of this expression in valid Alang language syntax.
    /// </summary>
    /// <value>
    /// A string representing the expression in Alang syntax.
    /// </value>
    public abstract string AlangExpressionString { get; }

    /// <summary>
    /// Indicates whether this expression is an empty string.
    /// An empty string is not a valid expression in Alang.
    /// Used internally by the Parser to handle empty strings.
    /// </summary>
    /// <value>
    /// <see langword="true"/> <c>iff</c> this expression is an empty string; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsEmptyString => this is Atom atom && atom.Symbol.Length == 0;

    /// <summary>
    /// Parses the specified input string into an <see cref="AlangRegex"/>.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    public static AlangRegex Parse(string input)
    {
        AlangCursor cursor = new(input);
        cursor.ShouldNotBeEmpty();

        AlangRegex regex = ParseAlangRegex(ref cursor);

        cursor.ShouldNotBeRightParen();

        return regex;
    }

    /// <summary>
    /// Parses the rule <c>AlangRegex</c> in the Alang grammar specification.
    /// </summary>
    /// <remarks>
    /// Parses an expression from the cursor, starting at the lowest precedence operator.
    /// </remarks>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    private static AlangRegex ParseAlangRegex(ref AlangCursor cursor)
    {
        AlangRegex regex = Union.Parse(ref cursor);
        return regex;
    }

    /// <summary>
    /// Parses the rule <c>PrimaryRegex</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse.</param>
    /// <returns>An <see cref="AlangRegex"/> representing the parsed primary expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when parentheses are unmatched in the input.</exception>
    internal static AlangRegex ParsePrimaryRegex(ref AlangCursor cursor)
    {
        if (cursor.TryConsume(Chars.LeftParen))
        {
            cursor.ShouldNotBeOperator();
            AlangRegex regex = ParseAlangRegex(ref cursor); // Parse inner expression
            cursor.ShouldBeRightParen();
            _ = cursor.TryConsume(Chars.RightParen);

            if (regex.IsEmptyString)
                return new EmptySet(); // Captured empty parentheses '()' => EmptySet

            return regex;
        }

        if (cursor.TryConsume(Chars.Wildcard))
            return new Wildcard();

        return cursor.ConsumeAtom();
    }

    /// <summary>
    /// Returns the string of the given expression, enclosed in parentheses only if necessary based on operator precedence.
    /// </summary>
    /// <remarks>
    /// Parentheses are added if the precedence of the given expression is lower than the precedence of the parent.
    /// </remarks>
    /// <param name="expr">The expression to get the string representation of.</param>
    /// <param name="parent">The parent expression of <paramref name="expr"/>.</param>
    /// <returns>The expression string, potentially enclosed in parentheses.</returns>
    protected static string Param(AlangRegex expr, AlangRegex parent)
        => expr.Precedence >= parent.Precedence
            ? expr.AlangExpressionString
            : $"({expr.AlangExpressionString})";

    /// <summary>
    /// String that represents the current object.
    /// </summary>
    /// <returns>The expression string of this expression.</returns>
    public override string ToString() => AlangExpressionString;
}


