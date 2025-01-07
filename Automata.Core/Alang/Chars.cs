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
    /// Determines if a character can be the start of an expression.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character can start an expression; otherwise, false.</returns>
    public static bool IsExpressionStart(char c)
        => !ForbiddenExpressionStarts.Contains(c) && !char.IsWhiteSpace(c);

    /// <summary>
    /// Determines if a character can be part of an atom.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character can be part of an atom; otherwise, false.</returns>
    public static bool IsAtomChar(char c)
        => !ForbiddenInAtoms.Contains(c) && !char.IsWhiteSpace(c);

    /// <summary>
    /// Determines if a character is a postfix operator.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a postfix operator; otherwise, false.</returns>
    public static bool IsPostfixOp(char c) => PostfixOps.Contains(c);

    private static readonly FrozenSet<char> ForbiddenExpressionStarts
       = [EOI, Invalid, RightParen, Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement];

    private static readonly FrozenSet<char> ForbiddenInAtoms
        = [EOI, Invalid, LeftParen, RightParen, Union, Difference, Intersection, Option, KleeneStar, KleenePlus, Complement];

    private static readonly FrozenSet<char> PostfixOps
        = [Option, KleeneStar, KleenePlus, Complement];
}
