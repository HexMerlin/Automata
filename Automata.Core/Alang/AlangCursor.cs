﻿
namespace Automata.Core.Alang;

/// <summary>
/// A cursor for parsing Alang expressions from an input string.
/// </summary>
/// <remarks>
/// A cursor is a lightweight struct that consumes characters from the left of a string input.
/// The input string is never modified. Instead the cursor maintains a lightweight span of the remaining input.
/// The parser available in namespace <see cref="Automata.Core.Alang"/> only need to create a single cursor instance that is passed along during the parse process.
/// The contract of the <see cref="AlangCursor"/> is that it always point to a non-white-space character or <see cref="Chars.EOI"/> if the input is empty.
/// <para>Consequently:</para>
/// <para>- All methods in <see cref="AlangCursor"/> that moves the cursor, must ensure on exit that leading whitespace is trimmed.</para>
/// <para>- All methods in <see cref="AlangCursor"/> can assume on entry that the input is trimmed of leading whitespace.</para>
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="AlangCursor"/> struct with the specified input string.
/// </remarks>
/// <param name="input">The input string to parse.</param>
public ref struct AlangCursor(string input)
{
    private readonly int OriginalInputLength = input.Length;

    private ReadOnlySpan<char> cursor = input.AsSpan().TrimStart();

    /// <summary>
    /// Gets a value indicating whether the cursor has reached the end of the input.
    /// </summary>
    public readonly bool IsEmpty => cursor.IsEmpty;

    /// <summary>
    /// Gets the first character in the remaining input, or <see cref="Chars.EOI"/> if the input is empty.
    /// </summary>
    private readonly char First
        => this.cursor.IsEmpty ? Chars.EOI : this.cursor[0];

    /// <summary>
    /// Gets a value indicating whether the current character indicates the start of an expression.
    /// If the input is empty, this method will return <see langword="false"/>.
    /// </summary>
    public readonly bool IsExpressionStart => Chars.IsExpressionStart(First);

    /// <summary>
    /// Indicates whether the first character in the remaining input is the specified character.
    /// </summary>
    public readonly bool Is(char c) => First == c;

    public void ShouldNotBeEmpty()
    {
        if (IsEmpty) throw new AlangFormatException(CursorIndex, ParseErrorType.EmptyInput, $"Input cannot be empty. To represent an empty set, use ()");
    }

    public void ShouldNotBeOperator()
    {
        if (Chars.IsOperator(First)) throw new AlangFormatException(CursorIndex, ParseErrorType.UnexpectedOperator, $"Unexpected operator {First} detected at index {CursorIndex}");
    }

    public void ShouldNotBeRightParen()
    {
        if (Is(Chars.RightParen)) throw new AlangFormatException(CursorIndex, ParseErrorType.UnexpectedClosingParenthesis, $"Unexpected {Chars.RightParen} detected at index {CursorIndex}");
    }

    public void ShouldBeRightParen()
    {
        if (!Is(Chars.RightParen)) throw new AlangFormatException(CursorIndex, ParseErrorType.MissingClosingParenthesis, $"Expected {Chars.RightParen} at index {CursorIndex}, but read {NextAsString}");
    }
    private bool TryConsumeBinaryOperator(char binaryOperator)
    {
        if (!TryConsume(binaryOperator)) return false;
        if (IsExpressionStart) return true;
        throw new AlangFormatException(CursorIndex, ParseErrorType.MissingRightOperand, $"Expected right operand after '{binaryOperator}' at index {CursorIndex}, but read {NextAsString}");
    }

    public bool TryConsumeUnionOperator() => TryConsumeBinaryOperator(Chars.Union);
    public bool TryConsumeDifferenceOperator() => TryConsumeBinaryOperator(Chars.Difference);

    public bool TryConsumeIntersectionOperator() => TryConsumeBinaryOperator(Chars.Intersection);

    public bool TryConsumeLeftParen() => TryConsume(Chars.LeftParen);

    public void ConsumeRightParen()
    {
        ShouldBeRightParen();
        _ = TryConsume(Chars.RightParen);
    }


    /// <summary>
    /// Tries to consume the specified character from the input and advances the cursor if successful.
    /// </summary>
    /// <param name="c">The character to attempt to consume.</param>
    /// <returns><c>true</c> if the character was successfully consumed; otherwise, <c>false</c>.</returns>
    public bool TryConsume(char c)
    {
        if (First != c)
            return false;

        this.cursor = this.cursor.IsEmpty ? this.cursor : this.cursor.Slice(1).TrimStart(); // Consume the character and trim whitespace
        return true;
    }

    /// <summary>
    /// Tries to consume one of the specified characters from the input and advances the cursor if successful.
    /// </summary>
    /// <param name="chars">An array of characters to attempt to consume.</param>
    /// <remarks>This method supports inclusion of <see cref="Chars.EOI"/> to also match against End-Of-Input.</remarks>
    /// <returns>The character that was consumed if successful; otherwise, <see cref="Chars.Invalid"/>.</returns>
    public char TryConsumeAny(params char[] chars)
    {
        char first = First;
        if (chars.Contains(first))
        {
            _ = TryConsume(first); // Will always consume and return true
            return first;
        }
        return Chars.Invalid;
    }

    /// <summary>
    /// Consume an <see cref="Atom"/> from the input.
    /// </summary>
    /// <remarks>The method will return an empty (invalid) Atom if no characters could be consumed. It is up to the calling code to handle this.
    /// </remarks>
    /// <returns>
    /// The consumed <see cref="Atom"/>.
    /// </returns>
    public Atom ConsumeAtom()
    {
        int pos = 0;

        while (pos < this.cursor.Length && Chars.IsAtomChar(this.cursor[pos]))
            pos++;

        Atom atom = new Atom(this.cursor.Slice(0, pos).ToString());

        this.cursor = this.cursor.Slice(pos).TrimStart();
        return atom;
    }

    /// <summary>
    /// Gets the next character in the remaining input as a human-readable string.
    /// </summary>
    public readonly string NextAsString => cursor.IsEmpty ? "End-Of-Input" : cursor[0].ToString();

    /// <summary>
    /// The index of the cursor in relation to the original input string.
    /// </summary>  
    public readonly int CursorIndex => OriginalInputLength - cursor.Length;

    /// <summary>
    /// Returns a string representation of the remaining input.
    /// </summary>
    /// <returns>A string that represents the remaining input.</returns>
    public override readonly string ToString() => cursor.IsEmpty ? "<EMPTY>" : $"'{string.Join("", cursor.ToArray())}'";
}
