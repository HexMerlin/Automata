﻿using System.Collections.Frozen;

namespace Automata.Core;

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

    /// <summary>
    /// Mapping from string symbols to their respective indices.
    /// </summary>
    public readonly FrozenDictionary<string, int> StringToIndexMap;
    #endregion Data

    /// <summary>
    /// An empty <see cref="CanonicalAlphabet"/> without any symbols.
    /// </summary>
    public static CanonicalAlphabet Empty => new CanonicalAlphabet(Array.Empty<string>());

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonicalAlphabet"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">Symbols to initialize the alphabet with.</param>
    public CanonicalAlphabet(IEnumerable<string> symbols)
    {
        indexToStringMap = [.. symbols.Distinct().OrderBy(s => s, CanonicalStringComparer)];

        StringToIndexMap =
            indexToStringMap
            .Select((symbol, index) => KeyValuePair.Create(symbol, index))
            .ToFrozenDictionary();
    }

    /// <summary>
    /// Canonical string comparer used by an alphabet, used to ensure canonical ordering of symbols, when required.
    /// </summary>
    public static StringComparer CanonicalStringComparer => StringComparer.Ordinal;

    /// <summary>
    /// Number of symbols in the alphabet.
    /// </summary>
    public int Count => indexToStringMap.Length;

    ///<inheritdoc/>
    public IReadOnlyCollection<string> Symbols => indexToStringMap;

    ///<inheritdoc/>
    IReadOnlyDictionary<string, int> IAlphabet.StringToIndexMap => StringToIndexMap;

    ///<inheritdoc/>
    public string this[int index] => index >= 0 && index < Count ? indexToStringMap[index] : throw new ArgumentOutOfRangeException($"Symbol with index {index} does not exist in alphabet");

    ///<inheritdoc/>
    public int this[string symbol] => StringToIndexMap.TryGetValue(symbol, out int index) ? index : Constants.InvalidSymbolIndex;

    ///<inheritdoc/>
    public bool Contains(string symbol) => StringToIndexMap.ContainsKey(symbol);

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

    ///<summary>
    ///Indicates whether the current alphabet is equal (identical) to another alphabet.
    ///</summary>
    public bool Equals(CanonicalAlphabet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Count != other.Count) return false;
        if (!this.indexToStringMap.SequenceEqual(other.indexToStringMap, CanonicalStringComparer)) return false;
        return true;
    }

    ///<inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CanonicalAlphabet);

    /// <summary>
    /// Hash code for the current alphabet.
    /// </summary>
    /// <returns>A hash code for the alphabet.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (int i = 0; i < indexToStringMap.Length; i++)
            hash.Add(indexToStringMap[i]);
        return hash.ToHashCode();
    }

    /// <summary>
    /// Indicates whether two specified instances of <see cref="CanonicalAlphabet"/> are equal.
    /// </summary>
    /// <param name="left">First alphabet to compare.</param>
    /// <param name="right">Second alphabet to compare.</param>
    /// <returns><see langword="true"/> <c>iff</c> the two alphabets are equal.</returns>
    public static bool operator ==(CanonicalAlphabet left, CanonicalAlphabet right) => left.Equals(right);

    /// <summary>
    /// Indicates whether two specified instances of <see cref="CanonicalAlphabet"/> are not equal.
    /// </summary>
    /// <param name="left">First alphabet to compare.</param>
    /// <param name="right">Second alphabet to compare.</param>
    /// <returns><see langword="true"/> <c>iff</c> the two alphabets are not equal.</returns>
    public static bool operator !=(CanonicalAlphabet left, CanonicalAlphabet right) => !left.Equals(right);

}
