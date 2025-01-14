using Automata.Core.Alang;

namespace Automata.CoreTests.Alang;


//Test cases:
// (a-a) b => ∅  (should not be b!!)

[TestClass()]
public class AlangExprTests
{
    #region Valid Expressions Tests

    private static void AssertInputParsesToExpectedExpression(string input, string expectedAlangExpression)
    {
        string actual = AlangRegex.Parse(input).AlangExpressionString;
        Assert.AreEqual(expectedAlangExpression, actual);
    }

    [TestMethod()]
    public void Parse_ForEmptyParenthesesWithWhitespaces_ReturnsEmptyLang() => AssertInputParsesToExpectedExpression(" () ", "()");

    [TestMethod()]
    public void Parse_ForSingleCharSymbol_ReturnsSymbol() => AssertInputParsesToExpectedExpression("a", "a");

    [TestMethod()]
    public void Parse_ForMultiCharSymbolWithWhitespaces_ReturnsSymbol() => AssertInputParsesToExpectedExpression("  aa1  ", "aa1");
    
    [TestMethod()]
    public void Parse_ForConcatSymbolWithEmptyLang_ReturnsEmptyLang() => AssertInputParsesToExpectedExpression("a1()", "a1()");

    [TestMethod()]
    public void Parse_ForMultiCharSymbolWithOption_ReturnsCorrect() => AssertInputParsesToExpectedExpression("a1a?", "a1a?");

    [TestMethod()]
    public void Parse_ForRepeatedConcatWithGroup_ReturnsCorrect() => AssertInputParsesToExpectedExpression("aa(bb)cc", "aa bb cc");

    [TestMethod()]
    public void Parse_ForNestedConcat_ReturnsCorrect() => AssertInputParsesToExpectedExpression("a1(b1(c1)d1)", "a1 b1 c1 d1");

    [TestMethod()]
    public void Parse_ForConcatWithOptional_ReturnsCorrect() => AssertInputParsesToExpectedExpression("a1?b1", "a1?b1");


    [TestMethod()]
    public void Parse_ForNestedOperations_ReturnsCorrectPrecedence() => AssertInputParsesToExpectedExpression("(((aa | (b & c)) (aa) ) (aa | bb))", "(aa|b&c)aa(aa|bb)");


    [TestMethod()]
    public void Parse_ForMultipleUnion_ReturnsCorrect() => AssertInputParsesToExpectedExpression("a | b | cc", "a|b|cc");

    [TestMethod()]
    public void Parse_ForMultipleDifferenceSymbols_ReturnsCorrect() => AssertInputParsesToExpectedExpression("  x-y -z ", "x-y-z");

    [TestMethod()]
    public void Parse_ForMultipleIntersectionSymbols_ReturnsCorrect() => AssertInputParsesToExpectedExpression("a & b & c", "a&b&c");

    [TestMethod()]
    public void Parse_ForMixedUnionAndIntersection_ReturnsCorrect() => AssertInputParsesToExpectedExpression(" a |(b & c ) ", "a|b&c");

    [TestMethod()]
    public void Parse_ForMultiplePostfixOps_ReturnsCorrect() => AssertInputParsesToExpectedExpression("  a?*  ", "a?*");

    [TestMethod()]
    public void Parse_ForNestedPrecedence_ReturnsCorrect() => AssertInputParsesToExpectedExpression("(((a-b))|c&d)", "a-b|c&d");

    [TestMethod()]
    public void Parse_ForNestedRedundantParentheses_ReturnsCorrect() => AssertInputParsesToExpectedExpression("(((a1)))", "a1");

    [TestMethod()]
    public void Parse_ForMultiplePostfixWithWhitespace_ReturnsCorrect() => AssertInputParsesToExpectedExpression("a++~b", "a++~b");

    #endregion Valid Expressions Tests

    #region Invalid Expressions Tests
    private static void AssertThrowsAlangFormatException(string input, int errorIndex, ParseErrorReason expectedErrorType)
    {
        AlangFormatException exception = Assert.ThrowsException<AlangFormatException>(() => AlangRegex.Parse(input));
        Assert.AreEqual(expectedErrorType, exception.ErrorReason);
        Assert.AreEqual(errorIndex, exception.Index);
    }

    [TestMethod()]
    public void Parse_ForEmptyString_ThrowsCorrectException() => AssertThrowsAlangFormatException("", 0, ParseErrorReason.EmptyInput);

    [TestMethod()]
    public void Parse_ForOnlyWhitespaces_ThrowsCorrectException() => AssertThrowsAlangFormatException("  \t   ", 6, ParseErrorReason.EmptyInput);

    [TestMethod()]
    public void Parse_ForSingleLeftParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException("(", 1, ParseErrorReason.MissingClosingParenthesis);

    [TestMethod()]
    public void Parse_ForMissingClosingParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException("a(", 2, ParseErrorReason.MissingClosingParenthesis);

    [TestMethod()]
    public void Parse_ForSingleClosingParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException(")", 0, ParseErrorReason.UnexpectedClosingParenthesis);

    [TestMethod()]
    public void Parse_ForUnexpectedClosingParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException("a)", 1, ParseErrorReason.UnexpectedClosingParenthesis);

    [TestMethod()]
    public void Parse_ForMissingRightOperandUnion_ThrowsCorrectException() => AssertThrowsAlangFormatException("a|", 2, ParseErrorReason.MissingRightOperand);

    [TestMethod()]
    public void Parse_ForMissingRightOperandDifference_ThrowsCorrectException() => AssertThrowsAlangFormatException("a-", 2, ParseErrorReason.MissingRightOperand);

    [TestMethod()]
    public void Parse_ForMissingRightOperandIntersection_ThrowsCorrectException() => AssertThrowsAlangFormatException("a&", 2, ParseErrorReason.MissingRightOperand);

    [TestMethod()]
    public void Parse_ForSingleComplementThrowsCorrectException() => AssertThrowsAlangFormatException("~", 0, ParseErrorReason.UnexpectedOperator);

    [TestMethod()]
    public void Parse_ForSingleIntersection_ThrowsCorrectException() => AssertThrowsAlangFormatException("&", 0, ParseErrorReason.UnexpectedOperator);

    [TestMethod()]
    public void Parse_ForEnclosedUnion_ThrowsCorrectException() => AssertThrowsAlangFormatException("(|)", 1, ParseErrorReason.UnexpectedOperator);

    [TestMethod()]
    public void Parse_ForConsecutiveUnionOperators_ThrowsCorrectException() => AssertThrowsAlangFormatException("a||b",2, ParseErrorReason.MissingRightOperand);

    #endregion Invalid Expressions Tests
}