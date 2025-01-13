using Automata.Core;
using Automata.Core.Alang;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;

namespace Automata.App;

public static class Program
{

    /// <summary>
    /// A sample program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
    /// </summary>
    public static void Main()
    {
        Console.WriteLine("Creating graph."); // Write some text output to the console window

        Random random = new Random(7);
        var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => random.Next(4).ToString())); //Create some random string sequences

        IFsa fsa = new Nfa(sequences).AsMfa(); //Create a minimized automaton from the sequences

        Graph graph = fsa.CreateGraph(displayStateIDs: true); // Create a displayable graph object (FSA wih layout)

        //Graph graph = sequences.CreateGraph(); //Alternatively you can use this command, to replace the 2 lines above

        GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal interactive window that displays the graph in it

        Console.WriteLine("Graph is displayed."); // Write some text output to the console window
    }

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




}


