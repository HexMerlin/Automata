namespace Automata.Core.Alang;

/// <summary>
/// Represents a Kleene star operation in the Alang language.
/// </summary>
/// <remarks>
/// The Kleene star operation is a unary postfix operator that denotes zero or more repetitions of the operand expression.
/// </remarks>
/// <param name="operand">The operand expression to which the Kleene star operation is applied.</param>
public class KleeneStar(AlangRegex operand) : UnaryExpr(operand)
{
    /// <inheritdoc/>
    public override int Precedence => 5;

    /// <inheritdoc/>
    public override string AlangExpressionString => $"{Param(Operand, this)}{Chars.KleeneStar}";
}
