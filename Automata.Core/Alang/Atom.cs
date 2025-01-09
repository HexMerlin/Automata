namespace Automata.Core.Alang;

/// <summary>
/// Represents an atomic expression in the <c>Alang</c> language used for defining finite-state automata.
/// </summary>
/// <remarks>
/// An atom is a basic unit in the <c>Alang</c> language, consisting of one or more characters that are not
/// operators or whitespace. It serves as an atomic leaf node in the expression tree.
/// </remarks>
public class Atom : AlangExpr
{
    /// <summary>
    /// Gets the symbol representing this atom.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Atom"/> class with the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol representing this atom.</param>
    public Atom(string symbol) : base()
        => Symbol = symbol;

    ///<inheritdoc/>
    public override int Precedence => 7;

    /// <summary>
    /// Gets the string representation of this atom.
    /// </summary>
    public override string AlangExpressionString => Symbol;
}
