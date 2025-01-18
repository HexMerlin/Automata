using Automata.Core;
using Automata.Core.Alang;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;

namespace Automata.App;
public static class Program
{
    /// <summary>
    /// A sample program that demonstrates creating an FSA from a regex and displaying it.
    /// </summary>
    public static void Main()
    {
        var fsa = AlangRegex.Compile("(a? (b | c) )+");  // Create an FSA from a regex

        Console.WriteLine("Creating a minimal FSA and displaying it."); // Write some info to the console

        Graph graph = fsa.CreateGraph(displayStateIDs: true); // Create a graph object (FSA with layout) 

        GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal window that displays the graph

        Console.WriteLine("FSA is displayed."); // Write some info to the console
    }
}


