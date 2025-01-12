namespace Automata.Core.Alang;

/// <summary>
/// Reason for parsing errors in Alang expressions.
/// Specifies the reason for a <see cref="AlangFormatException"/>.
/// </summary>
/// <seealso cref="AlangFormatException"/>
public enum ParseErrorReason
{  
    /// <summary>
    /// Indicates that an unexpected closing parenthesis was encountered.
    /// </summary>
    UnexpectedClosingParenthesis,

    /// <summary>
    /// Indicates that an unexpected operator was encountered.
    /// </summary>
    UnexpectedOperator,

    /// <summary>
    /// Indicates that a closing parenthesis was expected but not found.
    /// </summary>
    MissingClosingParenthesis,

    /// <summary>
    /// Indicates that a right operand was expected after a binary operator but was not found.
    /// </summary>
    MissingRightOperand,

    /// <summary>
    /// Indicates that the input was empty or white-space only.
    /// </summary>
    EmptyInput,
}
