namespace Automata.Core.Alang;


/// <summary>
/// Specifies the type of parsing error encountered in Alang expressions.
/// </summary>
public enum ParseErrorType
{
    /// <summary>
    /// Indicates that an atom was expected but not found.
    /// </summary>
    MissingAtom,

    /// <summary>
    /// Indicates that an unexpected closing parenthesis was encountered.
    /// </summary>
    UnexpectedClosingParenthesis,

    /// <summary>
    /// Indicates that a closing parenthesis was expected but not found.
    /// </summary>
    MissingClosingParenthesis,

    /// <summary>
    /// Indicates that a right operand was expected after a binary operator but was not found.
    /// </summary>
    MissingRightOperand,

    /// <summary>
    /// Indicates that a new subexpression or end-of-input was expected but not found.
    /// </summary>
    ExpectedBeginExpressionOrEOI,
}

/// <summary>
/// Represents an exception that is thrown when an error occurs during parsing of Alang expressions.
/// </summary>
/// <remarks>
/// Provides detailed information about the parsing error, including the index in the input where the error occurred and the specific type of error.
/// </remarks>
public sealed class AlangFormatException : Exception
{
    /// <summary>
    /// Gets the index in the input string where the parsing error occurred.
    /// </summary>
    public readonly int Index;

    /// <summary>
    /// Gets the type of parsing error that occurred.
    /// </summary>
    public readonly ParseErrorType ErrorType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlangFormatException"/> class with the specified error index, error type, and message.
    /// </summary>
    /// <param name="index">The index in the input string where the parsing error occurred.</param>
    /// <param name="ErrorType">The type of parsing error that occurred.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    private AlangFormatException(int index, ParseErrorType ErrorType, string message) : base(message)
    {
        this.Index = index;
        this.ErrorType = ErrorType;
    }

    /// <summary>
    /// Throws an <see cref="AlangFormatException"/> indicating that an atom was expected at the current cursor position.
    /// </summary>
    /// <param name="cursor">The cursor pointing to the current position in the input string.</param>
    /// <exception cref="AlangFormatException">Always thrown to indicate the specific parsing error.</exception>
    public static void ThrowExpectedAtom(AlangCursor cursor)
        => throw new AlangFormatException(cursor.CursorIndex, ParseErrorType.MissingAtom, $"Expected atom at index {cursor.CursorIndex}, but read {cursor.NextAsString}");

    /// <summary>
    /// Throws an <see cref="AlangFormatException"/> indicating that a right operand was expected after a binary operator.
    /// </summary>
    /// <param name="cursor">The cursor pointing to the current position in the input string.</param>
    /// <param name="binaryOperator">The binary operator after which the right operand was expected.</param>
    /// <exception cref="AlangFormatException">Always thrown to indicate the specific parsing error.</exception>
    public static void ThrowMissingRightOperand(AlangCursor cursor, char binaryOperator)
        => throw new AlangFormatException(cursor.CursorIndex, ParseErrorType.MissingRightOperand, $"Expected right operand after '{binaryOperator}' at index {cursor.CursorIndex}, but read {cursor.NextAsString}");

    /// <summary>
    /// Throws an <see cref="AlangFormatException"/> indicating that an unexpected closing parenthesis was encountered.
    /// </summary>
    /// <param name="cursor">The cursor pointing to the current position in the input string.</param>
    /// <exception cref="AlangFormatException">Always thrown to indicate the specific parsing error.</exception>
    public static void ThrowSpuriousClosingParenthesis(AlangCursor cursor)
        => throw new AlangFormatException(cursor.CursorIndex, ParseErrorType.UnexpectedClosingParenthesis, $"Spurious {Chars.RightParen} detected at index {cursor.CursorIndex}");

    /// <summary>
    /// Throws an <see cref="AlangFormatException"/> indicating that a closing parenthesis was expected but not found.
    /// </summary>
    /// <param name="cursor">The cursor pointing to the current position in the input string.</param>
    /// <exception cref="AlangFormatException">Always thrown to indicate the specific parsing error.</exception>
    public static void ThrowMissingClosingParenthesis(AlangCursor cursor)
        => throw new AlangFormatException(cursor.CursorIndex, ParseErrorType.MissingClosingParenthesis, $"Expected {Chars.RightParen} at index {cursor.CursorIndex}, but read {cursor.NextAsString}");

    /// <summary>
    /// Throws an <see cref="AlangFormatException"/> indicating that a new subexpression or end-of-input was expected.
    /// </summary>
    /// <param name="cursor">The cursor pointing to the current position in the input string.</param>
    /// <remarks>
    /// This error should not occur in the current implementation because it is handled by other error types earlier in the parsing process.
    /// </remarks>
    /// <exception cref="AlangFormatException">Always thrown to indicate the specific parsing error.</exception>
    public static void ThrowExpectedBeginExpressionOrEOI(AlangCursor cursor)
        => throw new AlangFormatException(cursor.CursorIndex, ParseErrorType.ExpectedBeginExpressionOrEOI, $"Expected new subexpression or end-of-input at index {cursor.CursorIndex}, but read {cursor.NextAsString}");
}
