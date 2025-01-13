namespace Automata.Core.Alang;

/// <summary>
/// Symbol in the <c>Alang</c> language used for defining finite-state automata.
/// </summary>
/// <remarks>
/// A Symbol is a atomic unit in the <c>Alang</c> language, consisting of one or more characters that are not
/// operators or whitespace. They are present as leaf nodes in the resulting parse tree.
/// </remarks>
public class Symbol : AlangRegex
{
    /// <summary>
    /// String value for this Symbol.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alang.Symbol"/> class with the specified symbol.
    /// </summary>
    /// <param name="symbol">String representing this Symbol.</param>
    public Symbol(string symbol) : base()
        => Value = symbol;

    ///<inheritdoc/>
    public override int Precedence => 7;

    ///<inheritdoc/>
    public override string AlangExpressionString => Value;
}
