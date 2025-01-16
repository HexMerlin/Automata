﻿using Automata.Core;
using Automata.Core.Operations;
using Automata.Core.Alang;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;

namespace Automata.App;

public static class Program
{
    public static void Display(IFsa fsa)
        => GraphView.OpenNew(fsa.CreateGraph());

    public static void Display(string alangRegex)
    => GraphView.OpenNew(AlangRegex.Parse(alangRegex).Compile().CreateGraph());

    public static void Display(string alangRegex, Alphabet alphabet)
    => GraphView.OpenNew(AlangRegex.Parse(alangRegex).Compile(alphabet).CreateGraph());

    public static void Main()
    {
        //Test cases:
        // (a-a) b   (should  be empty lang - not b!!)

        //AlangRegex regex = AlangRegex.Parse("(((a-b))|c&d)");
        //("(a a* b)+ a a* b*", "(a+ b) a+ (a | b)* b*");
        Alphabet alphabet = new Alphabet("a", "b");
        //"(a | b)* - (a | b)", "a", "b"

        Display(".~", alphabet);
        Display("(a | b)* - (a | b)", alphabet);
        //Display("(a+ b)+");
        return;

        AlangRegex regex = AlangRegex.Parse("(a a* b)+ a a* b*");
        Console.WriteLine(regex.AlangExpressionString);
        //"(a+ b (a+ b)*)");
        
        // AlangRegex regex = AlangRegex.Parse("a & a");

        //aa bb
        //aa aa aa
        //aa aa bb

        //Alphabet alphabet = new Alphabet();
        //var f1 = new Mfa("a", alphabet).AsNfa();
        //var f2 = new Mfa("b", alphabet).AsIDfa();

        //Ops.UnionWith(f1, f2);
        //Display(f1);

        //var res = Ops.Union(m1, m2);
        //Display(res);
        // var fsa = m1.AsNfa().UnionWith(m2.AsIDfa());
        // var fsa = m1.AsNfa();
        Mfa mfa = regex.Compile();
        Console.WriteLine("\"" + mfa.ToCanonicalString() + "\"");
        Graph graph = mfa.CreateGraph();
        GraphView graphView = GraphView.OpenNew(graph);
    } 

    /// <summary>
    /// A sample program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
    /// </summary>
    public static void Main2()
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

}


