using System;

namespace Automata.Core.Alang;

/// <summary>
/// Represents an expression in the <c>Alang</c> (Automata language) used for defining finite-state automata. 
/// </summary>
/// <remarks>
/// The <c>Alang</c> grammar specification:
/// <code>
/// AlangExpr             ::= UnionExpr
/// UnionExpr             ::= DifferenceExpr ('|' DifferenceExpr)*
/// DifferenceExpr        ::= IntersectionExpr ('-' IntersectionExpr)*
/// IntersectionExpr      ::= ConcatenationExpr ('&amp;' ConcatenationExpr)*
/// ConcatenationExpr     ::= PostfixExpr PostfixExpr*
/// PostfixExpr           ::= PrimaryExpr PostfixOp*
/// PrimaryExpr           ::= '(' AlangExpr? ')' | Atom
/// Atom                  ::= AtomChar+
/// 
/// PostfixOp::= '?' | '*' | '+' | '~'
/// AtomChar::= ^('|' | '&amp;' | '-' | '?' | '*' | '+' | '~' | '(' | ')' | '\ws')
/// </code>
/// <para><c>Alang</c> operators ordered by precedence (lowest-to-highest).</para>
/// <list type="table">
/// <listheader>
///     <term>Precedence</term>
///     <term>Operation</term>
///     <term>Operator character</term>
///     <term>Position &amp; Arity</term>
/// </listheader>
/// <item>
///     <term>1</term>
///     <term>Union</term>
///     <term>|</term>
///     <term>Infix binary</term>
/// </item>
/// <item>
///     <term>2</term>
///     <term>Difference</term>
///     <term>-</term>
///     <term>Infix binary</term>
/// </item>
/// <item>
///     <term>3</term>
///     <term>Intersection</term>
///     <term>&amp;</term>
///     <term>Infix binary</term>
/// </item>
/// <item>
///     <term>4</term>
///     <term>Concatenation</term>
///     <term>(implicit)</term>
///     <term>Juxtaposition</term>
/// </item>
/// <item>
///     <term>5</term>
///     <term>Option</term>
///     <term>?</term>
///     <term>Postfix unary</term>
/// </item>
/// <item>
///     <term>5</term>
///     <term>Kleene Star</term>
///     <term>*</term>
///     <term>Postfix unary</term>
/// </item>
/// <item>
///     <term>5</term>
///     <term>Kleene Plus</term>
///     <term>+</term>
///     <term>Postfix unary</term>
/// </item>
/// <item>
///     <term>5</term>
///     <term>Complement</term>
///     <term>~</term>
///     <term>Postfix unary</term>
/// </item>
/// <item>
///     <term>6</term>
///     <term>Group</term>
///     <term>( )</term>
///     <term>Enclosing unary</term>
/// </item>
/// <item>
///     <term></term>
///     <term>Atom</term>
///     <term>string literal</term>
///     <term>Atomic leaf</term>
/// </item>
/// </list>
/// Operators with higher precedence levels bind more tightly than those with lower levels.
/// Same level precedence are left-associative.
/// Whitespace is allowed anywhere in the grammar, but it is never required unless to separate directly adjacent atoms.
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
    public static AlangExpr Empty => new Atom(string.Empty);

    /// <summary>
    /// Gets a value indicating whether this expression is empty.
    /// </summary>
    public bool IsEmpty => this is Atom atom && atom.Symbol.Length == 0;

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
    /// <exception cref="FormatException">Thrown when the input contains unexpected tokens.</exception>
    public static AlangExpr Parse(string input)
    {
        AlangCursor cursor = new(input);
        var expression = ParseExpression(ref cursor);

        if (!cursor.IsEmpty)
            throw new FormatException($"Unexpected trailing token: {cursor.First}");

        return expression;
    }

    /// <summary>
    /// Parses an expression from the cursor, starting at the lowest precedence operator.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    private static AlangExpr ParseExpression(ref AlangCursor cursor)
        => cursor.IsEmpty
            ? AlangExpr.Empty
            : Union.Parse(ref cursor);

    /// <summary>
    /// Parses a primary expression from the cursor.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed primary expression.</returns>
    /// <exception cref="FormatException">Thrown when parentheses are unmatched in the input.</exception>
    internal static AlangExpr ParsePrimary(ref AlangCursor cursor)
    {
        if (cursor.TryConsume(Chars.LeftParen))
        {
            if (cursor.TryConsume(Chars.RightParen))
                return AlangExpr.Empty; // Handles empty parentheses

            var expression = AlangExpr.ParseExpression(ref cursor);

            if (!cursor.TryConsume(Chars.RightParen))
                throw new FormatException("Unmatched left parenthesis '(' in input.");
            return expression;
        }

        return cursor.ConsumeAtom();
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


