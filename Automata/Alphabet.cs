namespace Automata;
public class Alphabet
{
    #region Instance
    private readonly List<string> indexToStringMap = new();

    private readonly Dictionary<string, int> stringToIndexMap = new();
    #endregion
    
    public const int InvalidIndex = -1;

    public Alphabet() { }

    public int Count => indexToStringMap.Count;

    public string this[int index] => index >= 0 && index < Count ? indexToStringMap[index] : throw new ArgumentOutOfRangeException($"Symbol with index {index} does not exist in alphabet");

    public int this[string symbol] => stringToIndexMap.TryGetValue(symbol, out int index) ? index : InvalidIndex;

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
