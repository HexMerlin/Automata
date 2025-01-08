namespace Automata.Core;

/// <summary>
/// Represents a mutable alphabet associated with a finite state automaton.
/// </summary>
public class MutableAlphabet : IAlphabet
{

    #region Data
    private readonly List<string> indexToStringMap = [];

    private readonly Dictionary<string, int> stringToIndexMap = [];
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableAlphabet"/> class.
    /// </summary>
    public MutableAlphabet() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableAlphabet"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to initialize the alphabet with.</param>
    public MutableAlphabet(IEnumerable<string> symbols) : this() => AddAll(symbols);

    ///<inheritdoc/>
    public int Count => indexToStringMap.Count;

    ///<inheritdoc/>
    public string this[int index] => index >= 0 && index < Count ? indexToStringMap[index] : throw new ArgumentOutOfRangeException($"Symbol with index {index} does not exist in alphabet");

    ///<inheritdoc/>
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

    /// <returns>A string with each symbol and its index, separated by a newline.</returns>
    public string ToStringExpanded() => string.Join("\n", Enumerable.Range(0, Count).Select(i => $"{i}: {this[i]}"));

    /// <returns>A string that represents the current alphabet, including its size.</returns>
    public override string ToString() => $"Alphabet, size: {Count}";


}
