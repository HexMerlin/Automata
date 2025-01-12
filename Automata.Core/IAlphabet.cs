using System.Collections.Frozen;

namespace Automata.Core;

/// <summary>
/// Common interface for an alphabet associated with a finite state automaton.
/// </summary>
public interface IAlphabet
{
    /// <summary>
    /// Number of symbols in the alphabet.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Symbol at the specified index.
    /// </summary>
    /// <param name="index">Index of the symbol.</param>
    /// <returns>Symbol at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    string this[int index] { get; }

    /// <summary>
    /// Index of the specified symbol.
    /// </summary>
    /// <param name="symbol">Symbol to get the index of.</param>
    /// <returns>Index of the specified symbol, or <see cref="Constants.InvalidSymbolIndex"/> if the symbol does not exist.</returns>
    int this[string symbol] { get; }

    /// <summary>
    /// Indicates whether the alphabet contains the specified symbol.
    /// </summary>
    /// <param name="symbol">Symbol to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the alphabet contains the symbol.</returns>
    bool Contains(string symbol);

    /// <summary>
    /// Read-only collection of symbols in the alphabet.
    /// </summary>
    IReadOnlyCollection<string> Symbols { get; }

    /// <summary>
    /// Mapping from string symbols to their respective indices.
    /// </summary>
    IReadOnlyDictionary<string, int> StringToIndexMap { get; }

    /// <summary>
    /// String representation of the current alphabet.
    /// </summary>
    /// <returns>A string representation of the current alphabet.</returns>
    string ToString();

    /// <summary>
    /// String with each symbol and its index, separated by a newline.
    /// </summary>
    /// <returns>A string with each symbol and its index, separated by a newline.</returns>
    string ToStringExpanded();
}
