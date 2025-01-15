namespace Automata.Core.Alang;

/// <summary>
/// Represents an exception thrown when an error occurs during parsing of Alang expressions.
/// </summary>
/// <remarks>
/// Provides detailed information about the parsing error, including the index in the input where the error occurred and the specific type of error.
/// All throwing of this exception is done in the <see cref="AlangCursor"/> class.
/// </remarks>
/// <seealso cref="AlangCursor"/>
[Serializable]
public class AlangFormatException : Exception
{
    /// <summary>
    /// Index in the input string where the parsing error occurred.
    /// </summary>
    public readonly int Index;

    /// <summary>
    /// Type of parsing error that occurred.
    /// </summary>
    public readonly ParseErrorReason ErrorReason;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlangFormatException"/> class with the specified error index, error type, and message.
    /// </summary>
    /// <param name="errorReason">Reason for the parsing error.</param>
    /// <param name="index">Index in the input string where the parsing error occurred.</param>
    /// <param name="message">Error message that explains the reason for the exception.</param>
    private AlangFormatException(ParseErrorReason errorReason, int index, string message) : base(message)
    {
        this.Index = index;
        this.ErrorReason = errorReason;
    }

    /// <summary>
    /// Conditionally throws an <see cref="AlangFormatException"/> <c>iff</c> <paramref name="condition"/> is <see langword="false"/>.
    /// </summary>
    /// <param name="condition"><c>iff</c> <see langword="false"/> an exception is thrown.</param>
    /// <param name="errorReason">Reason for the parsing error.</param>
    /// <param name="index">Index in the input string where the parsing error occurred.</param>
    /// <param name="message">Error message with details about the exception.</param>
    /// <returns><see langword="true"/> or throws exception.</returns>
    public static bool Assert(bool condition, ParseErrorReason errorReason, int index, string message)
        => condition ? true : throw new AlangFormatException(errorReason, index, message);
}


