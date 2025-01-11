namespace Automata.Core.Alang;

/// <summary>
/// Represents an exception that is thrown when an error occurs during parsing of Alang expressions.
/// </summary>
/// <remarks>
/// Provides detailed information about the parsing error, including the index in the input where the error occurred and the specific type of error.
/// </remarks>
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
    public readonly ParseErrorType ErrorType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlangFormatException"/> class with the specified error index, error type, and message.
    /// </summary>
    /// <param name="index">Index in the input string where the parsing error occurred.</param>
    /// <param name="ErrorType">Type of parsing error that occurred.</param>
    /// <param name="message">Error message that explains the reason for the exception.</param>
    public AlangFormatException(int index, ParseErrorType ErrorType, string message) : base(message)
    {
        this.Index = index;
        this.ErrorType = ErrorType;
    }
}


