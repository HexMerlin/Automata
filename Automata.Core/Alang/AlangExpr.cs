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
    /// Indicates whether this expression is an empty string.
    /// An empty string is not a valid expression in Alang.
    /// Is used internally by the Parser to handle empty strings.
    /// </summary>
    public bool IsEmptyString => this is Atom atom && atom.Symbol.Length == 0;

    /// <summary>
    /// Parses the specified input string into an <see cref="AlangExpr"/>.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    public static AlangExpr Parse(string input)
    {
        AlangCursor cursor = new(input);
        cursor.ShouldNotBeEmpty();

        AlangExpr expression = ParseAlangExpr(ref cursor);
        
        cursor.ShouldNotBeRightParen();
      
        return expression;
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
 
        AlangExpr expression = Union.Parse(ref cursor);
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
        //cursor.ShouldNotBeRightParen();
       
        if (cursor.TryConsume(Chars.LeftParen))
        {
            //if (cursor.TryConsume(Chars.RightParen))
            //    return new EmptySet(); // Handle empty parentheses => '()'

            //if (cursor.IsEmpty)
            //    AlangFormatException.ThrowMissingClosingParenthesis(cursor);       
            cursor.ShouldNotBeOperator();
            AlangExpr expression = ParseAlangExpr(ref cursor); // Parse inner expression
            cursor.ConsumeRightParen();
            
            if (expression.IsEmptyString)
                return new EmptySet(); // Handle empty parentheses => '()'
           
            return expression;
        }

        if (cursor.TryConsume(Chars.Wildcard))
            return new Wildcard();

        return cursor.ConsumeAtom();
        //if (cursor.TryConsumeAtom(out Atom? atom))
        //    return atom!;

        //AlangFormatException.ThrowExpectedBeginExpressionOrEOI(cursor);
        //return null!; //make compiler happy
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
            ? expr.AlangExpressionString
            : $"({expr.AlangExpressionString})";

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The expression string of this expression.</returns>
    public override string ToString() => AlangExpressionString;
}


