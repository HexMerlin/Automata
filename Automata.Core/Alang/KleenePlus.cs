namespace Automata.Core.Alang;

/// <summary>
/// Represents a Kleene plus unary expression in the Alang language.
/// </summary>
/// <remarks>
/// The Kleene plus operation is a postfix unary operation that denotes one or more repetitions of the operand expression.
/// </remarks>
/// <param name="operand">The operand of the Kleene plus expression.</param>
public class KleenePlus(AlangRegex operand) : UnaryExpr(operand)
{
    /// <inheritdoc/>
    public override int Precedence => 5;

    /// <inheritdoc/>
    public override string AlangExpressionString => $"{Param(Operand, this)}{Chars.KleenePlus}";
}
