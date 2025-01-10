using Automata.Core.Alang;

namespace Automata.CoreTests.Alang;

[TestClass()]
public class AlangExprTests
{
    #region Valid Expressions Tests

    private static void AssertInputParsesToExpectedExpression(string input, string expectedAlangExpression)
    {
        string actual = AlangExpr.Parse(input).AlangExpressionString;
        Assert.AreEqual(expectedAlangExpression, actual);
    }

    [TestMethod()]
    public void Parse_ForEmptyParenthesesWithWhitespaces_ReturnsEmpty() => AssertInputParsesToExpectedExpression(" () ", "()");

    [TestMethod()]
    public void Parse_ForSingleCharSymbol_ReturnsSymbol() => AssertInputParsesToExpectedExpression("a", "a");

    [TestMethod()]
    public void Parse_ForMultiCharSymbolWithWhitespaces_ReturnsSymbol() => AssertInputParsesToExpectedExpression("  aa1  ", "aa1");

    [TestMethod()]
    public void Parse_ForConcatAtomWithEmptySet_ReturnsAtom() => AssertInputParsesToExpectedExpression("a1()", "a1");

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
    private static void AssertThrowsAlangFormatException(string input, int errorIndex, ParseErrorType expectedErrorType)
    {
        AlangFormatException exception = Assert.ThrowsException<AlangFormatException>(() => AlangExpr.Parse(input));
        Assert.AreEqual(expectedErrorType, exception.ErrorType);
        Assert.AreEqual(errorIndex, exception.Index);
    }

    [TestMethod()]
    public void Parse_ForEmptyString_ReturnsEmpty() => AssertThrowsAlangFormatException("", 0, ParseErrorType.ExpectedBeginExpressionOrEOI);

    [TestMethod()]
    public void Parse_ForOnlyWhitespaces_ThrowsCorrectException() => AssertThrowsAlangFormatException("  \t   ", 6, ParseErrorType.ExpectedBeginExpressionOrEOI);

    [TestMethod()]
    public void Parse_ForSingleLeftParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException("(", 1, ParseErrorType.MissingClosingParenthesis);

    [TestMethod()]
    public void Parse_ForMissingClosingParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException("a(", 2, ParseErrorType.MissingClosingParenthesis);

    [TestMethod()]
    public void Parse_ForSingleClosingParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException(")", 0, ParseErrorType.UnexpectedClosingParenthesis);

    [TestMethod()]
    public void Parse_ForUnexpectedClosingParenthesis_ThrowsCorrectException() => AssertThrowsAlangFormatException("a)", 1, ParseErrorType.UnexpectedClosingParenthesis);

    [TestMethod()]
    public void Parse_ForMissingRightOperand_ThrowsCorrectException() => AssertThrowsAlangFormatException("a|", 2, ParseErrorType.MissingRightOperand);

    [TestMethod()]
    public void Parse_ForEnclosedUnion_ThrowsCorrectException() => AssertThrowsAlangFormatException("(|)", 1, ParseErrorType.ExpectedBeginExpressionOrEOI);

    [TestMethod()]
    public void Parse_ForConsecutiveUnionOperators_ThrowsCorrectException() => AssertThrowsAlangFormatException("a||b", 2, ParseErrorType.MissingRightOperand);

    #endregion Invalid Expressions Tests
}