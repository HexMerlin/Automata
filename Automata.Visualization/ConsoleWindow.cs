using Microsoft.Msagl.Drawing;
using Color = System.Drawing.Color;

namespace Automata.Visualization;

///<summary>
///A class that represents a console window for displaying text output with the option to display graphs in a separate window.
///The class encapsulates and hides boilerplate code for the user, such as synchronizing with the GUI-thread or managing the lifecycle of windows.
///</summary>
///<example>Full program that demonstrates how to create a graph from a collection of sequences and display it in a separate window.
///<code>
///static void Main()
///{
///    // Create the main console window.
///    ConsoleWindow consoleWindow = ConsoleWindow.Create(); 
///    // Write some colored text output to the console window
///    consoleWindow.WriteLine("Creating graph...", System.Drawing.Color.Blue); 
///    //create some random sequences
///    var sequences = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 8).Select(_ => Random.Shared.Next(4).ToString()));
///    // Create a graph object to display using the sequences
///    Graph graph = GraphFactory.CreateGraph(sequences, minimize: true);
///    // Open a new non-modal interactive window that displays the graph in it. 
///    consoleWindow.ShowGraph(graph); 
///    // Write some more colored text output to the console window
///    consoleWindow.WriteLine("Graph created.", System.Drawing.Color.Green); 
///}
/// </code>
/// </example>

public partial class ConsoleWindow : Form
{
    public bool IsAlive => !Disposing && !IsDisposed;

    /// <summary>
    /// Creates a new ConsoleWindow on a separate thread and waits for it to be initialized.
    /// </summary>
    /// <returns>A new instance of <see cref="ConsoleWindow"/>.</returns>
    public static ConsoleWindow Create()
    {
        using ManualResetEvent guiInitialized = new ManualResetEvent(false);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        ConsoleWindow? consoleWindow = null;
        Thread guiThread = new Thread(() =>
        {
            consoleWindow = new ConsoleWindow();

            EventHandler handler = null!;
            handler = (sender, args) =>
            {
                guiInitialized.Set();
                consoleWindow.Shown -= handler;
            };
            consoleWindow.Shown += handler;

            Application.Run(consoleWindow);
        });
        guiThread.SetApartmentState(ApartmentState.STA);
        guiThread.Start();
        guiInitialized.WaitOne();

        return consoleWindow!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleWindow"/> class.
    /// </summary>
    private ConsoleWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Displays the specified graph in the graph view.
    /// </summary>
    /// <param name="graph">The graph to display.</param>
    public void ShowGraph(Graph graph)
    {
        if (!IsAlive) return;
        // Check if we're on the UI thread
        if (InvokeRequired)
        {
            // If not on UI thread, re-invoke this method on the UI thread
            Invoke(new Action(() => ShowGraph(graph)));
            return;
        }
        GraphView graphView = new GraphView();
        // Safe to update the GViewer directly here
        graphView.GViewer.Graph = graph;
        graphView.Show();
        graphView.Activate();
    }

    /// <summary>
    /// Writes a line of text to the console window.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="textColor">The color of the text. If null, the default color is used.</param>
    public void WriteLine(string text, Color? textColor = null) => Write(text + Environment.NewLine, textColor);

    /// <summary>
    /// Writes text to the console window.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="textColor">The color of the text. If null, the default color is used.</param>
    public void Write(string text, Color? textColor = null)
    {
        if (!IsAlive) return;

        if (InvokeRequired)
        {
            Invoke(new Action(() => Write(text, textColor)));
            return;
        }
        Color color = textColor ?? this.mainText.ForeColor;

        if (color != this.mainText.ForeColor)
        {
            this.mainText.SelectionStart = this.mainText.TextLength;
            this.mainText.SelectionLength = 0;
            this.mainText.SelectionColor = color;
        }
        this.mainText.AppendText(text);
    }

    /// <summary>
    /// Clears all text from the console window.
    /// </summary>
    public void ClearText()
    {
        if (!IsAlive) return;
        if (InvokeRequired)
        {
            Invoke(new Action(() => ClearText()));
            return;
        }
        this.mainText.Clear();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="ConsoleWindow"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
