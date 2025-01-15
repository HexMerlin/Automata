namespace Automata.Core.Alang.Tests;

[TestClass()]
public class AlangCompilerTests
{

    #region Succesful Compilation Tests
    private static void Assert_Compile_ReturnsCorrect(string input, string expected)
    {
        var regex = AlangRegex.Parse(input);
        var actual = AlangCompiler.Compile(regex, new Alphabet()).ToCanonicalString();
        Assert.AreEqual(expected, actual);
    }

  
    [TestMethod()]
    public void Compile_ForEmptyParenthesesWithWhitespaces_ReturnsEmptyLang() => Assert_Compile_ReturnsCorrect(" () ", "S#=0, F#=0, T#=0");

    [TestMethod()]
    public void Compile_ForSingleCharSymbol_ReturnsSymbol() => Assert_Compile_ReturnsCorrect("a", "S#=2, F#=1: [1], T#=1: [0->1 a]");

    [TestMethod()]
    public void Compile_ForMultiCharSymbolWithWhitespaces_ReturnsSymbol() => Assert_Compile_ReturnsCorrect("  aa1  ", "S#=2, F#=1: [1], T#=1: [0->1 aa1]");

    [TestMethod()]
    public void Compile_ForConcatSymbolWithEmptyLang_ReturnsEmptyLang() => Assert_Compile_ReturnsCorrect("a1()", "S#=0, F#=0, T#=0");

    [TestMethod()]
    public void Compile_ForMultiCharSymbolWithOption_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("a1a?", "S#=2, F#=2: [0, 1], T#=1: [0->1 a1a]");

    [TestMethod()]
    public void Compile_ForRepeatedConcatWithGroup_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("aa(bb)cc", "S#=4, F#=1: [3], T#=3: [0->1 aa, 1->2 bb, 2->3 cc]");

    [TestMethod()]
    public void Compile_ForNestedConcat_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("a1(b1(c1)d1)", "S#=5, F#=1: [4], T#=4: [0->1 a1, 1->2 b1, 2->3 c1, 3->4 d1]");

    [TestMethod()]
    public void Compile_ForConcatWithOptional_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("a1?b1", "S#=3, F#=1: [2], T#=3: [0->1 a1, 0->2 b1, 1->2 b1]");


    [TestMethod()]
    public void Compile_ForNestedOperations_ReturnsCorrectPrecedence() => Assert_Compile_ReturnsCorrect("(((aa | (b & c)) (aa) ) (aa | bb))", "S#=4, F#=1: [3], T#=4: [0->1 aa, 1->2 aa, 2->3 aa, 2->3 bb]");

    [TestMethod()]
    public void Compile_ForUnion_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("a | b", "S#=2, F#=1: [1], T#=2: [0->1 a, 0->1 b]");

    [TestMethod()]
    public void Compile_ForMultipleUnion_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("a | b | cc", "S#=2, F#=1: [1], T#=3: [0->1 a, 0->1 b, 0->1 cc]");

    [TestMethod()]
    public void Compile_ForMultipleDifferenceSymbols_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("  x-y -z ", "S#=2, F#=1: [1], T#=1: [0->1 x]");

    [TestMethod()]
    public void Compile_ForMultipleIntersectionSymbols_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("a & b & c", "S#=0, F#=0, T#=0");

    [TestMethod()]
    public void Compile_ForMixedUnionAndIntersection_ReturnsCorrect() => Assert_Compile_ReturnsCorrect(" a |(b & c ) ", "S#=2, F#=1: [1], T#=1: [0->1 a]");

    [TestMethod()]
    public void Compile_ForDoublePostfixOps_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("  a?*  ", "S#=1, F#=1: [0], T#=1: [0->0 a]");

    [TestMethod()]
    public void Compile_ForNestedPrecedence_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("(((a-b))|c&d)", "a-b|c&d");

    [TestMethod()]
    public void Compile_ForNestedRedundantParentheses_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("(((a1)))", "a1");

    [TestMethod()]
    public void Compile_ForIntersectionOfSame_ReturnsSame() => Assert_Compile_ReturnsCorrect("a & a", "S#=2, F#=1: [1], T#=1: [0->1 a]");

    [TestMethod()]
    public void Compile_ForDifferenceWithDisjunct_ReturnsLeft() => Assert_Compile_ReturnsCorrect("a - b", "S#=2, F#=1: [1], T#=1: [0->1 a]");

    [TestMethod()]
    public void Compile_ForKleenePlusWithComplement_ReturnsCorrect() => Assert_Compile_ReturnsCorrect("a+~ b", "S#=4, F#=1: [2], T#=8: [0->1 a, 0->2 b, 1->1 a, 1->3 b, 2->3 a, 2->2 b, 3->3 a, 3->2 b]");

    [TestMethod()]
    public void Compile_OptionOnEmptyLanguage_ReturnsEpsilonAccepting() => Assert_Compile_ReturnsCorrect("()?", "S#=4, F#=1: [2], T#=8: [0->1 a, 0->2 b, 1->1 a, 1->3 b, 2->3 a, 2->2 b, 3->3 a, 3->2 b]");
    #endregion

    #region Unsuccessful Compilation Tests

    private static void Assert_Compile_ThrowsCorrectException(string input, ParseErrorReason parseErrorReason)
    {
        AlangFormatException exception = Assert.ThrowsException<AlangFormatException>(() => AlangRegex.Parse(input));
        Assert.AreEqual(parseErrorReason, exception.ErrorReason);
    }

    [TestMethod()]
    public void Compile_ForEmptyInput_ThrowsCorrectException()
        => Assert_Compile_ThrowsCorrectException("  ", ParseErrorReason.EmptyInput);

    [TestMethod()]
    public void Compile_ForInvalidInput_ThrowsCorrectException()
       => Assert_Compile_ThrowsCorrectException("(&)", ParseErrorReason.UnexpectedOperator);

    

    #endregion
}
