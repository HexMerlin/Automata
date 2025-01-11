namespace Automata.Core;

/// <summary>
/// Represents a mutable alphabet associated with a finite state automaton.
/// </summary>
public class MutableAlphabet : IAlphabet
{
    #region Data
    private readonly List<string> indexToStringMap;
    private readonly Dictionary<string, int> stringToIndexMap;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableAlphabet"/> class.
    /// </summary>
    public MutableAlphabet()
    {
        stringToIndexMap = [];
        indexToStringMap = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableAlphabet"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to initialize the alphabet with.</param>
    public MutableAlphabet(IEnumerable<string> symbols) : this() => AddAll(symbols);

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableAlphabet"/> class from an existing <see cref="CanonicalAlphabet"/>.
    /// </summary>
    /// <param name="alphabet">The canonical alphabet to initialize from.</param>
    public MutableAlphabet(CanonicalAlphabet alphabet)
    {
        this.indexToStringMap = new(alphabet.Symbols);
        this.stringToIndexMap = new Dictionary<string, int>(alphabet.StringToIndexMap);
    }

    /// <summary>
    /// Gets the number of symbols in the alphabet.
    /// </summary>
    public int Count => indexToStringMap.Count;

    /// <summary>
    /// Gets a read-only collection of symbols in the alphabet.
    /// </summary>
    public IReadOnlyCollection<string> Symbols => indexToStringMap;

    /// <summary>
    /// Gets the symbol at the specified index.
    /// </summary>
    /// <param name="index">The index of the symbol to get.</param>
    /// <returns>The symbol at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    public string this[int index] => index >= 0 && index < Count ? indexToStringMap[index] : throw new ArgumentOutOfRangeException($"Symbol with index {index} does not exist in alphabet");

    /// <summary>
    /// Gets the index of the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol whose index to get.</param>
    /// <returns>The index of the specified symbol, or <see cref="Constants.InvalidSymbolIndex"/> if not found.</returns>
    public int this[string symbol] => stringToIndexMap.TryGetValue(symbol, out int index) ? index : Constants.InvalidSymbolIndex;

    /// <summary>
    /// Indicates whether the alphabet contains the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol to locate in the alphabet.</param>
    /// <returns><see langword="true"/> if the symbol is found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(string symbol) => stringToIndexMap.ContainsKey(symbol);

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
    /// Adds all the specified symbols to the alphabet that are not already present.
    /// </summary>
    /// <param name="symbols">The symbols to add.</param>
    public void AddAll(IEnumerable<string> symbols)
    {
        foreach (string symbol in symbols)
            GetOrAdd(symbol);
    }

    /// <summary>
    /// Returns a string with each symbol and its index, separated by a newline.
    /// </summary>
    /// <returns>A string with each symbol and its index, separated by a newline.</returns>
    public string ToStringExpanded() => string.Join("\n", Enumerable.Range(0, Count).Select(i => $"{i}: {this[i]}"));

    /// <summary>
    /// Returns a string that represents the current alphabet, including its size.
    /// </summary>
    /// <returns>A string representation of the alphabet.</returns>
    public override string ToString() => $"Alphabet, size: {Count}";
}
