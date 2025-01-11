﻿
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
    #endregion

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

    public bool Contains(int item) => Members.Contains(item);
    public bool IsProperSubsetOf(IEnumerable<int> other) => Members.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<int> other) => Members.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<int> other) => Members.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<int> other) => Members.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<int> other) => Members.Overlaps(other);
    public bool SetEquals(IEnumerable<int> other) => Members.SetEquals(other);
    public IEnumerator<int> GetEnumerator() => Members.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
