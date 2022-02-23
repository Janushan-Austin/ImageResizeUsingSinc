
namespace ImageResizeUsingSinc
{
    partial class ImageResizeResultForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.QuadrupleSizeTab = new System.Windows.Forms.TabPage();
            this.QuadrupleSizeCanvas = new System.Windows.Forms.PictureBox();
            this.DoubleSizeTab = new System.Windows.Forms.TabPage();
            this.DoubleSizeCanvas = new System.Windows.Forms.PictureBox();
            this.PhaseTabsControl = new System.Windows.Forms.TabControl();
            this.OriginalImageTab = new System.Windows.Forms.TabPage();
            this.OriginalCanvas = new System.Windows.Forms.PictureBox();
            this.QuadrupleSizeTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QuadrupleSizeCanvas)).BeginInit();
            this.DoubleSizeTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DoubleSizeCanvas)).BeginInit();
            this.PhaseTabsControl.SuspendLayout();
            this.OriginalImageTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OriginalCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // QuadrupleSizeTab
            // 
            this.QuadrupleSizeTab.AutoScroll = true;
            this.QuadrupleSizeTab.Controls.Add(this.QuadrupleSizeCanvas);
            this.QuadrupleSizeTab.Location = new System.Drawing.Point(4, 22);
            this.QuadrupleSizeTab.Name = "QuadrupleSizeTab";
            this.QuadrupleSizeTab.Padding = new System.Windows.Forms.Padding(3);
            this.QuadrupleSizeTab.Size = new System.Drawing.Size(783, 431);
            this.QuadrupleSizeTab.TabIndex = 1;
            this.QuadrupleSizeTab.Text = "Quadruple Size";
            // 
            // QuadrupleSizeCanvas
            // 
            this.QuadrupleSizeCanvas.Location = new System.Drawing.Point(4, 3);
            this.QuadrupleSizeCanvas.Name = "QuadrupleSizeCanvas";
            this.QuadrupleSizeCanvas.Size = new System.Drawing.Size(775, 425);
            this.QuadrupleSizeCanvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.QuadrupleSizeCanvas.TabIndex = 1;
            this.QuadrupleSizeCanvas.TabStop = false;
            // 
            // DoubleSizeTab
            // 
            this.DoubleSizeTab.AutoScroll = true;
            this.DoubleSizeTab.Controls.Add(this.DoubleSizeCanvas);
            this.DoubleSizeTab.Location = new System.Drawing.Point(4, 22);
            this.DoubleSizeTab.Name = "DoubleSizeTab";
            this.DoubleSizeTab.Padding = new System.Windows.Forms.Padding(3);
            this.DoubleSizeTab.Size = new System.Drawing.Size(783, 431);
            this.DoubleSizeTab.TabIndex = 0;
            this.DoubleSizeTab.Text = "Double Size";
            // 
            // DoubleSizeCanvas
            // 
            this.DoubleSizeCanvas.Location = new System.Drawing.Point(3, 6);
            this.DoubleSizeCanvas.Name = "DoubleSizeCanvas";
            this.DoubleSizeCanvas.Size = new System.Drawing.Size(775, 425);
            this.DoubleSizeCanvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.DoubleSizeCanvas.TabIndex = 0;
            this.DoubleSizeCanvas.TabStop = false;
            // 
            // PhaseTabsControl
            // 
            this.PhaseTabsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PhaseTabsControl.Controls.Add(this.OriginalImageTab);
            this.PhaseTabsControl.Controls.Add(this.DoubleSizeTab);
            this.PhaseTabsControl.Controls.Add(this.QuadrupleSizeTab);
            this.PhaseTabsControl.Location = new System.Drawing.Point(12, 12);
            this.PhaseTabsControl.Name = "PhaseTabsControl";
            this.PhaseTabsControl.SelectedIndex = 0;
            this.PhaseTabsControl.Size = new System.Drawing.Size(791, 457);
            this.PhaseTabsControl.TabIndex = 1;
            // 
            // OriginalImageTab
            // 
            this.OriginalImageTab.AutoScroll = true;
            this.OriginalImageTab.Controls.Add(this.OriginalCanvas);
            this.OriginalImageTab.Location = new System.Drawing.Point(4, 22);
            this.OriginalImageTab.Name = "OriginalImageTab";
            this.OriginalImageTab.Size = new System.Drawing.Size(783, 431);
            this.OriginalImageTab.TabIndex = 5;
            this.OriginalImageTab.Text = "Original";
            // 
            // OriginalCanvas
            // 
            this.OriginalCanvas.Location = new System.Drawing.Point(4, 3);
            this.OriginalCanvas.Name = "OriginalCanvas";
            this.OriginalCanvas.Size = new System.Drawing.Size(775, 425);
            this.OriginalCanvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.OriginalCanvas.TabIndex = 1;
            this.OriginalCanvas.TabStop = false;
            // 
            // ImageResizeResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 475);
            this.Controls.Add(this.PhaseTabsControl);
            this.Name = "ImageResizeResultForm";
            this.Text = "CannyEdgePhaseForm";
            this.QuadrupleSizeTab.ResumeLayout(false);
            this.QuadrupleSizeTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.QuadrupleSizeCanvas)).EndInit();
            this.DoubleSizeTab.ResumeLayout(false);
            this.DoubleSizeTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DoubleSizeCanvas)).EndInit();
            this.PhaseTabsControl.ResumeLayout(false);
            this.OriginalImageTab.ResumeLayout(false);
            this.OriginalImageTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OriginalCanvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage QuadrupleSizeTab;
        public System.Windows.Forms.PictureBox QuadrupleSizeCanvas;
        private System.Windows.Forms.TabPage DoubleSizeTab;
        public System.Windows.Forms.PictureBox DoubleSizeCanvas;
        public System.Windows.Forms.TabControl PhaseTabsControl;
        private System.Windows.Forms.TabPage OriginalImageTab;
        public System.Windows.Forms.PictureBox OriginalCanvas;
    }
}