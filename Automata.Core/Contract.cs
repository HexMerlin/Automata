using System.Runtime.CompilerServices;

namespace Automata.Core;

/// <summary>
/// Asserts conditions and throws exceptions.
/// </summary>
public static class Contract
{
    /// <summary>
    /// Asserts <paramref name="value"/> is within <paramref name="range"/>.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <param name="range">Valid range (end-exclusive).</param>
    /// <param name="paramName">Captured automatically.</param>
    /// <returns><paramref name="value"/> <c>iff</c> <paramref name="value"/> is within  <paramref name="range"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown <c>iff</c> <paramref name="value"/> is not within <paramref name="range"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ShouldBeInRange(
        this int value,
        Range range,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, range.Start.Value, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, range.End.Value, paramName);
        return value;
    }

    /// <summary>
    /// Assert <paramref name="value"/> is not negative.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <param name="paramName">Captured automatically.</param>
    /// <returns>
    /// <paramref name="value"/> <c>iff</c> <paramref name="value"/> is zero or positive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown <c>iff</c> <paramref name="value"/> is negative.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ShouldNotBeNegative(
        this int value,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
        return value;
    }

}
