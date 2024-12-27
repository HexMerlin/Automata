
using Automata.Core;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;

namespace Automata.App;

/// <summary>
/// A sample program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
/// </summary>
public static class Program
{
    /// <summary>Creates a DFA accepting the regular expression: <c>s*</c></summary>
    private static Dfa CreateKleeneStarDfa(string s)
    {
        Alphabet a = new Alphabet([s]);
        Dfa dfa = new Dfa(a);
        dfa.SetInitial(0);
        dfa.SetFinal(0);
        dfa.Add(new SymbolicTransition(0, 0, 0));
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
        dfa.Add(new SymbolicTransition(0, 0, 0));
        dfa.Add(new SymbolicTransition(0, 1, 1));
        dfa.Add(new SymbolicTransition(1, 1, 1));
        return dfa;
    }

    public static void Main2()
    {
        Alphabet a = new Alphabet(["0", "1"]);
        Dfa dfa1 = new Dfa(a);
        dfa1.SetInitial(0);
        dfa1.SetFinal(0);
        dfa1.Add(new SymbolicTransition(0, 0, 0));
        dfa1.Add(new SymbolicTransition(0, 1, 0));

        Dfa dfa2 = new Dfa(a);
        dfa2.SetInitial(0);
        dfa2.SetFinal(2);
        dfa2.Add(new SymbolicTransition(0, 0, 1));
        dfa2.Add(new SymbolicTransition(0, 1, 1));
        dfa2.Add(new SymbolicTransition(1, 1, 2));

        Dfa actual = dfa1.Intersection(dfa2);

        //Graph graph = GraphFactory.CreateGraph(dfa1);
        //GraphView graphView = GraphView.OpenNew(graph);

        //graph = GraphFactory.CreateGraph(dfa2);
        //graphView = GraphView.OpenNew(graph);

        //Graph graph = GraphFactory.CreateGraph(actual);
        //GraphView graphView = GraphView.OpenNew(graph);
    }

    public static void Main()
    {
        Console.WriteLine("Creating graph."); // Write some text output to the console window

        var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString())); //Create some random sequences

        IFsa fsa = new Nfa(sequences).ToDfa().Minimized();

        Graph graph = GraphFactory.CreateGraph(fsa); // Create a graph object to display using the sequences

        //Graph graph = GraphFactory.CreateGraph(sequences); //Alternatively you can use this command, to replace the 2 lines above

        GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal interactive window that displays the graph in it

        Console.WriteLine("Graph is displayed."); // Write some text output to the console window
    }

}


