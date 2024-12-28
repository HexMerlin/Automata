namespace Automata.Core.Alphabets;

public interface IAlphabet
{
    string this[int index] { get; }
    int this[string symbol] { get; }
    int Count { get; }

    /// <summary>
    /// Gets the index of the specified symbol or adds it if it does not exist.
    /// </summary>
    /// <param name="symbol">The symbol to get or add.</param>
    /// <returns>The index of the specified symbol.</returns>
    public int GetOrAdd(string symbol);

    string ToString();
    string ToStringExpanded();
}