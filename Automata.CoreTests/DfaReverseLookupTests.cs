namespace Automata.Core.Tests;

[TestClass()]
public class DfaReverseLookupTests
{

    private static Dfa GetTestDfa ()
    {
        string[] sequences = ["_1 0 1", "1 1 1", "1 0 _2 1", "_1 1 2", "_1 2 _2 1", "_1 1 3 _4 1", "_1 _2 0 1", "1 _2 4 _4 1", "1 0 0 1", "1 _3 1 _3 1", "1 _1 1 1", "1 1 1 _3 1", "_1 3 1 1", "_1 0 2 _3 1", "_1 1 _1 _2 1", "_1 _2 4 _3 1", "1 _3 5 _3 1", "_1 0 1 2", "1 0 2 _2 1", "_1 _1 0 _1 1", "1 0 1 _1 1", "1 _1 _2 0 1", "_1 2 _1 0 1", "1 _1 0 0 1", "_1 3 _2 1 1", "_1 1 0 _3 2", "_1 _2 1 _2 2", "1 _3 4 _2 2"];
        Dfa dfa = (Dfa)new Nfa(sequences.Select(s => s.Split(' '))).AsDeterministic();
        dfa = dfa.Minimal_Brzozowski();
        return dfa;
    }

    [TestMethod()]
    public void LookupToState_ReturnsSymbols()
    {
        //Arrange
        var dfa = GetTestDfa();
        Dictionary<int, HashSet<int>> toState_Symbols = new();
       
        foreach (var t in dfa.Transitions())
        {
            if (!toState_Symbols.TryGetValue(t.ToState, out HashSet<int>? symbols))
            {
                symbols = [];
                toState_Symbols[t.ToState] = symbols;
            }
            symbols.Add(t.Symbol);
        }

        //Act and Assert
        DfaReverseLookup dfaReverseLookup = new DfaReverseLookup(dfa);
        foreach (int toState in toState_Symbols.Keys)
        {
            HashSet<int> expectedSymbols = toState_Symbols[toState];
            int[] actualSymbols = dfaReverseLookup.Symbols(toState);

            Assert.IsTrue(expectedSymbols.SetEquals(actualSymbols));
        }

    }

    [TestMethod()]
    public void LookupToStateAndSymbol_ReturnsCorrectFromStates()
    {
        //Arrange
        var dfa = GetTestDfa();
        Dictionary<(int, int), HashSet<int>> toStateSymbol_FromStates = new();

        foreach (var t in dfa.Transitions())
        {
            var key = (t.ToState, t.Symbol);

            if (! toStateSymbol_FromStates.TryGetValue(key, out HashSet<int>? fromStates))
            {
                fromStates = [];
                toStateSymbol_FromStates[key] = fromStates;
            }                        
            fromStates.Add(t.FromState);
        }

        //Act and Assert
        DfaReverseLookup dfaReverseLookup = new DfaReverseLookup(dfa);
        foreach ((int toState, int symbol) in toStateSymbol_FromStates.Keys)
        {
            HashSet<int> expectedFromStates = toStateSymbol_FromStates[(toState, symbol)];
            IntSet actualFromStates = dfaReverseLookup.FromStates(toState, symbol);
      
            Assert.IsTrue(actualFromStates.SetEquals(expectedFromStates), $"Expected: {string.Join(", ", expectedFromStates)}, Actual: {string.Join(", ", actualFromStates)}");
        }

    }

}