namespace Automata.Core.Alang;

/// <summary>
/// Represents an option expression in the Alang language.
/// </summary>
/// <remarks>
/// An option expression matches zero or one occurrence of its operand.
/// </remarks>
/// <param name="operand">The operand of the option expression.</param>
public class Option(AlangRegex operand) : UnaryRegex(operand)
{
    /// <inheritdoc/>
    public override int Precedence => 5;

    /// <inheritdoc/>
    public override string AlangExpressionString => $"{Param(Operand, this)}{Chars.Option}";
}
