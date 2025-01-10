using System;

namespace Automata.Core.Alang;

/// <summary>
/// Represents an expression in the <c>Alang</c> (Automata language) used for defining finite-state automata. 
/// </summary>
/// <remarks>
/// For more information about the <c>Alang</c> language, see the <see href="https://hexmerlin.github.io/Automata/ALANG.html">Alang Grammar Specification</see>.
/// </remarks>
public abstract class AlangExpr
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlangExpr"/> class.
    /// </summary>
    protected AlangExpr() { }

    /// <summary>
    /// Gets the precedence level of this expression according to Alang grammar specification.
    /// </summary>
    public abstract int Precedence { get; }

    /// <summary>
    /// Gets the string representation of this expression in valid Alang language syntax.
    /// </summary>
    public abstract string AlangExpressionString { get; }

    /// <summary>
    /// Parses the specified input string into an <see cref="AlangExpr"/>.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    public static AlangExpr Parse(string input)
    {
        AlangCursor cursor = new(input);
        return ParseAlangExpr(ref cursor);
    }


    /// <summary>
    /// Parses the rule <c>AlangExpr</c> in the Alang grammar specification.
    /// </summary>
    /// <remarks>
    /// Parses an expression from the cursor, starting at the lowest precedence operator.
    /// </remarks>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    private static AlangExpr ParseAlangExpr(ref AlangCursor cursor)
    {         
        var expression = Union.Parse(ref cursor);

        if (cursor.Is(Chars.RightParen))
            AlangFormatException.ThrowSpuriousClosingParenthesis(cursor);

        if (!cursor.IsEmpty)
            throw new Exception("NOT EMPTY EXCEPTION");
        return expression;
    }

    /// <summary>
    /// Parses a primary expression from the cursor.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed primary expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when parentheses are unmatched in the input.</exception>
    internal static AlangExpr ParsePrimaryExpr(ref AlangCursor cursor)
    {
        // 1. Spurious closing parenthesis check
        if (cursor.Is(Chars.RightParen))
            AlangFormatException.ThrowSpuriousClosingParenthesis(cursor);
        
        // 2. Parenthesized expression or empty-set
        if (cursor.Is(Chars.LeftParen))
            return ParseParenthesizedExpr(ref cursor);

        // 3. Wildcard ('.')
        if (cursor.TryConsume(Chars.Wildcard))
            return new Wildcard();
        
        // 4. Atom (e.g. "abc")
        if (cursor.TryConsumeAtom(out Atom? atom))
            return atom!;

        if (!cursor.IsExpressionStart)
          AlangFormatException.ThrowExpectedBeginExpressionOrEOI(cursor);

        throw new Exception("Unreachable code");
    }

    private static AlangExpr ParseParenthesizedExpr(ref AlangCursor cursor)
    {
        if (!cursor.TryConsume(Chars.LeftParen))
        {
            throw new Exception("SHOULD NOT REACH HERE. Expected '('");
        }

        if (cursor.IsEmpty)
            // We have a missing closing parenthesis
            AlangFormatException.ThrowMissingClosingParenthesis(cursor);
                
        if (cursor.TryConsume(Chars.RightParen))
            return new EmptySet(); // Handle empty parentheses => '()'
               
        AlangExpr expression = ParseAlangExpr(ref cursor); // Parse inner expression
               
        if (!cursor.TryConsume(Chars.RightParen))
            AlangFormatException.ThrowMissingClosingParenthesis(cursor);
       
        return expression;
    }


    //internal static AlangExpr ParsePrimaryExpr(ref AlangCursor cursor)
    //{

    //    if (cursor.Is(Chars.RightParen))
    //        AlangFormatException.ThrowSpuriousClosingParenthesis(cursor);
    //    if (cursor.TryConsume(Chars.LeftParen))
    //    {
    //        if (cursor.TryConsume(Chars.RightParen))
    //            return new EmptySet(); // Captured the empty set expression

    //        var expression = AlangExpr.ParseAlangExpr(ref cursor);

    //        if (!cursor.TryConsume(Chars.RightParen))
    //            AlangFormatException.ThrowMissingClosingParenthesis(cursor);

    //        return expression;
    //    }

    //    if (cursor.Is(Chars.LeftParen))
    //        AlangFormatException.ThrowMissingClosingParenthesis(cursor);

    //    if (cursor.TryConsume(Chars.Wildcard))
    //        return new Wildcard();

    //    if (cursor.TryConsumeAtom(out Atom? atom))
    //    {
    //        return atom!;
    //    }

    //    //THE PROBLEM: For the initial input string "a(" we finally end up here with an characters consumed. That is not desired. 
    //    //We have a missing closing parenthesis in the input, but this cannot be easily detected here.
    //    //parentheses mathcing and missing closing parentheses is assumed to handled inside the if-clause above in this method, where a left parenthesis has been consumed.
    //    throw new Exception("Unhandled EXCEPTION IN ParsePrimaryExpr");
    //}

    /// <summary>
    /// Returns the expression string of the given expression, enclosed in parentheses only if necessary based on operator precedence.
    /// </summary>
    /// <remarks>
    /// Parentheses are added if the precedence of the given expression is lower than the precedence of the parent.
    /// </remarks>
    /// <param name="expr">The expression to get the string representation of.</param>
    /// <param name="parent">The parent expression of <paramref name="expr"/>.</param>
    /// <returns>The expression string, potentially enclosed in parentheses.</returns>
    protected static string Param(AlangExpr expr, AlangExpr parent)
        => expr.Precedence >= parent.Precedence
            ? expr.AlangExpressionString
            : $"({expr.AlangExpressionString})";

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The expression string of this expression.</returns>
    public override string ToString() => AlangExpressionString;
}


