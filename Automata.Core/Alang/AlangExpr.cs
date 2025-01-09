﻿using System;

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
    /// Gets an instance of <see cref="Atom"/> representing the empty expression.
    /// </summary>
    public static AlangExpr EmptySetExpr => new EmptySetExpr();

    /// <summary>
    /// Gets the precedence level of this expression.
    /// </summary>
    public abstract int Precedence { get; }

    /// <summary>
    /// Gets the string representation of this expression.
    /// </summary>
    public abstract string ExpressionString { get; }

    /// <summary>
    /// Parses the specified input string into an <see cref="AlangExpr"/>.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input contains unexpected tokens.</exception>
    public static AlangExpr Parse(string input)
    {
        AlangCursor cursor = new(input);
        var expression = ParseExpression(ref cursor);

        if (!cursor.IsEmpty)
            AlangFormatException.ThrowExpectedBeginExpressionOrEOI(cursor);
                
        return expression;
    }

    /// <summary>
    /// Parses an expression from the cursor, starting at the lowest precedence operator.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    private static AlangExpr ParseExpression(ref AlangCursor cursor)
        => cursor.IsEmpty
            ? AlangExpr.EmptySetExpr
            : Union.Parse(ref cursor);

    /// <summary>
    /// Parses a primary expression from the cursor.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed primary expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when parentheses are unmatched in the input.</exception>
    internal static AlangExpr ParsePrimary(ref AlangCursor cursor)
    {
        if (cursor.TryConsume(Chars.LeftParen))
        {
            if (cursor.TryConsume(Chars.RightParen))
                return AlangExpr.EmptySetExpr; // Captured the empty set expression

            var expression = AlangExpr.ParseExpression(ref cursor);

            if (!cursor.TryConsume(Chars.RightParen))
                AlangFormatException.ThrowMissingClosingParenthesis(cursor);

            return expression;
        }

        return cursor.ConsumeAtomOrWildcard();
    }

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
            ? expr.ExpressionString
            : $"({expr.ExpressionString})";

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The expression string of this expression.</returns>
    public override string ToString() => ExpressionString;
}


