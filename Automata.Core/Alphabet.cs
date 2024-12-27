using System;

namespace Automata.Core;

/// <summary>
/// Represents an alphabet used in a finite state automaton.
/// </summary>
public class Alphabet
{
    #region Data
    private readonly List<string> indexToStringMap = [];

    private readonly Dictionary<string, int> stringToIndexMap = [];
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Alphabet"/> class.
    /// </summary>
    public Alphabet() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alphabet"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to initialize the alphabet with.</param>
    public Alphabet(IEnumerable<string> symbols) : this() => AddAll(symbols);

    /// <summary>
    /// Gets the number of symbols in the alphabet.
    /// </summary>
    public int Count => indexToStringMap.Count;

    /// <summary>
    /// Gets the symbol at the specified index.
    /// </summary>
    /// <param name="index">The index of the symbol.</param>
    /// <returns>The symbol at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    public string this[int index] => index >= 0 && index < Count ? indexToStringMap[index] : throw new ArgumentOutOfRangeException($"Symbol with index {index} does not exist in alphabet");

    /// <summary>
    /// Gets the index of the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol to get the index of.</param>
    /// <returns>The index of the specified symbol, or <see cref="Constants.InvalidSymbolIndex"/> if the symbol does not exist.</returns>
    public int this[string symbol] => stringToIndexMap.TryGetValue(symbol, out int index) ? index : Constants.InvalidSymbolIndex;

    /// <summary>
    /// Gets the index of the specified symbol or adds it if it does not exist.
    /// </summary>
    /// <param name="symbol">The symbol to get or add.</param>
    /// <returns>The index of the specified symbol.</returns>
    public int GetOrAdd(string symbol)
    {
        if (stringToIndexMap.TryGetValue(symbol, out int index))
            return index;
        index = Count;
        stringToIndexMap[symbol] = index;
        indexToStringMap.Add(symbol);
        return index;
    }

    /// <summary>
    /// Adds all the specified symbols to the alphabet, that are not already present.
    /// </summary>
    /// <param name="symbols">The symbols to add.</param>
    public void AddAll(IEnumerable<string> symbols)
    {
        foreach (string symbol in symbols)
            GetOrAdd(symbol);
    }
}
