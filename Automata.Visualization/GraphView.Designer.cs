using Microsoft.Msagl.GraphViewerGdi;

namespace Automata.Visualization;

partial class GraphView
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private GViewer gViewer;


    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphView));
        this.gViewer = new GViewer();
        SuspendLayout();
        // 
        // gViewer
        // 
        this.gViewer.ArrowheadLength = 10D;
        this.gViewer.AsyncLayout = false;
        this.gViewer.AutoScroll = true;
        this.gViewer.BackwardEnabled = false;
        this.gViewer.BuildHitTree = true;
        this.gViewer.CurrentLayoutMethod = LayoutMethod.UseSettingsOfTheGraph;
        this.gViewer.Dock = DockStyle.Fill;
        this.gViewer.EdgeInsertButtonVisible = true;
        this.gViewer.FileName = "";
        this.gViewer.ForwardEnabled = false;
        this.gViewer.Graph = null;
        this.gViewer.IncrementalDraggingModeAlways = false;
        this.gViewer.InsertingEdge = false;
        this.gViewer.LayoutAlgorithmSettingsButtonVisible = true;
        this.gViewer.LayoutEditingEnabled = true;
        this.gViewer.Location = new Point(0, 0);
        this.gViewer.LooseOffsetForRouting = 0.25D;
        this.gViewer.MouseHitDistance = 0.05D;
        this.gViewer.Name = "gViewer";
        this.gViewer.NavigationVisible = true;
        this.gViewer.NeedToCalculateLayout = true;
        this.gViewer.OffsetForRelaxingInRouting = 0.6D;
        this.gViewer.PaddingForEdgeRouting = 8D;
        this.gViewer.PanButtonPressed = false;
        this.gViewer.SaveAsImageEnabled = true;
        this.gViewer.SaveAsMsaglEnabled = true;
        this.gViewer.SaveButtonVisible = true;
        this.gViewer.SaveGraphButtonVisible = true;
        this.gViewer.SaveInVectorFormatEnabled = true;
        this.gViewer.Size = new Size(1573, 937);
        this.gViewer.TabIndex = 0;
        this.gViewer.TightOffsetForRouting = 0.125D;
        this.gViewer.ToolBarIsVisible = true;
        this.gViewer.Transform = (Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)resources.GetObject("gViewer.Transform");
        this.gViewer.UndoRedoButtonsVisible = true;
        this.gViewer.WindowZoomButtonPressed = false;
        this.gViewer.ZoomF = 1D;
        this.gViewer.ZoomWindowThreshold = 0.05D;
        // 
        // GraphView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1573, 937);
        Controls.Add(this.gViewer);
        Name = "GraphView";
        Text = "GraphView";
        ResumeLayout(false);
    }

    #endregion


}