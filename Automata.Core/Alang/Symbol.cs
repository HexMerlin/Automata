namespace Automata.Core.Alang;

/// <summary>
/// Terminal in the <c>Alang</c> language used for defining finite-state automata.
/// </summary>
/// <remarks>
/// A terminal is a atomunit in the <c>Alang</c> language, consisting of one or more characters that are not
/// operators or whitespace. It serves as an atomic leaf node in the expression tree.
/// </remarks>
public class Symbol : AlangRegex
{
    /// <summary>
    /// Symbol representing this atom.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alang.Symbol"/> class with the specified symbol.
    /// </summary>
    /// <param name="symbol">Symbol representing this atom.</param>
    public Symbol(string symbol) : base()
        => Symbol = symbol;

    ///<inheritdoc/>
    public override int Precedence => 7;

    /// <summary>
    /// String representation of this atom.
    /// </summary>
    public override string AlangExpressionString => Symbol;
}
