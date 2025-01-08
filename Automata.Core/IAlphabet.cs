namespace Automata.Core;

/// <summary>
/// Common interface for an alphabet associated with a finite state automaton.
/// </summary>
public interface IAlphabet
{
    /// <summary>
    /// Gets the number of symbols in the alphabet.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the symbol at the specified index.
    /// </summary>
    /// <param name="index">The index of the symbol.</param>
    /// <returns>The symbol at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    string this[int index] { get; }

    /// <summary>
    /// Gets the index of the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol to get the index of.</param>
    /// <returns>The index of the specified symbol, or <see cref="Constants.InvalidSymbolIndex"/> if the symbol does not exist.</returns>
    int this[string symbol] { get; }

    /// <summary>
    /// Returns a string representation of the current alphabet,.
    /// </summary>
    string ToString();

    /// <summary>
    /// Returns a string with each symbol and its index, separated by a newline.
    /// </summary>
    /// <returns>A string with each symbol and its index, separated by a newline.</returns>
    string ToStringExpanded();
}