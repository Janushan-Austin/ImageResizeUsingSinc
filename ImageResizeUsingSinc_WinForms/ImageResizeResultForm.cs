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
    public partial class ImageResizeResultForm : Form
    {
        private delegate Color Sinc(BitmapData bitmapData, int x, int y);
        private Sinc SincPointer;
        private Sinc ColorSinc = (BitmapData bitmapData, int x, int y) =>
        {
            return Color.Black;
        };
        private Sinc GrayscaleSinc = (BitmapData bitmapData, int x, int y) =>
        {
            return Color.White;
        };
        public ImageResizeResultForm(Bitmap bitmapBitmap, string imageName, bool grayscale = false)
        {
            InitializeComponent();
            this.Text = $"{imageName} Resize Reults";
            OriginalCanvas.Image = bitmapBitmap;
            SincPointer = grayscale == true ? GrayscaleSinc : ColorSinc;
        }

        private void GenerateResizedImages()
        {
            DoubleSizeCanvas.Image = DoubleImage((Bitmap)OriginalCanvas.Image);
            QuadrupleSizeCanvas.Image = DoubleImage((Bitmap)DoubleSizeCanvas.Image);
        }

        private Bitmap DoubleImage(Bitmap bitmap)
        {
            Bitmap doubledBitmap = new Bitmap(bitmap.Width * 2, bitmap.Height * 2);

            BitmapData originalBitmapData = new BitmapData(bitmap);
            BitmapData doubledBitmapData = new BitmapData(doubledBitmap);

            for(int y = 0; y < originalBitmapData.Height; y++)
            {
                for(int x = 0; x < originalBitmapData.Width; x += originalBitmapData.BytesPerPixel)
                {
                    SincPointer(originalBitmapData, x, y);
                }
            }

            return doubledBitmap;
        }

    }

    public class BitmapData
    {
        public byte[] Bytes;
        public int BytesPerPixel;
        public int Height;
        public int Width;
        public int Stride;
        private System.Drawing.Imaging.PixelFormat PixelFormat;
        private int BitmapWidth;
        private int BitmapHeight;

        public BitmapData(Bitmap bitmap)
        {
            ReadBitmapData(bitmap);
        }

        public void ReadBitmapData(Bitmap bitmap)
        {
            PixelFormat = bitmap.PixelFormat;
            BitmapWidth = bitmap.Width;
            BitmapHeight = bitmap.Height;

            Bitmap copyBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            
            BytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            Height = bitmapData.Height;
            Width = bitmapData.Width * BytesPerPixel;
            Stride = Math.Abs(bitmapData.Stride);

            Bytes = new byte[Stride * Height];
            Marshal.Copy(bitmapData.Scan0, Bytes, 0, Bytes.Length);
            bitmap.UnlockBits(bitmapData);
        }

        public Bitmap WriteToBitmap()
        {
            Bitmap bitmap = new Bitmap(BitmapWidth, BitmapHeight, PixelFormat);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, BitmapWidth, BitmapHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat);
            
            Marshal.Copy(Bytes, 0, bitmapData.Scan0, Bytes.Length);
            
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
