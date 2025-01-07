using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automata.Core.Alang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core.Alang.Tests;

[TestClass()]
public class AlangExprTests
{

    [TestMethod()]
    public void Parse_ForEmptyString_ReturnsEmpty()
    {
        string expected = string.Empty;
        string actual = AlangExpr.Parse("").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForWhitespaces_ReturnsEmpty()
    {
        string expected = string.Empty;
        string actual = AlangExpr.Parse("  \t   ").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForEmptyGroupWithWhitespaces_ReturnsEmpty()
    {
        string expected = string.Empty;
        string actual = AlangExpr.Parse(" () ").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForSingleCharSymbol_ReturnsSymbol()
    {
        string expected = "a";
        string actual = AlangExpr.Parse("a").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForMultiCharSymbolWithWhitespaces_ReturnsSymbol()
    {
        string expected = "aa1";
        string actual = AlangExpr.Parse("  aa1  ").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForConcatWithEmptyGroup_ReturnsSymbol()
    {
        string expected = "a1";
        string actual = AlangExpr.Parse("a1()").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForMultiCharSymbolWithOption_ReturnsCorrect()
    {
        string expected = "a1?";
        string actual = AlangExpr.Parse("a1?").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForRepeatedConcatWithGroup_ReturnsCorrect()
    {
        string expected = "aa bb cc";
        string actual = AlangExpr.Parse("aa(bb)cc").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForRepeatedConcatOfGroups_ReturnsCorrect()
    {
        string expected = "aa bb cc";
        string actual = AlangExpr.Parse("(aa)(bb) (cc)").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForNestedConcats_ReturnsCorrect()
    {
        string expected = "a1 b1 c1 d1";
        string actual = AlangExpr.Parse("a1(b1(c1)d1)").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForConcatWithOptional_ReturnsCorrect()
    {
        string expected = "a1?b1";
        string actual = AlangExpr.Parse("a1?b1").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForNestedOperations_ReturnsCorrectPrecedence()
    {
        string expected = "(aa|b&c)aa(aa|bb)";
        string actual = AlangExpr.Parse("(((aa | (b & c)) (aa) ) (aa | bb))").ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForInvalidExpression1_ThrowsFormatException()
    {
        Assert.ThrowsException<FormatException>(() => AlangExpr.Parse(")"));
    }

    [TestMethod()]
    public void Parse_ForMultipleUnionSymbols_ReturnsCorrect()
    {
        string input = "a | b | cc";
        string expected = "a|b|cc";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForMultipleDifferenceSymbols_ReturnsCorrect()
    {
        string input = "  x-y -z ";
        string expected = "x-y-z";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForMultipleIntersectionSymbols_ReturnsCorrect()
    {
        string input = "a & b & c";
        string expected = "a&b&c";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForMixedUnionAndIntersection_ReturnsCorrect()
    {
        string input = " a |(b & c ) ";
        string expected = "a|b&c";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForMultiplePostfixOps_ReturnsCorrect()
    {
        string input = "  a?*  ";
        string expected = "a?*";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForNestedPrecedence_ReturnsCorrect()
    {

        string input = "(((a-b))|c&d)";
        string expected = "a-b|c&d";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForNestedRedundantParentheses_ReturnsCorrect()
    {
        // (( (a1) )) => a1
        string input = "(((a1)))";
        string expected = "a1";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForMultiplePostfixWithWhitespace_ReturnsCorrect()
    {
        string input = "a++~b";
        string expected = "a++~b";
        string actual = AlangExpr.Parse(input).ExpressionString;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void Parse_ForDanglingUnionOperator_ThrowsFormatException()
    {
        Assert.ThrowsException<FormatException>(() => AlangExpr.Parse("a|"));
    }

    [TestMethod()]
    public void Parse_ForUnclosedGroup_ThrowsFormatException()
    {
        Assert.ThrowsException<FormatException>(() => AlangExpr.Parse("("));
    }

    [TestMethod()]
    public void Parse_ForMultipleOperatorsInARow_ThrowsFormatException()
    {
        Assert.ThrowsException<FormatException>(() => AlangExpr.Parse("a||b"));
    }

    [TestMethod()]
    public void Parse_ForSpuriousClosingParen_ThrowsFormatException()
    {
        Assert.ThrowsException<FormatException>(() => AlangExpr.Parse("a)"));
    }
}