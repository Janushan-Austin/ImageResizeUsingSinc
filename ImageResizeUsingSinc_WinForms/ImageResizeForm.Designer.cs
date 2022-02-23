
namespace ImageResizeUsingSinc
{
    partial class ImageResizeForm
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
            this.LoadImageButton = new System.Windows.Forms.Button();
            this.ImagePanel = new System.Windows.Forms.Panel();
            this.ImagePictureBox = new System.Windows.Forms.PictureBox();
            this.ResizeImageButton = new System.Windows.Forms.Button();
            this.ImagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadImageButton
            // 
            this.LoadImageButton.Location = new System.Drawing.Point(9, 10);
            this.LoadImageButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.LoadImageButton.Name = "LoadImageButton";
            this.LoadImageButton.Size = new System.Drawing.Size(88, 20);
            this.LoadImageButton.TabIndex = 2;
            this.LoadImageButton.Text = "Load Image";
            this.LoadImageButton.UseVisualStyleBackColor = true;
            this.LoadImageButton.Click += new System.EventHandler(this.LoadImageButton_Click);
            // 
            // ImagePanel
            // 
            this.ImagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImagePanel.AutoScroll = true;
            this.ImagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ImagePanel.Controls.Add(this.ImagePictureBox);
            this.ImagePanel.Location = new System.Drawing.Point(103, 10);
            this.ImagePanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ImagePanel.Name = "ImagePanel";
            this.ImagePanel.Size = new System.Drawing.Size(517, 628);
            this.ImagePanel.TabIndex = 18;
            // 
            // ImagePictureBox
            // 
            this.ImagePictureBox.Location = new System.Drawing.Point(0, 0);
            this.ImagePictureBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ImagePictureBox.Name = "ImagePictureBox";
            this.ImagePictureBox.Size = new System.Drawing.Size(514, 626);
            this.ImagePictureBox.TabIndex = 0;
            this.ImagePictureBox.TabStop = false;
            // 
            // ResizeImageButton
            // 
            this.ResizeImageButton.Location = new System.Drawing.Point(9, 36);
            this.ResizeImageButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ResizeImageButton.Name = "ResizeImageButton";
            this.ResizeImageButton.Size = new System.Drawing.Size(88, 23);
            this.ResizeImageButton.TabIndex = 19;
            this.ResizeImageButton.Text = "Resize Image";
            this.ResizeImageButton.UseVisualStyleBackColor = true;
            this.ResizeImageButton.Click += new System.EventHandler(this.ResizeImageButton_Click);
            // 
            // ImageResizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 648);
            this.Controls.Add(this.ResizeImageButton);
            this.Controls.Add(this.ImagePanel);
            this.Controls.Add(this.LoadImageButton);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ImageResizeForm";
            this.Text = "Canny Edge Detection";
            this.ImagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadImageButton;
        private System.Windows.Forms.Panel ImagePanel;
        private System.Windows.Forms.PictureBox ImagePictureBox;
        private System.Windows.Forms.Button ResizeImageButton;
    }
}

