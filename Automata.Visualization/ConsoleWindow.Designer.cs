namespace Automata.Visualization;

partial class ConsoleWindow
{

    private RichTextBox mainText;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.mainText = new RichTextBox();
        SuspendLayout();
        // 
        // mainText
        // 
        this.mainText.BackColor = Color.Black;
        this.mainText.Dock = DockStyle.Fill;
        this.mainText.Font = new Font("Consolas", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        this.mainText.ForeColor = Color.FromArgb(192, 192, 255);
        this.mainText.Location = new Point(0, 0);
        this.mainText.Name = "mainText";
        this.mainText.Size = new Size(1920, 904);
        this.mainText.TabIndex = 0;
        this.mainText.Text = "";
        // 
        // ConsoleWindow
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1920, 904);
        Controls.Add(this.mainText);
        Name = "ConsoleWindow";
        Text = "Console";
        ResumeLayout(false);
    }

    #endregion

}