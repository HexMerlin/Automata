using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

namespace Automata.Visualization;

///<summary>
/// A class for displaying finite-state automata as graphs in a separate window.
///</summary>
///<remarks>
///This class uses the MSAGL library for rendering and displaying graphs.
///</remarks>
public partial class GraphView : Form
{
    ///<summary>
    /// Gets the GViewer control used to display the graph.
    ///</summary>
    internal GViewer GViewer => gViewer;

    public bool IsAlive => !Disposing && !IsDisposed;

    ///<summary>
    /// Initializes a new instance of the <see cref="GraphView"/> class.
    ///</summary>
    private GraphView()
    {
        InitializeComponent();
        GViewer.PanButtonPressed = true;
       
    }

    public static GraphView Create()
    {
        using ManualResetEvent guiInitialized = new ManualResetEvent(false);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
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

            graphView.Visible = false;
            Application.Run(new ApplicationContext(graphView));
          

        });
        guiThread.SetApartmentState(ApartmentState.STA);
        guiThread.Start();
        guiInitialized.WaitOne();

        return graphView!;
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
     
        // Safe to update the GViewer directly here
        this.GViewer.Graph = graph;
        this.Show();
        this.Activate();
    }

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
