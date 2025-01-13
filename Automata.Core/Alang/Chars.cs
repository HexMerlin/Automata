using System.Collections.Frozen;

namespace Automata.Core.Alang;

/// <summary>
/// Character definitions and utility methods for Alang.
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
    public const char Invalid = '\uFFFF'; // Invalid character

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
    /// Wildcard token denoting any string in the alphabet.
    /// </summary>
    public const char Wildcard = '.';

    /// <summary>
    /// Indicates if a character can be the start of an expression.
    /// </summary>
    /// <param name="c">Character to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the character can start an expression.</returns>
    public static bool IsExpressionStart(char c)
        => !ForbiddenExpressionStarts.Contains(c) && !char.IsWhiteSpace(c);

    /// <summary>
    /// Indicates if a character can be part of Symbol.
    /// </summary>
    /// <param name="c">Character to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the character can be part of a Symbol.</returns>
    public static bool IsSymbolChar(char c)
        => !ForbiddenInSymbols.Contains(c) && !char.IsWhiteSpace(c);

    /// <summary>
    /// Indicates if a character is an operator.
    /// </summary>
    /// <param name="c">Character to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the character is an operator.</returns>
    public static bool IsOperator(char c) => Operators.Contains(c);

    private static readonly FrozenSet<char> ForbiddenExpressionStarts
       = [EOI, Invalid, RightParen, Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement];

    private static readonly FrozenSet<char> ForbiddenInSymbols
        = [EOI, Invalid, LeftParen, RightParen, Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement, Wildcard];

    private static readonly FrozenSet<char> Operators
        = [Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement];
}
