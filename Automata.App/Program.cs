using Automata.Core;
using Automata.Core.Operations;
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
        //Create a regex (for all non-empty permutations of {a, b, c}, where 'a' must be followed by b or c)
        //var regex = AlangRegex.Parse("(a? (b | c) )+");
        //var regex = AlangRegex.Parse("(a | b)* (a a) (a | b)* ");
        //var regex = AlangRegex.Parse("(.* a a .*)");

        Mfa fsa = AlangRegex.Compile("(a? (b | c) )+"); // Compile a regex to an FSA
                

        //var fsa1 = AlangRegex.Compile(".* a a .*", "b");


        //var regex = AlangRegex.Parse("(a | b)* a a (a | b)*");
        // var regex = AlangRegex.Parse(".*");
        //compile to an FSA

        Console.WriteLine("Creating a graph and display it."); // Write some info to the console window

        Graph graph = fsa.CreateGraph(displayStateIDs: true); // Create a graph object (FSA with layout) 

        GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal window that displays the graph

        Console.WriteLine("Graph is displayed."); // Write some text output to the console window
    }

}


