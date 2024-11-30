
using System.Collections.Frozen;
using System.Diagnostics;

namespace Automata;

[DebuggerDisplay("{ToString()}")]
public class IntSet : IEquatable<IntSet>
{

    #region Instance
    public FrozenSet<int> Members { get; }
    
    #endregion

    public IntSet(IEnumerable<int> elements) 
        => Members = elements.ToFrozenSet();

    public IntSet(ISet<int> elements) 
        => Members = elements.ToFrozenSet();

    public int Count => Members.Count;

    public bool Equals(IntSet? other) 
        => other != null && Members.SetEquals(other.Members);

    public override bool Equals(object? obj) 
        => Equals(obj as IntSet);

    public override int GetHashCode() 
        => Members.Aggregate(0, (hashCode, element) => hashCode ^ element.GetHashCode());

    public override string ToString() 
        => Count <= 10 ? string.Join(", ", Members) : string.Join(", ", Members.Take(10)) + ", ...";
}
