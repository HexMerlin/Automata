using System.Collections.Frozen;

namespace Automata.Core.Alang;

/// <summary>
/// Provides character definitions and utility methods Alang.
/// </summary>
public static class Chars
{
    /// <summary>
    /// End of input character.
    /// </summary>
    public const char EOI = char.MinValue;

    /// <summary>
    /// Invalid character.
    /// </summary>
    public const char Invalid = '\uFFFF'; //Invalid character

    /// <summary>
    /// Union operator character.
    /// </summary>
    public const char Union = '|';

    /// <summary>
    /// Difference operator character.
    /// </summary>
    public const char Difference = '-';

    /// <summary>
    /// Intersection operator character.
    /// </summary>
    public const char Intersection = '&';

    /// <summary>
    /// Option operator character.
    /// </summary>
    public const char Option = '?';

    /// <summary>
    /// Kleene star operator character.
    /// </summary>
    public const char KleeneStar = '*';

    /// <summary>
    /// Kleene plus operator character.
    /// </summary>
    public const char KleenePlus = '+';

    /// <summary>
    /// Complement operator character.
    /// </summary>
    public const char Complement = '~';

    /// <summary>
    /// Left parenthesis character.
    /// </summary>
    public const char LeftParen = '(';

    /// <summary>
    /// Right parenthesis character.
    /// </summary>
    public const char RightParen = ')';

    /// <summary>
    /// Wildcard
    /// </summary>
    /// <remarks>A wildcard token denoting any string in the alphabet.</remarks>
    public const char Wildcard = '.';

    /// <summary>
    /// Indicates if a character can be the start of an expression.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character can start an expression; otherwise, false.</returns>
    public static bool IsExpressionStart(char c)
        => !ForbiddenExpressionStarts.Contains(c) && !char.IsWhiteSpace(c);

    /// <summary>
    /// Indicates if a character can be part of an atom.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character can be part of an atom; otherwise, false.</returns>
    public static bool IsAtomChar(char c)
        => !ForbiddenInAtoms.Contains(c) && !char.IsWhiteSpace(c);

    /// <summary>
    /// Indicates if a character is an operator.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True <c>iff</c> the character is an operator.</returns>
    public static bool IsOperator(char c) => Operators.Contains(c);

    private static readonly FrozenSet<char> ForbiddenExpressionStarts
       = [EOI, Invalid, RightParen, Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement];

    private static readonly FrozenSet<char> ForbiddenInAtoms
        = [EOI, Invalid, LeftParen, RightParen, Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement, Wildcard];

    private static readonly FrozenSet<char> Operators
        = [Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement];
}
