namespace OSC_UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            OscillatorsListPanel = new Panel();
            SuspendLayout();
            // 
            // OscillatorsListPanel
            // 
            OscillatorsListPanel.Location = new Point(12, 70);
            OscillatorsListPanel.Name = "OscillatorsListPanel";
            OscillatorsListPanel.Size = new Size(776, 381);
            OscillatorsListPanel.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 33, 38);
            ClientSize = new Size(800, 450);
            Controls.Add(OscillatorsListPanel);
            Name = "Form1";
            Text = "Form1";
            FormClosed += formClose;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Panel OscillatorsListPanel;
    }
}