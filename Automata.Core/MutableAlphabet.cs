namespace Automata.Core;

/// <summary>
/// Mutable alphabet associated with a finite state automaton.
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
    /// <param name="symbols">Symbols to initialize the alphabet with.</param>
    public MutableAlphabet(IEnumerable<string> symbols) : this() => AddAll(symbols);

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableAlphabet"/> class from the specified alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet to initialize the mutable alphabet with.</param>
    public MutableAlphabet(IAlphabet alphabet)
    {
        this.stringToIndexMap = new Dictionary<string, int>(alphabet.StringToIndexMap);
        this.indexToStringMap = new(alphabet.Symbols);
    }

    /// <summary>
    /// Number of symbols in the alphabet.
    /// </summary>
    public int Count => indexToStringMap.Count;

    /// <summary>
    /// Read-only collection of symbols in the alphabet.
    /// </summary>
    public IReadOnlyCollection<string> Symbols => indexToStringMap;

    /// <summary>
    /// Mapping from string symbols to their respective indices.
    /// </summary>
    IReadOnlyDictionary<string, int> StringToIndexMap => stringToIndexMap;

    /// <summary>
    /// Mapping from string symbols to their respective indices.
    /// </summary>
    IReadOnlyDictionary<string, int> IAlphabet.StringToIndexMap => stringToIndexMap;

    /// <summary>
    /// Symbol at the specified index.
    /// </summary>
    /// <param name="index">Index of the symbol to get.</param>
    /// <returns>Symbol at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    public string this[int index] => index >= 0 && index < Count ? indexToStringMap[index] : throw new ArgumentOutOfRangeException($"Symbol with index {index} does not exist in alphabet");

    /// <summary>
    /// Index of the specified symbol.
    /// </summary>
    /// <param name="symbol">Symbol whose index to get.</param>
    /// <returns>Index of the specified symbol, or <see cref="Constants.InvalidSymbolIndex"/> if not found.</returns>
    public int this[string symbol] => stringToIndexMap.TryGetValue(symbol, out int index) ? index : Constants.InvalidSymbolIndex;

    /// <summary>
    /// Indicates whether the alphabet contains the specified symbol.
    /// </summary>
    /// <param name="symbol">Symbol to locate in the alphabet.</param>
    /// <returns><see langword="true"/> <c>iff</c> the symbol is found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(string symbol) => stringToIndexMap.ContainsKey(symbol);

    /// <summary>
    /// Index of the specified symbol or adds it if it does not exist.
    /// </summary>
    /// <param name="symbol">Symbol to get or add.</param>
    /// <returns>Index of the specified symbol.</returns>
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
    /// <param name="symbols">Symbols to add.</param>
    public void AddAll(IEnumerable<string> symbols)
    {
        foreach (string symbol in symbols)
            GetOrAdd(symbol);
    }

    /// <summary>
    /// String with each symbol and its index, separated by a newline.
    /// </summary>
    /// <returns>A string with each symbol and its index, separated by a newline.</returns>
    public string ToStringExpanded() => string.Join("\n", Enumerable.Range(0, Count).Select(i => $"{i}: {this[i]}"));

    /// <summary>
    /// String that represents the current alphabet, including its size.
    /// </summary>
    /// <returns>A string representation of the alphabet.</returns>
    public override string ToString() => $"Alphabet, size: {Count}";
}
