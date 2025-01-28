using System.Diagnostics;

namespace Automata.Core;

/// <summary>
/// Immutable set of integers.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class IntSet : HashSet<int>, IEquatable<IntSet>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IntSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">Elements to include in the set.</param>
    public IntSet(IEnumerable<int> elements) : base(elements) { }

    private IntSet() : base() { }

    public (IntSet intersection, IntSet difference) Split(IEnumerable<int> other)
    {
        var intersection = new IntSet(this);
        intersection.IntersectWith(other);
        var difference = new IntSet(this);
        difference.IntersectWith(intersection);
        return (intersection, difference);
    }

    /// <summary>
    /// Indicates whether the current set is equal to another set.
    /// </summary>
    /// <param name="other">Other set to compare to.</param>
    /// <returns><see langword="true"/> <c>iff</c> the sets are equal.</returns>
    public bool Equals(IntSet? other)
        => other != null && SetEquals(other);

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
    {
        int hash = 17;
        int count = 0;
        foreach (int element in this)
        {
            hash = hash * 31 + element;
            if (++count == 10) break;
        }
        return hash;
    }

    /// <summary>
    /// String that represents the current set.
    /// </summary>
    /// <returns>A string that represents the current set.</returns>
    public override string ToString()
        => Count <= 10 ? string.Join(", ", this) : string.Join(", ", this.Take(10)) + ", ...";

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="true"/> <c>iff</c> the operands are equal.</returns>
    public static bool operator ==(IntSet? left, IntSet? right) => Equals(left, right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="true"/> <c>iff</c> the operands are not equal.</returns>
    public static bool operator !=(IntSet? left, IntSet? right) => !Equals(left, right);

    #endregion Accessors
}
