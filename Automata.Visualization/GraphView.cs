using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = System.Drawing.Color;

namespace Automata.Visualization;

///<summary>
/// A form that displays a graph using the MSAGL library.
///</summary>
internal partial class GraphView : Form
{
    ///<summary>
    /// Gets the GViewer control used to display the graph.
    ///</summary>
    internal GViewer GViewer => gViewer;

    ///<summary>
    /// Initializes a new instance of the <see cref="GraphView"/> class.
    ///</summary>
    public GraphView()
    {
        InitializeComponent();
        GViewer.PanButtonPressed = true;
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
