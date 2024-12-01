using Automata;
using Automata.Visualization;
using Microsoft.Msagl.Drawing;
namespace Automata.App;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ConsoleWindow consoleWindow = ConsoleWindow.Create(); // Creates the main console window.
        consoleWindow.WriteLine("Creating graph.", System.Drawing.Color.Blue); // Write some colored text output to the console window
        Graph graph = CreateGraph(); // Create a graph object to display
        consoleWindow.ShowGraph(graph); // Open a new window that displays the graph in it. This line does not block the code execution below.
        consoleWindow.WriteLine("Graph created.", System.Drawing.Color.Green); // Write some more colored text output to the console window
        
        Thread.Sleep(4000); // Wait for 5 seconds
        consoleWindow.WriteLine("Exiting.", System.Drawing.Color.Red); // Write some more colored text output to the console window. This line is executed even if the graph window is closed


    }

    public static Graph CreateGraph()
    {
        NFA nfa = new NFA();

        Random random = new Random();
        for (int i = 0; i < 10; i++)
        {
            var sequence = Enumerable.Range(0, 8).Select(_ => random.Next(4).ToString());
            nfa.Add(sequence);

        }
        DFA dfa = nfa.ToDFA(); //determinize the nfa
        dfa = dfa.Minimized(); //minimize the dfa

        var graph = GraphFactory.CreateGraph(dfa);


        return graph;
    }

}