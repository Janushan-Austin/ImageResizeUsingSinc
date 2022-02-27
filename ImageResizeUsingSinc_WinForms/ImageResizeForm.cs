using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageResizeUsingSinc
{
    public partial class ImageResizeForm : Form
    {
        string OpenImageFilename = "";
        public ImageResizeForm()
        {
            InitializeComponent();
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog imageFileDialog = new OpenFileDialog();
            imageFileDialog.Filter = "(*.jpg;*.jpeg; *.gif; *.bmp; *.png)| *.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (imageFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenImageFilename = new System.IO.FileInfo(imageFileDialog.FileName).Name;

                    //var bm = new Bitmap(800, 600);
                    //for (int y = 0; y < 600; y++)
                    //    for (int x = 0; x < 800; x++)
                    //        bm.SetPixel(x,y, Color.FromArgb(0,0,255));
                    //ImagePictureBox.Image = bm;
                    ImagePictureBox.Image = new Bitmap(imageFileDialog.FileName);
                    ImagePictureBox.Size = ImagePictureBox.Image.Size;
                    //SetColorComponentButtonsActive(true);
                }
                catch
                {
                    OpenImageFilename = "";
                    ImagePictureBox.Image = null;
                    //SetColorComponentButtonsActive(false);
                }
            }
        }

        private int Map(int x, int fromLower, int fromHigher, int toLower, int toHigher)
        {
            return (int)((x - fromLower) / (float)(fromHigher - fromLower) * (toHigher - toLower)) + toLower;
        }

        private void ResizeImageButton_Click(object sender, EventArgs e)
        {
            if(ImagePictureBox.Image == null)
            {
                return;
            }

            ImageResizeResultForm cannyPhasesForm = new ImageResizeResultForm((Bitmap)ImagePictureBox.Image, OpenImageFilename);
            //Bitmap copyBitmap = new Bitmap((ImagePictureBox.Image));
            //cannyPhasesForm.OriginalCanvas.Image = ImagePictureBox.Image;
            //cannyPhasesForm.Text = $"{OpenImageFilename} Canny Edge Phases";
            //cannyPhasesForm.PhaseTabsControl.SelectedIndex = cannyPhasesForm.PhaseTabsControl.TabCount - 1;
            cannyPhasesForm.Show();
        }

        private Bitmap ConvertToGrayScale(Bitmap original)
        {
            Bitmap copyBitmap = new Bitmap(original.Width, original.Height);
            var originalData = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, original.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(original.PixelFormat) / 8;

            int height = originalData.Height;
            int width = originalData.Width * bytesPerPixel;
            int stride = Math.Abs(originalData.Stride);

            byte[] transformationBytes = new byte[stride * height];
            Marshal.Copy(originalData.Scan0, transformationBytes, 0, transformationBytes.Length);
            original.UnlockBits(originalData);

            for (int y = 0; y < height; y++)
            {
                int row = y * stride;
                for (int x = 0; x < width; x += bytesPerPixel)
                {
                    int blue = transformationBytes[row + x];
                    int green = transformationBytes[row + x + 1];
                    int red = transformationBytes[row + x + 2];
                    byte luminace = (byte)(red * .299 + green * .587 + blue * .114);

                    transformationBytes[row + x] = luminace;
                    transformationBytes[row + x + 1] = luminace;
                    transformationBytes[row + x + 2] = luminace;
                }
            }
            var alteredData = copyBitmap.LockBits(new Rectangle(0, 0, original.Width, original.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, original.PixelFormat);
            Marshal.Copy(transformationBytes, 0, alteredData.Scan0, transformationBytes.Length);
            copyBitmap.UnlockBits(alteredData);
            return copyBitmap;
        }
    }
}
