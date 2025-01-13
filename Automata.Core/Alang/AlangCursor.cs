
namespace Automata.Core.Alang;

/// <summary>
/// Represents a cursor for parsing Alang expressions from an input string.
/// </summary>
/// <remarks>
/// A cursor is a lightweight struct that consumes characters from the left of a string input.
/// The input string is never modified. Instead, the cursor maintains a lightweight span of the remaining input.
/// The parser in the <see cref="Automata.Core.Alang"/> namespace only needs to create a single cursor instance that is passed along during the parse process.
/// The contract of the <see cref="AlangCursor"/> is that it always points to a non-white-space character or <see cref="Chars.EOI"/> if the input is empty.
/// <para>Consequently:</para>
/// <para>- All methods in <see cref="AlangCursor"/> that move the cursor must ensure on exit that leading whitespace is trimmed.</para>
/// <para>- All methods in <see cref="AlangCursor"/> can assume on entry that the input is trimmed of leading whitespace.</para>
/// </remarks>
/// <param name="input">The input string to parse.</param>
public ref struct AlangCursor(string input)
{
    #region Data
    private readonly int OriginalInputLength = input.Length;

    private ReadOnlySpan<char> cursor = input.AsSpan().TrimStart();

    #endregion Data

    /// <summary>
    /// Indicates whether the cursor has reached the end of the input.
    /// </summary>
    public readonly bool IsEmpty => cursor.IsEmpty;

    /// <summary>
    /// First character in the remaining input, or <see cref="Chars.EOI"/> if the input is empty.
    /// </summary>
    private readonly char First => cursor.IsEmpty ? Chars.EOI : cursor[0];

    /// <summary>
    /// Indicates whether the current character indicates the start of an expression.
    /// <see langword="false"/> if the input is empty.
    /// </summary>
    public readonly bool IsExpressionStart => Chars.IsExpressionStart(First);

    /// <summary>
    /// Indicates whether the first character in the remaining input is the specified character.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the first character is the specified character.</returns>
    public readonly bool Is(char c) => First == c;

    /// <summary>
    /// Indicates whether the first character in the remaining input is not the specified character.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns><see langword="true"/> <c>iff</c> the first character is NOT the specified character.</returns>
    public readonly bool IsNot(char c) => First != c;

    /// <summary>
    /// Validates that the cursor is not at the end of input.
    /// </summary>
    /// <exception cref="AlangFormatException">Thrown when the cursor is at the end of input.</exception>
    /// <remarks>
    /// This method enforces the rule that empty input is not valid in Alang expressions.
    /// To represent an empty set, use parentheses '()' instead.
    /// </remarks>
    public readonly void ShouldNotBeEmpty() 
        => AlangFormatException.Assert(!IsEmpty, ParseErrorReason.EmptyInput, CursorIndex, "Input cannot be empty. To represent an empty set, use ()");

    /// <summary>
    /// Validates that the current character is not an operator.
    /// </summary>
    /// <exception cref="AlangFormatException">Thrown when the current character is an operator.</exception>
    /// <remarks>
    /// This method ensures expressions do not start with an operator.
    /// </remarks>
    public readonly void ShouldNotBeOperator() 
        => AlangFormatException.Assert(!Chars.IsOperator(First), ParseErrorReason.UnexpectedOperator, CursorIndex, $"Unexpected operator '{First}' detected at index {CursorIndex}");

    /// <summary>
    /// Validates that the current character is not a right parenthesis.
    /// </summary>
    /// <exception cref="AlangFormatException">Thrown when the current character is a right parenthesis ')'.</exception>
    public readonly void ShouldNotBeRightParen() 
        => AlangFormatException.Assert(IsNot(Chars.RightParen), ParseErrorReason.UnexpectedClosingParenthesis, CursorIndex, $"Unexpected '{Chars.RightParen}' detected at index {CursorIndex}");

    /// <summary>
    /// Validates that the current character is a right parenthesis.
    /// </summary>
    /// <exception cref="AlangFormatException">Thrown when the current character is not a right parenthesis ')'.</exception>
    public readonly void ShouldBeRightParen() 
        => AlangFormatException.Assert(Is(Chars.RightParen), ParseErrorReason.MissingClosingParenthesis, CursorIndex, $"Expected '{Chars.RightParen}' at index {CursorIndex}, but read {NextAsString}");

    /// <summary>
    /// Validates that there is right operand after a binary operator.
    /// </summary>
    /// <param name="binaryOperator">The preceding binary operator character. (Only used for better informed error messages if validation fails). </param>
    /// <exception cref="AlangFormatException">Thrown when the current character is not a right parenthesis ')'.</exception>
    public readonly void ShouldBeRightOperand(char binaryOperator)
      => AlangFormatException.Assert(IsExpressionStart, ParseErrorReason.MissingRightOperand, CursorIndex, $"Expected right operand after '{binaryOperator}' at index {CursorIndex}, but read {NextAsString}");
      

    /// <summary>
    /// Tries to consume the specified character from the input and advances the cursor if successful.
    /// </summary>
    /// <param name="c">The character to attempt to consume.</param>
    /// <returns><see langword="true"/> <c>iff</c> the character was successfully consumed.</returns>
    public bool TryConsume(char c)
    {
        if (First != c)
            return false;

        cursor = cursor.IsEmpty ? cursor : cursor.Slice(1).TrimStart(); // Consume the character and trim whitespace
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
    /// Tries to consume a binary operator from the input and advances the cursor if successful.
    /// </summary>
    /// <param name="binaryOperator">The binary operator character to attempt to consume.</param>
    /// <returns><see langword="true"/> <c>iff</c> the binary operator was successfully consumed and the next character is a valid expression start.</returns>
    /// <exception cref="AlangFormatException">Thrown when the binary operator was consumed but the next character was not <see cref="IsExpressionStart"/> .</exception>
    public bool TryConsumeBinaryOperator(char binaryOperator)
    {
        if (!TryConsume(binaryOperator))
            return false;
      
        ShouldBeRightOperand(binaryOperator);
        return true;
    }

    /// <summary>
    /// Consumes an <see cref="Symbol"/> from the input.
    /// </summary>
    /// <remarks>
    /// This method will return an empty (invalid) <see cref="Symbol"/> if no characters could be consumed. It is up to the calling code to handle this.
    /// </remarks>
    /// <returns>The consumed <see cref="Symbol"/>.</returns>
    public Symbol ConsumeSymbol()
    {
        int pos = 0;

        while (pos < cursor.Length && Chars.IsSymbolChar(cursor[pos]))
            pos++;

        Symbol symbol = new Symbol(cursor.Slice(0, pos).ToString());

        cursor = cursor.Slice(pos).TrimStart();
        return symbol;
    }

    /// <summary>
    /// String representation of the next character in the input, or "End-Of-Input" if at the end of the input.
    /// </summary>
    public readonly string NextAsString => cursor.IsEmpty ? "End-Of-Input" : cursor[0].ToString();

    /// <summary>
    /// Current position of the cursor in the original input string.
    /// </summary>  
    public readonly int CursorIndex => OriginalInputLength - cursor.Length;

    /// <summary>
    /// String representation of the remaining input.
    /// </summary>
    /// <returns>A string that represents the remaining input.</returns>
    public override readonly string ToString() => cursor.IsEmpty ? "<EMPTY>" : $"'{new string(cursor)}'";
}
