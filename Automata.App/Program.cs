
using System.Diagnostics;
using System.Text;
using Automata.Core;
using Automata.Core.Alphabets;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;

namespace Automata.App;

/// <summary>
/// A sample program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Creating graph."); // Write some text output to the console window

        Random random = new Random(7);
        var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => random.Next(4).ToString())); //Create some random string sequences

        IFsa fsa = new Nfa(sequences).ToDfa().Minimized(); //Create a minimized DFA from the sequences

        Graph graph = fsa.CreateGraph(displayStateIDs: true); // Create a displayable graph object (FSA wih layout)

        //Graph graph = sequences.CreateGraph(); //Alternatively you can use this command, to replace the 2 lines above

        GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal interactive window that displays the graph in it

        Console.WriteLine("Graph is displayed."); // Write some text output to the console window
    }

    /* Notice. Currently actively working on this file for testing. Just ignore dev-code below */
    /// <summary>Creates a DFA accepting the regular expression: <c>s*</c></summary>
    private static Dfa CreateKleeneStarDfa(string s)
    {
        Alphabet a = new Alphabet([s]);
        Dfa dfa = new Dfa(a);
        dfa.SetInitial(0);
        dfa.SetFinal(0);
        dfa.Add(new Transition(0, 0, 0));
        return dfa;
    }

    /// <summary>Creates a DFA accepting the regular expression: <c>s0*s1*</c></summary>
    private static Dfa CreateKleeneStarDfa(string s0, string s1)
    {
        Alphabet a = new Alphabet([s0, s1]);
        Dfa dfa = new Dfa(a);
        dfa.SetInitial(0);
        dfa.SetFinal(0);
        dfa.SetFinal(1);
        dfa.Add(new Transition(0, 0, 0));
        dfa.Add(new Transition(0, 1, 1));
        dfa.Add(new Transition(1, 1, 1));
        return dfa;
    }

    //public static int GetHashCode1(string[] strings)
    //{
    //    var hash = new HashCode();

    //    for (var i = 0; i < strings.Length; i++)
    //        hash.Add(strings[i]);

    //    return hash.ToHashCode();
    //}

    //public static uint GetHashCode2(string[] strings)
    //{
    //    var hash = new System.IO.Hashing.XxHash32();

    //    for (var i = 0; i < strings.Length; i++)
    //        hash.Append(Encoding.UTF8.GetBytes(strings[i]));
    //    //return hash.GetCurrentHashAsUInt64();
    //    return hash.GetCurrentHashAsUInt32();
    //}

    public static IEnumerable<string> Generate()
    {
        Random _random = new Random();
        while (true)
        {
            var length = _random.Next(2, 11);
            var randomString = new string(
                Enumerable.Range(0, length)
                          .Select(_ => (char)_random.Next('a', 'z' + 1))
                          .ToArray());
            yield return randomString;
        }
    }

    public static void Main2()
    {
        Alphabet a = new Alphabet(["0", "1"]);
        Dfa dfa1 = new Dfa(a);
        dfa1.SetInitial(0);
        dfa1.SetFinal(0);
        dfa1.Add(new Transition(0, 0, 0));
        dfa1.Add(new Transition(0, 1, 0));

        Dfa dfa2 = new Dfa(a);
        dfa2.SetInitial(0);
        dfa2.SetFinal(2);
        dfa2.Add(new Transition(0, 0, 1));
        dfa2.Add(new Transition(0, 1, 1));
        dfa2.Add(new Transition(1, 1, 2));

        //Dfa actual = dfa1.Intersection(dfa2);

        //Graph graph = GraphFactory.CreateGraph(dfa1);
        //GraphView graphView = GraphView.OpenNew(graph);

        //graph = GraphFactory.CreateGraph(dfa2);
        //graphView = GraphView.OpenNew(graph);

        //Graph graph = GraphFactory.CreateGraph(actual);
        //GraphView graphView = GraphView.OpenNew(graph);
    }
    public static void Main3()
    {
        //Transition[] transitions = new[]
        //{
        //    new Transition(0, 1, 1),
        //    new Transition(1, 0, 2),
        //    new Transition(1, 1, 2),
        //    new Transition(2, 0, 1),
        //};
        //ImmutableTransitions trans = new(transitions);

        //Console.WriteLine("Expecting: 0, 1, 1: " + trans.Transition(0, 1));
        //Console.WriteLine("Expecting: 1, 0, 2: " + trans.Transition(1, 0));
        //Console.WriteLine("Expecting: 1, 1, 2: " + trans.Transition(1, 1));
        //Console.WriteLine("Expecting: 2, 0, 1: " + trans.Transition(2, 0));
        //Console.WriteLine("Expecting invalid: " + trans.Transition(0, 0));
        //Console.WriteLine("Expecting invalid: " + trans.Transition(0, 2));
        //Console.WriteLine("Expecting invalid: " + trans.Transition(1, 2));
        //Console.WriteLine("Expecting invalid: " + trans.Transition(2, 1));
        //string[] strings = new string[] { "2", "1", "0" };

        //Alphabet a1 = new Alphabet(strings);
        //Console.WriteLine(a1.ToStringExpanded());
        //Console.WriteLine();
        //Alphabet a2 = new Alphabet(strings.OrderBy(s => s, StringComparer.Ordinal));
        //Console.WriteLine(a2.ToStringExpanded());
        //return;

        Console.WriteLine("Creating graph."); // Write some text output to the console window

        string[][] sequences = [
             ["1", "2", "2"],
             ["1", "1", "2"],
             ["1", "0", "2"],
             ["0", "2", "1"],
             ["0", "1", "1"],
             ["0", "0", "1"],
             ];
        //Random random = new Random(17);
        //var sequences = Enumerable.Range(0, 5).Select(_ => Enumerable.Range(0, 5).Select(_ => random.Next(4).ToString())); //Create some random sequences
        
        IFsa fsa = new Nfa(sequences).ToDfa().Minimized();

        Graph graph = GraphFactory.CreateGraph(fsa); // Create a graph object to display using the sequences

        //Graph graph = GraphFactory.CreateGraph(sequences); //Alternatively you can use this command, to replace the 2 lines above

        //GraphView.OpenNew(graph); // Open a new non-modal interactive window that displays the graph in it
        Console.WriteLine("FSA: \n" + fsa.Alphabet.ToStringExpanded());
        Cfa cfa = new Cfa(fsa);
        Console.WriteLine("CFA: \n" + cfa.Alphabet.ToStringExpanded());
        GraphView.OpenNew(cfa.CreateGraph(displayStateIDs: true));

        Console.WriteLine("StateCount: " + cfa.StateCount);
        for (int state = 0; state < cfa.StateCount; state++)
        {
            Console.WriteLine("State: " + state);
            var tra = cfa.Transitions(state);
            foreach (Transition t in tra)
            {
                Console.WriteLine("\t" + t);
            }
        }

        Console.WriteLine("Graph is displayed."); // Write some text output to the console window
    }



}


