using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

namespace Automata.Visualization;

///<summary>
/// Class for displaying finite-state automata as graphs in a separate window (and its own separate thread).
///</summary>
///<remarks>
/// You do not need to involve any GUI boilerplate code to display a graph in a separate window, like calling the blocking `Application.Run()`, setting STA thread environment or bother about the GUI messes with your threads.
/// Simply just create and open a graph view by calling either <see cref="GraphView.OpenNew()"/> or <see cref="GraphView.OpenNew(Graph)"/>.
/// This class uses the MSAGL library for layout and rendering of graphs.
///</remarks>
public partial class GraphView : Form
{
    ///<summary>
    /// GViewer control used to display the graph.
    ///</summary>
    internal GViewer GViewer => gViewer;

    static GraphView()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
    }

    ///<summary>
    /// Initializes a new instance of the <see cref="GraphView"/> class.
    ///</summary>
    private GraphView()
    {
        InitializeComponent();
        GViewer.PanButtonPressed = true;
    }

    ///<summary>
    /// Opens a new instance of the <see cref="GraphView"/> class in a new thread.
    ///</summary>
    ///<returns>A new instance of the <see cref="GraphView"/> class.</returns>
    public static GraphView OpenNew()
    {
        using ManualResetEvent guiInitialized = new ManualResetEvent(false);
        GraphView? graphView = null;
        Thread guiThread = new Thread(() =>
        {
            graphView = new GraphView();

            EventHandler handler = null!;
            handler = (sender, args) =>
            {
                guiInitialized.Set();
                graphView.Shown -= handler;
            };
            graphView.Shown += handler;

            Application.Run(graphView);
        });
        guiThread.SetApartmentState(ApartmentState.STA);
        guiThread.Start();
        guiInitialized.WaitOne();

        return graphView!;
    }

    ///<summary>
    /// Opens a new instance of the <see cref="GraphView"/> class in a new thread and sets the specified graph.
    ///</summary>
    ///<param name="graph">Graph to display.</param>
    ///<returns>A new instance of the <see cref="GraphView"/> class with the specified graph set.</returns>
    public static GraphView OpenNew(Graph graph)
    {
        GraphView graphView = OpenNew();
        graphView.SetGraph(graph);
        return graphView;
    }

    ///<summary>
    /// Invokes the specified action on the UI thread.
    ///</summary>
    ///<param name="action">Action to invoke.</param>
    public new void Invoke(Action action)
    {
        if (Disposing || IsDisposed) return;
        if (InvokeRequired)
            base.Invoke(action);
        else action();
    }

    /// <summary>
    /// Displays the specified graph in the graph view.
    /// </summary>
    /// <param name="graph">Graph to display.</param>
    public void SetGraph(Graph graph) => Invoke(() => GViewer.Graph = graph);

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GViewer?.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
