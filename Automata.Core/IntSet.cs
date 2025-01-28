
using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics;

namespace Automata.Core;

/// <summary>
/// Immutable set of integers.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class IntSet : IEquatable<IntSet>, IReadOnlySet<int>
{
    #region Data
    /// <summary>
    /// Members of the set.
    /// </summary>
    private FrozenSet<int> Members { get; }

    #endregion Data

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IntSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">Elements to include in the set.</param>
    public IntSet(IEnumerable<int> elements)
        => Members = elements.ToFrozenSet();

    /// <summary>
    /// Initializes a new instance of the <see cref="IntSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">Elements to include in the set.</param>
    public IntSet(HashSet<int> elements)
        => Members = elements.ToFrozenSet();

    /// <summary>
    /// Initializes a new instance of the <see cref="IntSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">Elements to include in the set.</param>
    public IntSet(ISet<int> elements)
        => Members = elements.ToFrozenSet();

    #endregion Constructors

    #region Accessors

    /// <summary>
    /// Number of elements in the set.
    /// </summary>
    public int Count => Members.Count;

    /// <summary>
    /// Indicates whether the current set is equal to another set.
    /// </summary>
    /// <param name="other">Other set to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the sets are equal.</returns>
    public bool Equals(IntSet? other)
        => other != null && Members.SetEquals(other.Members);

    /// <summary>
    /// Indicates whether the current set is equal to another object.
    /// </summary>
    /// <param name="obj">Object to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the object is an <see cref="IntSet"/> and the sets are equal.</returns>
    public override bool Equals(object? obj)
        => Equals(obj as IntSet);

    /// <summary>
    /// Hash code for the current set.
    /// </summary>
    /// <returns>The hash code for the current set.</returns>
    public override int GetHashCode()
        => Members.Aggregate(0, (hashCode, element) => hashCode ^ element.GetHashCode());

    /// <summary>
    /// String that represents the current set.
    /// </summary>
    /// <returns>A string that represents the current set.</returns>
    public override string ToString()
        => Count <= 10 ? string.Join(", ", Members) : string.Join(", ", Members.Take(10)) + ", ...";

    /// <summary>
    /// Indicates whether the set contains the specified <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item to locate in the set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set contains the specified <paramref name="item"/>.</returns>
    public bool Contains(int item) => Members.Contains(item);

    /// <summary>
    /// Indicates whether the set is a proper subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set is a proper subset of the specified collection.</returns>
    public bool IsProperSubsetOf(IEnumerable<int> other) => Members.IsProperSubsetOf(other);

    /// <summary>
    /// Indicates whether the set is a proper superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set is a proper superset of the specified collection.</returns>
    public bool IsProperSupersetOf(IEnumerable<int> other) => Members.IsProperSupersetOf(other);

    /// <summary>
    /// Indicates whether the set is a subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set is a subset of the specified collection.</returns>
    public bool IsSubsetOf(IEnumerable<int> other) => Members.IsSubsetOf(other);

    /// <summary>
    /// Indicates whether the set is a superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set is a superset of the specified collection.</returns>
    public bool IsSupersetOf(IEnumerable<int> other) => Members.IsSupersetOf(other);

    /// <summary>
    /// Indicates whether the set overlaps with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set overlaps with the specified collection.</returns>
    public bool Overlaps(IEnumerable<int> other) => Members.Overlaps(other);

    /// <summary>
    /// Indicates whether the set is equal to the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set is equal to the specified collection.</returns>
    public bool SetEquals(IEnumerable<int> other) => Members.SetEquals(other);

    /// <summary>
    /// Intersection of the current set with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>A new <see cref="IntSet"/> that are both in the current set and in the specified collection.</returns>
    public IntSet Intersect(IEnumerable<int> other) => new(Members.Intersect(other));

    /// <summary>
    /// Difference of the current set with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>A new <see cref="IntSet"/> containing elements that are in the current set but not in the specified collection.</returns>
    public IntSet Except(IEnumerable<int> other) => new(Members.Except(other));

    /// <summary>
    /// Returns an enumerator that iterates through the set.
    /// </summary>
    /// <returns>An enumerator for the set.</returns>
    public IEnumerator<int> GetEnumerator() => Members.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the set.
    /// </summary>
    /// <returns>An enumerator for the set.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion Accessors
}
