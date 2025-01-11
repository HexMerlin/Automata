﻿namespace Automata.Core.Alang;

public class Difference(AlangExpr left, AlangExpr right) : InfixBinary(left, right)
{
    /// <summary>
    /// Parses the rule <c>Difference</c> in the Alang grammar specification.
    /// </summary>
    /// <param name="cursor">The cursor from which to parse the expression.</param>
    /// <returns>An <see cref="AlangExpr"/> representing the parsed expression.</returns>
    /// <exception cref="AlangFormatException">Thrown when the input is invalid.</exception>
    internal static AlangExpr Parse(ref AlangCursor cursor)
    {
        AlangExpr left = Intersection.Parse(ref cursor);
        if (cursor.TryConsumeDifferenceOperator())
        {
            AlangExpr right = Difference.Parse(ref cursor);  // Recursive call for repeated
            return new Difference(left, right);
        }
        return left;
    }

    ///<inheritdoc/>
    public override int Precedence => 2;

    ///<inheritdoc/>
    public override string AlangExpressionString => $"{Param(Left, this)}{Chars.Difference}{Param(Right, this)}";
}
