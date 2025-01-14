namespace Automata.Core.Alang;


/// <summary>
/// Represents the empty language (∅) in <c>Alang</c> (Automata language).
/// </summary>
/// <remarks>
/// It is a valid expression in the <c>Alang</c> language, denoted by the empty parentheses <c>()</c>.
/// </remarks>
public class EmptyLang : AlangRegex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyLang"/> class.
    /// </summary>
    public EmptyLang() : base() { }

    /// <inheritdoc/>
    public override int Precedence => 7;

    /// <inheritdoc/>
    public override string AlangExpressionString => "()";
}
