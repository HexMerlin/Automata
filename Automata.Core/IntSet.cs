using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Automata.Core;

/// <summary>
/// Immutable set of integers.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class IntSet : IEquatable<IntSet>, IReadOnlySet<int>
{
    #region Data
    private readonly HashSet<int> set;
    private readonly int hashCode;

    #endregion Data

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IntSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">Elements to include in the set.</param>
    public IntSet(IEnumerable<int> elements)
    {
        this.set = new(elements);
        this.hashCode = ComputeHashCode();
    }

    #endregion Constructors

    #region Accessors

    /// <summary>
    /// Number of elements in the set.
    /// </summary>
    public int Count => set.Count;

    /// <summary>
    /// Indicates whether the current set is equal to another set.
    /// </summary>
    /// <param name="other">Other set to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the sets are equal.</returns>
    public bool Equals(IntSet? other)
        => other is not null && this.hashCode == other.hashCode && set.SetEquals(other.set);

    /// <summary>
    /// Indicates whether the current set is equal to another object.
    /// </summary>
    /// <param name="obj">Object to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the object is an <see cref="IntSet"/> and the sets are equal.</returns>
    public override bool Equals(object? obj)
        => Equals(obj as IntSet);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeHashCode()
    {
        unchecked
        {
            int hash = 1610612741;
            foreach (int value in this.set)
            {
                int mixed = value * 16777619;
                hash ^= (mixed ^ (mixed >> 16));
            }
            return (hash ^ (hash >> 16));
        }
    }


    /// <summary>
    /// Hash code for the current set.
    /// </summary>
    /// <returns>The hash code for the current set.</returns>
    public override int GetHashCode() => this.hashCode;


    /// <summary>
    /// String that represents the current set.
    /// </summary>
    /// <returns>A string that represents the current set.</returns>
    public override string ToString()
        => Count <= 10 ? string.Join(", ", this) : string.Join(", ", this.set.Take(10)) + ", ...";

    /// <summary>
    /// Indicates whether the current set contains the specified item.
    /// </summary>
    /// <param name="item">Item to locate in the set.</param>
    /// <returns><see langword="true"/> <c>iff</c> the set contains the specified item.</returns>
    public bool Contains(int item) => this.set.Contains(item);

    /// <summary>
    /// Indicates whether the current set overlaps with the specified collection.
    /// </summary>
    /// <param name="other">Collection to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the current set overlaps with the specified collection.</returns>
    public bool Overlaps(IEnumerable<int> other) => set.Overlaps(other);

    /// <summary>
    /// Indicates whether the current set is a proper subset of the specified collection.
    /// </summary>
    /// <param name="other">Collection to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the current set is a proper subset of the specified collection.</returns>
    public bool IsProperSubsetOf(IEnumerable<int> other) => this.set.IsProperSubsetOf(other);

    /// <summary>
    /// Indicates whether the current set is a proper superset of the specified collection.
    /// </summary>
    /// <param name="other">Collection to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the current set is a proper superset of the specified collection.</returns>
    public bool IsProperSupersetOf(IEnumerable<int> other) => this.set.IsProperSupersetOf(other);

    /// <summary>
    /// Indicates whether the current set is a subset of the specified collection.
    /// </summary>
    /// <param name="other">Collection to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the current set is a subset of the specified collection.</returns>
    public bool IsSubsetOf(IEnumerable<int> other) => this.set.IsSubsetOf(other);

    /// <summary>
    /// Indicates whether the current set is a superset of the specified collection.
    /// </summary>
    /// <param name="other">Collection to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the current set is a superset of the specified collection.</returns>
    public bool IsSupersetOf(IEnumerable<int> other) => this.set.IsSupersetOf(other);

    /// <summary>
    /// Indicates whether the current set is equal to the specified collection.
    /// </summary>
    /// <param name="other">Collection to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the current set is equal to the specified collection.</returns>
    public bool SetEquals(IEnumerable<int> other) => this.set.SetEquals(other);

    /// <summary>
    /// Returns an enumerator that iterates through the set.
    /// </summary>
    /// <returns>An enumerator for the set.</returns>
    public IEnumerator<int> GetEnumerator() => this.set.GetEnumerator();
    
    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="true"/> <c>iff</c> the operands are equal.</returns>
    public static bool operator ==(IntSet left, IntSet right) 
        => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="true"/> <c>iff</c> the operands are not equal.</returns>
    public static bool operator !=(IntSet left, IntSet right) => !(left == right);

    #endregion Accessors
}
