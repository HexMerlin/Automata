using Automata;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;
namespace Automata.App;

/// <summary>
/// A sample program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
/// </summary>
internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>

    static void Main()
    {
        ConsoleWindow consoleWindow = ConsoleWindow.Create(); // Creates the main console window.
        consoleWindow.WriteLine("Creating graph...", System.Drawing.Color.Blue); // Write some colored text output to the console window
        var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString())); //create some random sequences
        Graph graph = GraphFactory.CreateGraph(sequences, minimize: true); // Create a graph object to display using the sequences
        consoleWindow.ShowGraph(graph); // Open a new non-modal window that displays the graph in it. 
        consoleWindow.WriteLine("Graph created.", System.Drawing.Color.Green); // Write some more colored text output to the console window
    }


}