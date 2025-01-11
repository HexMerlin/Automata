namespace Automata.Core.Alang;


/// <summary>
/// Represents an empty set expression in the <c>Alang</c> (Automata language).
/// </summary>
/// <remarks>
/// This class is used to define an empty set in finite-state automata expressions.
/// </remarks>
public class EmptySet : AlangExpr
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptySet"/> class.
    /// </summary>
    public EmptySet() : base() { }

    /// <inheritdoc/>
    public override int Precedence => 7;

    /// <inheritdoc/>
    public override string AlangExpressionString => "()";
}
