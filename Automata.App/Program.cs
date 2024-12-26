
using Automata.Core;
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

        var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString())); //Create some random sequences

        IFsa fsa = new Nfa(sequences).ToDfa().Minimized();

        Graph graph = GraphFactory.CreateGraph(fsa); // Create a graph object to display using the sequences

        //Graph graph = GraphFactory.CreateGraph(sequences); //Alternatively you can use this command, to replace the 2 lines above

        GraphView graphView = GraphView.OpenNew(graph); // Open a new non-modal interactive window that displays the graph in it

        Console.WriteLine("Graph is displayed."); // Write some text output to the console window
    }

}


