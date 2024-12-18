
using Automata.Core;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;
using Spectre.Console;

namespace Automata.App;

/// <summary>
/// A sample program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
/// </summary>
public static class Program
{
    public static void Main()
    {

        Console.WriteLine("Creating graph..."); // Write some text output to the console window

        var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString())); //Create some random sequences

  
        IFsa fsa = new NFA(sequences).ToDFA().Minimized();
        Graph graph = GraphFactory.CreateGraph(fsa); // Create a graph object to display using the sequences

        //Alternatively: you can replace the two lines above, with the line below which creates a minimized graph directly from a collection of sequences
        //Graph graph = GraphFactory.CreateGraph(sequences, minimize: true); // Create a displayable graph object directly using the sequences

        GraphView graphView = GraphView.Create(); //Create a non-modal window for displaying graphs

       // graphView.ShowGraph(graph); //Display the graph in the graph view

        AnsiConsole.MarkupLine("Graph is displayed in a separate window"); 

    }



    /// <summary>
    /// Displays the specified graph in the graph view.
    /// </summary>
    /// <param name="graph">The graph to display.</param>
    //public static void ShowGraph(Graph graph)
    //{
    //    if (!IsAlive) return;
    //    // Check if we're on the UI thread
    //    if (InvokeRequired)
    //    {
    //        // If not on UI thread, re-invoke this method on the UI thread
    //        Invoke(new Action(() => ShowGraph(graph)));
    //        return;
    //    }
    //    GraphView graphView = new GraphView();
    //    // Safe to update the GViewer directly here
    //    graphView.GViewer.Graph = graph;
    //    graphView.Show();
    //    graphView.Activate();
    //}
}
