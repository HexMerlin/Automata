namespace Automata.Core;

/// <summary>
/// An alphabet associated with a finite state automaton.
/// </summary>
/// <remarks>
/// An <see cref="Alphabet"/> is a collection of symbols used by a finite state automaton.
/// It can be extended with new symbols, but symbols cannot be removed. 
/// This enables multiple automata to share the same alphabet. 
/// Furthermore, automata never needs to deals with strings, but can use integer indices instead for all operations.
/// Default behavior for operations that create new automata is to use the existing alphabet of the input automata.
/// If a new alphabet is needed, an explicit creation of a new alphabet is typically required.
/// </remarks>
public class Alphabet : IEquatable<Alphabet>
{
    #region Data

    private readonly List<string> indexToStringMap;
    private readonly Dictionary<string, int> stringToIndexMap;
    
    #endregion

    /// <summary>
    /// Initializes a new empty instance of the <see cref="Alphabet"/> class.
    /// </summary>
    public Alphabet()
    {
        stringToIndexMap = [];
        indexToStringMap = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alphabet"/> class with the specified symbols.
    /// <param name="symbols">Symbols to initialize the alphabet with.</param>
    /// </summary>
    public Alphabet(params string[] symbols) : this() => AddAll(symbols);

    /// <summary>
    /// Initializes a new instance of the <see cref="Alphabet"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">Symbols to initialize the alphabet with.</param>
    public Alphabet(IEnumerable<string> symbols) : this() => AddAll(symbols);

    /// <summary>
    /// Initializes a new cloned instance of the <see cref="Alphabet"/> class from the specified alphabet.
    /// </summary>
    /// <param name="alphabet">Alphabet to initialize the alphabet with.</param>
    public Alphabet(Alphabet alphabet)
    {
        this.stringToIndexMap = new Dictionary<string, int>(alphabet.stringToIndexMap);
        this.indexToStringMap = new(alphabet.Symbols);
    }

    /// <summary>
    /// Number of symbols in the alphabet.
    /// </summary>
    public int Count => indexToStringMap.Count;

    /// <summary>
    /// Readonly collection of symbols in the alphabet.
    /// </summary>
    public IReadOnlyCollection<string> Symbols => indexToStringMap;

    /// <summary>
    /// Returns an enumerable collection of indices of the symbols in the alphabet.
    /// This is effectively the integers in range [0 .. Count).
    /// </summary>
    public IEnumerable<int> SymbolIndices => Enumerable.Range(0, Count);

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
    /// Indicates whether the alphabet contains the specified symbol as an integer.
    /// </summary>
    /// <param name="symbol">Symbol index to locate in the alphabet.</param>
    /// <returns><see langword="true"/> <c>iff</c> the symbol is found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(int symbol) => symbol >= 0 && symbol < this.Count;

    /// <summary>
    /// Indicates whether the alphabet contains the specified symbol as a string.
    /// </summary>
    /// <param name="symbol">Symbol string to locate in the alphabet.</param>
    /// <returns><see langword="true"/> <c>iff</c> the symbol is found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(string symbol) => stringToIndexMap.ContainsKey(symbol);

    /// <summary>
    /// Tries to get the index of the specified symbol.
    /// </summary>
    /// <param name="symbol">Symbol whose index to get.</param>
    /// <param name="index">When this method returns, contains the index of the specified symbol, if the symbol is found; otherwise, -1.</param>
    /// <returns><see langword="true"/> <c>iff</c> the symbol is found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetIndex(string symbol, out int index) => stringToIndexMap.TryGetValue(symbol, out index);

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
    /// Add another alphabet to the current alphabet.
    /// </summary>
    /// <param name="other">The other alphabet to merge into this.</param>
    /// <returns>A dictionary mapping indices from the other alphabet to the current alphabet.</returns>
    public Dictionary<int, int> UnionWith(Alphabet other)
    {
        var mapOtherToThis = new Dictionary<int, int>();

        foreach (KeyValuePair<string, int> entry in other.stringToIndexMap)
        {
            string otherSymbol = entry.Key;
            int otherIndex = entry.Value;
            int index = GetOrAdd(otherSymbol);
            mapOtherToThis[otherIndex] = index;
        }
        return mapOtherToThis;
    }

    /// <summary>
    /// Adds all the specified symbols to the alphabet that are not already present.
    /// </summary>
    /// <param name="symbols">Symbols to add.</param>
    public void AddAll(IEnumerable<string> symbols)
    {
        foreach (string symbol in symbols)
            _ = GetOrAdd(symbol);
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

    ///<inheritdoc/>
    public bool Equals(Alphabet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Count != other.Count) return false;
        return indexToStringMap.SequenceEqual(other.indexToStringMap);
    }

    ///<inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Alphabet);

    ///<inheritdoc/>
    public static bool operator ==(Alphabet left, Alphabet right) => left.Equals(right);

    ///<inheritdoc/>
    public static bool operator !=(Alphabet left, Alphabet right) => !left.Equals(right);
    
    ///<inheritdoc/>
    public override int GetHashCode() => indexToStringMap.Aggregate(new HashCode(), (hash, symbol) => { hash.Add(symbol); return hash; }).ToHashCode();
}
