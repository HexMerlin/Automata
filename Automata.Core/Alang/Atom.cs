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

    /// <summary>
    /// Gets the precedence level of this atom.
    /// </summary>
    /// <remarks>
    /// The precedence level for an atom is set to <c>10</c>, indicating it binds most tightly in expressions.
    /// </remarks>
    public override int Precedence => 10;

    /// <summary>
    /// Gets the string representation of this atom.
    /// </summary>
    public override string ExpressionString => Symbol;
}
