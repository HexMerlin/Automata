using Automata.Core;
using Automata.Core.Operations;
using Automata.Core.Alang;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;

namespace Automata.App;

public static class Program
{
    public static void Display(string alangRegex)
        => GraphView.OpenNew(AlangRegex.Parse(alangRegex).Compile().CreateGraph());

    /// <summary>
    /// A sample program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
    /// </summary>
    public static void Main()
    {
        var regex = AlangRegex.Parse("a (b|c)* d"); //Create a regex from a string
        var fsa = regex.Compile(); //Compile the regex to a minimized automaton


        Console.WriteLine(fsa.ToCanonicalString()); //output the automaton to the console



        //Console.WriteLine("Creating graph."); // Write some text output to the console window

        //IFsa fsa = new Nfa(sequences).AsMfa(); //Create a minimized automaton from the sequences

        //Graph graph = fsa.CreateGraph(displayStateIDs: true); // Create a displayable graph object (FSA wih layout)

        ////Graph graph = sequences.CreateGraph(); //Alternatively you can use this command, to replace the 2 lines above

        //GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal interactive window that displays the graph in it

        //Console.WriteLine("Graph is displayed."); // Write some text output to the console window
    }

}


