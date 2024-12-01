
using System.Collections.Frozen;
using System.Diagnostics;

namespace Automata;

/// <summary>
/// Represents a set of integers with various utility methods.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class IntSet : IEquatable<IntSet>
{
    #region Data
    /// <summary>
    /// Gets the members of the set.
    /// </summary>
    public FrozenSet<int> Members { get; }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="IntSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">The elements to include in the set.</param>
    public IntSet(IEnumerable<int> elements)
        => Members = elements.ToFrozenSet();

    /// <summary>
    /// Initializes a new instance of the <see cref="IntSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">The elements to include in the set.</param>
    public IntSet(ISet<int> elements)
        => Members = elements.ToFrozenSet();

    /// <summary>
    /// Gets the number of elements in the set.
    /// </summary>
    public int Count => Members.Count;

    /// <summary>
    /// Indicates whether the current set is equal to another set.
    /// </summary>
    /// <param name="other">The other set to compare to.</param>
    /// <returns><c>true</c> if the sets are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(IntSet? other)
        => other != null && Members.SetEquals(other.Members);

    /// <summary>
    /// Indicates whether the current set is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns><c>true</c> if the object is an <see cref="IntSet"/> and the sets are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => Equals(obj as IntSet);

    /// <summary>
    /// Gets the hash code for the current set.
    /// </summary>
    /// <returns>The hash code for the current set.</returns>
    public override int GetHashCode()
        => Members.Aggregate(0, (hashCode, element) => hashCode ^ element.GetHashCode());

    /// <summary>
    /// Returns a string that represents the current set.
    /// </summary>
    /// <returns>A string that represents the current set.</returns>
    public override string ToString()
        => Count <= 10 ? string.Join(", ", Members) : string.Join(", ", Members.Take(10)) + ", ...";
}
