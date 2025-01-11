namespace Automata.Core.Alang;

/// <summary>
/// Represents a complement expression in the Alang language.
/// </summary>
/// <param name="operand">The operand of the complement expression.</param>
public class Complement(AlangExpr operand) : UnaryExpr(operand)
{
    /// <inheritdoc/>
    public override int Precedence => 5;

    /// <inheritdoc/>
    public override string AlangExpressionString => $"{Param(Operand, this)}{Chars.Complement}";
}
