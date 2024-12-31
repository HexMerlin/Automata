using System.Collections.Frozen;

namespace Automata.Core.Alphabets;

/// <summary>
/// Immutable and optimized representation of an alphabet with contiguous, ordered symbols.
/// </summary>
/// <remarks>
/// A <see cref="CanonicalAlphabet"/> is defined by the following properties:
/// <list type="number">
/// <item>
/// <description>
/// <b>Immutable:</b> Structural and behavioral invariance, ensuring thread safety and predictable behavior.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Performance:</b> Optimized for fast, read-only operations with minimal memory overhead.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Ordering:</b> Symbols are sorted according to <see cref="CanonicalStringComparer"/>.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Indexing:</b> Symbols are associated with contiguous, ordered integer indices <c>[0..Count)</c>.
/// </description>
/// </item>
/// </list>
/// </remarks>
/// <seealso cref="Cfa"/>
public class CanonicalAlphabet : IEquatable<CanonicalAlphabet>, IAlphabet
{ 
    #region Data
    private readonly string[] indexToStringMap;

    private readonly FrozenDictionary<string, int> stringToIndexMap;
    #endregion Data

    /// <summary>
    /// An empty <see cref="CanonicalAlphabet"/> without any symbols.
    /// </summary>
    public static CanonicalAlphabet Empty => new CanonicalAlphabet(Array.Empty<string>());

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonicalAlphabet"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to initialize the alphabet with.</param>
    public CanonicalAlphabet(IEnumerable<string> symbols)
    {
        indexToStringMap = [.. symbols.Distinct().OrderBy(s => s, CanonicalStringComparer)];

        stringToIndexMap = 
            indexToStringMap
            .Select((symbol, index) => KeyValuePair.Create(symbol, index))
            .ToFrozenDictionary();
    }

    /// <summary>
    /// Canonical string comparer used by an alphabet, used to ensure canonical ordering of symbols, when required.
    /// </summary>
    public static StringComparer CanonicalStringComparer => StringComparer.Ordinal;

    /// <summary>
    /// Gets the number of symbols in the alphabet.
    /// </summary>
    public int Count => indexToStringMap.Length;

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

    public int GetOrAdd(string symbol) => throw new NotSupportedException($"Adding to immutable {nameof(CanonicalAlphabet)} is not supported.");
   
    /// <returns>A string with each symbol and its index, separated by a newline.</returns>
    public string ToStringExpanded() => string.Join("\n", Enumerable.Range(0, Count).Select(i => $"{i}: {this[i]}"));

    /// <returns>A string that represents the current alphabet, including its size.</returns>
    public override string ToString() => $"Alphabet, size: {Count}";

    ///<summary>
    ///Indicates whether the current alphabet is equal (identical) to another alphabet.
    ///</summary>
    public bool Equals(CanonicalAlphabet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (this.Count != other.Count) return false;
        if (!this.indexToStringMap.SequenceEqual(other.indexToStringMap, CanonicalStringComparer)) return false;
        return true;
    }

    ///<inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CanonicalAlphabet);

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (int i = 0; i < indexToStringMap.Length; i++)
            hash.Add(indexToStringMap[i]);
        return hash.ToHashCode();
    }

    public static bool operator ==(CanonicalAlphabet left, CanonicalAlphabet right) => left.Equals(right);

    public static bool operator !=(CanonicalAlphabet left, CanonicalAlphabet right) => !left.Equals(right);

}
