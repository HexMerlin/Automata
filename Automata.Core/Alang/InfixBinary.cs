namespace Automata.Core.Alang;

/// <summary>
/// Represents a binary infix expression in the Alang language.
/// </summary>
/// <remarks>
/// This is an abstract class that serves as a base for specific binary infix expressions.
/// </remarks>
public abstract class InfixBinary : AlangExpr
{
    /// <summary>
    /// Left operand of the binary infix expression.
    /// </summary>
    public AlangExpr Left { get; }

    /// <summary>
    /// Right operand of the binary infix expression.
    /// </summary>
    public AlangExpr Right { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfixBinary"/> class with the specified left and right operands.
    /// </summary>
    /// <param name="left">The left operand of the binary infix expression.</param>
    /// <param name="right">The right operand of the binary infix expression.</param>
    public InfixBinary(AlangExpr left, AlangExpr right) : base()
    {
        Left = left;
        Right = right;
    }
}
