namespace Automata;

/// <summary>
/// Represents an alphabet used in a finite state automaton.
/// </summary>
public class Alphabet
{
    #region Data
    private readonly List<string> indexToStringMap = new();

    private readonly Dictionary<string, int> stringToIndexMap = new();
    #endregion

    /// <summary>
    /// Represents an invalid index.
    /// </summary>
    public const int InvalidIndex = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Alphabet"/> class.
    /// </summary>
    public Alphabet() { }

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
    /// <returns>The index of the specified symbol, or <see cref="InvalidIndex"/> if the symbol does not exist.</returns>
    public int this[string symbol] => stringToIndexMap.TryGetValue(symbol, out int index) ? index : InvalidIndex;

    /// <summary>
    /// Gets the index of the specified symbol, adding it to the alphabet if it does not already exist.
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
}
