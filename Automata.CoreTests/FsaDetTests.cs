using Automata.Core.Alang;

namespace Automata.Core.Tests;

[TestClass()]
public class FsaDetTests
{
    [TestMethod()]
    public void StatePathTest()
    {
        var mfa = AlangRegex.Compile("_1 0 1 | 1 1 1 | 1 0 _2 1 | _1 1 2 | _1 2 _2 1 | _1 1 3 _4 1 | _1 _2 0 1 | 1 _2 4 _4 1 | 1 0 0 1 | 1 _3 1 _3 1 | 1 _1 1 1 | 1 1 1 _3 1 | _1 3 1 1 | _1 0 2 _3 1 | _1 1 _1 _2 1 | _1 _2 4 _3 1 | 1 _3 5 _3 1 | _1 0 1 2 | 1 0 2 _2 1 | _1 _1 0 _1 1 | 1 0 1 _1 1 | 1 _1 _2 0 1 | _1 2 _1 0 1 | 1 _1 0 0 1 | _1 3 _2 1 1 | _1 1 0 _3 2 | _1 _2 1 _2 2 | 1 _3 4 _2 2");
        var testSeqStrings = new string[] { "1", "_2", "4", "_4", "1" };

        //Test alphabet mappings are as expected
        int[] testSeqExpected = [2, 3, 7, 6, 2];
        int[] testSeqActual = testSeqStrings.Select(s => mfa.Alphabet[s]).ToArray();
        CollectionAssert.AreEqual(testSeqExpected, testSeqActual);

        //Test state path is as expected
        var statePathExpected = new int[] { 0, 1, 6, 19, 14, 24 };
        var statePathActual = mfa.StatePath(testSeqActual).ToArray();
        CollectionAssert.AreEqual(statePathExpected, statePathActual);

    }
}