using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = System.Drawing.Color;

namespace Automata.Visualization;
public partial class GraphView : Form
{
    
    internal GViewer GViewer => gViewer;

    public GraphView()
    {
        InitializeComponent();
        GViewer.PanButtonPressed = true;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
