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
        private static readonly List<int> SincMultiplicationConstants = new List<int> {-9,13,-21,64,64,-21,13,-9 };
        private delegate Pixel Sinc(List<Pixel>Pixels); //TODO: Add grayscale
        private Sinc SincFunction = (List<Pixel> pixels) =>
        {
            if(pixels == null || pixels.Count != 8)
            {
                throw new ArgumentException("Pixels must have the 8 neighboring elments on either the same row or column", "Pixels");
            }
            Func<List<int>,byte> colorComponentSummation = (List<int> componentValues) =>
            {
                int summationValue = 47;
                for(int i = 0; i < componentValues.Count; i++)
                {
                    summationValue += componentValues[i] * SincMultiplicationConstants[i];
                }
                summationValue /= 94;
                summationValue = Math.Max(0, Math.Min(255, summationValue));
                if(summationValue > 255 || summationValue < 0)
                {
                    byte returnValue = (byte)summationValue;
                    summationValue++;
                    summationValue--;
                }
                return (byte)summationValue;
            };
            byte red = colorComponentSummation(pixels.Select(p => (int)p.Red).ToList());
            byte green = colorComponentSummation(pixels.Select(p => (int)p.Green).ToList());
            byte blue = colorComponentSummation(pixels.Select(p => (int)p.Blue).ToList());

            return new Pixel(red, green, blue);
        };
        public ImageResizeResultForm(Bitmap bitmapBitmap, string imageName, bool grayscale = false)
        {
            InitializeComponent();
            this.Text = $"{imageName} Resize Reults";
            OriginalCanvas.Image = bitmapBitmap;
            GenerateResizedImages();
        }

        private void GenerateResizedImages()
        {
            DoubleSizeCanvas.Image = DoubleImage((Bitmap)OriginalCanvas.Image);
            QuadrupleSizeCanvas.Image = DoubleImage((Bitmap)DoubleSizeCanvas.Image);
        }

        private Bitmap DoubleImage(Bitmap bitmap)
        {
            Bitmap doubledBitmap = new Bitmap(bitmap.Width * 2, bitmap.Height * 2, bitmap.PixelFormat);
            BitmapData originalBitmapData = new BitmapData(bitmap);
            BitmapData doubledBitmapData = new BitmapData(doubledBitmap);
            
            Queue<Pixel> cachedPixels = new Queue<Pixel>();
            
            int fourthPixelDistance = originalBitmapData.BytesPerPixel * 4;
            int lastColIndex = (originalBitmapData.Width - originalBitmapData.BytesPerPixel);

            int originalHeightStride = originalBitmapData.Height * originalBitmapData.Stride;
            int doubleBytesPerPixel = 2 * originalBitmapData.BytesPerPixel;
            int doubledDoubleBytesPerPixel = 2 * doubledBitmapData.BytesPerPixel;
            int doubledDoubleStride = doubledBitmapData.Stride * 2;

            for (int originalRow = 0, doubledRow = 0; originalRow < originalHeightStride; originalRow += originalBitmapData.Stride, doubledRow += doubledDoubleStride)
            {
                for (int x = 0, doubledX = 0; x < originalBitmapData.Width; x += originalBitmapData.BytesPerPixel, doubledX += doubledDoubleBytesPerPixel)
                {
                    doubledBitmapData[doubledRow + doubledX] = originalBitmapData[originalRow + x];
                }
            }

            for (int row = 0, doubleRow = 0; row < originalHeightStride; row += originalBitmapData.Stride, doubleRow += doubledDoubleStride)
            {
                cachedPixels.Clear();
                for (int x = -3; x < 4; x++)
                {
                    int byteIndex = row + Math.Abs(x) * originalBitmapData.BytesPerPixel;
                    //byte blue = originalBitmapData.Bytes[byteIndex];
                    //byte green = originalBitmapData.Bytes[byteIndex + 1];
                    //byte red = originalBitmapData.Bytes[byteIndex + 2];
                    //cachedPixels.Enqueue(new Pixel(red, green, blue));
                    cachedPixels.Enqueue(originalBitmapData[byteIndex]);
                }
                for (int x = 0, estimatedPixelIndex = originalBitmapData.BytesPerPixel; x < originalBitmapData.Width; x += originalBitmapData.BytesPerPixel, estimatedPixelIndex += doubleBytesPerPixel)
                {
                    int fourthPixelByteIndex = x + fourthPixelDistance;
                    if (fourthPixelByteIndex >= originalBitmapData.Width)
                    {
                        fourthPixelByteIndex = lastColIndex + (lastColIndex - fourthPixelByteIndex);
                    }
                    //byte blue = originalBitmapData.Bytes[row + fourthPixelByteIndex];
                    //byte green = originalBitmapData.Bytes[row + fourthPixelByteIndex + 1];
                    //byte red = originalBitmapData.Bytes[row + fourthPixelByteIndex + 2];

                    //cachedPixels.Enqueue(new Pixel(red, green, blue));
                    cachedPixels.Enqueue(originalBitmapData[row + fourthPixelByteIndex]);

                    Pixel estimatedPixel = SincFunction(cachedPixels.ToList());
                    doubledBitmapData[doubleRow + estimatedPixelIndex] = estimatedPixel;
                    //doubledBitmapData.Bytes[doubleRow + estimatedPixelIndex] = estimatedPixel.Blue;
                    //doubledBitmapData.Bytes[doubleRow + estimatedPixelIndex + 1] = estimatedPixel.Green;
                    //doubledBitmapData.Bytes[doubleRow + estimatedPixelIndex + 2] = estimatedPixel.Red;

                    cachedPixels.Dequeue();
                }
            }

            int doubledEightStrides = doubledBitmapData.Stride * 8;
            int heightStride = doubledBitmapData.Height * doubledBitmapData.Stride;
            int lastRowIndexStart = (doubledBitmapData.Height - 1) * doubledBitmapData.Stride;
            for (int x = 0, lastRowIndex = lastRowIndexStart; x < doubledBitmapData.Width; x += doubledBitmapData.BytesPerPixel, lastRowIndex += doubledBitmapData.BytesPerPixel)
            {
                //int lastRowIndex = lastRowIndexStart+ x;
                cachedPixels.Clear();
                for (int y = -3; y < 4; y++)
                {
                    int byteIndex = x + Math.Abs(y) * doubledDoubleStride;
                    //byte blue = doubledBitmapData.Bytes[byteIndex];
                    //byte green = doubledBitmapData.Bytes[byteIndex + 1];
                    //byte red = doubledBitmapData.Bytes[byteIndex + 2];
                    //cachedPixels.Enqueue(new Pixel(red, green, blue));
                    cachedPixels.Enqueue(doubledBitmapData[byteIndex]);
                }
                for (int y = 0, estimatedPixelIndex = doubledBitmapData.Stride + x; y < heightStride; y += doubledDoubleStride, estimatedPixelIndex += doubledDoubleStride)
                {
                    int fourthPixelByteIndex = y + doubledEightStrides + x;
                    if (fourthPixelByteIndex >= heightStride)
                    {
                        fourthPixelByteIndex = lastRowIndex + (lastRowIndex - fourthPixelByteIndex);
                    }
                    //byte blue = doubledBitmapData.Bytes[fourthPixelByteIndex];
                    //byte green = doubledBitmapData.Bytes[fourthPixelByteIndex + 1];
                    //byte red = doubledBitmapData.Bytes[fourthPixelByteIndex + 2];

                    //cachedPixels.Enqueue(new Pixel(red, green, blue));
                    cachedPixels.Enqueue(doubledBitmapData[fourthPixelByteIndex]);

                    Pixel estimatedPixel = SincFunction(cachedPixels.ToList());
                    doubledBitmapData[estimatedPixelIndex] = estimatedPixel;
                    //doubledBitmapData.Bytes[estimatedPixelIndex] = estimatedPixel.Blue;
                    //doubledBitmapData.Bytes[estimatedPixelIndex + 1] = estimatedPixel.Green;
                    //doubledBitmapData.Bytes[estimatedPixelIndex + 2] = estimatedPixel.Red;

                    cachedPixels.Dequeue();
                }
            }
            //return doubledBitmapData.WriteToBitmap();
            return doubledBitmap;
        }
    }
    public class Pixel
    {
        public byte Red;
        public byte Green;
        public byte Blue;

        public Pixel(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }
    public class BitmapData
    {
        public byte[] Bytes;
        public IntPtr PixelPtr { get; private set; }
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

        public Pixel this[int index]
        {
            get
            {
                IntPtr pixelPtr = IntPtr.Add(PixelPtr, index);
                byte blue = Marshal.ReadByte(pixelPtr);
                return new Pixel(Marshal.ReadByte(pixelPtr,2), Marshal.ReadByte(pixelPtr, 1), Marshal.ReadByte(pixelPtr));
            }
            set
            {
                IntPtr bluePtr = IntPtr.Add(PixelPtr, index);
                IntPtr greenPtr = IntPtr.Add(PixelPtr, index + 1);
                IntPtr redPtr = IntPtr.Add(PixelPtr, index + 2);
                Marshal.WriteByte(bluePtr, value.Blue);
                Marshal.WriteByte(greenPtr, value.Green);
                Marshal.WriteByte(redPtr, value.Red);
            }
        }

        public Pixel this[int row, int col]
        {
            get => this[row * Stride + col];
            set => this[row * Stride + col] = value;
        }

        public void ReadBitmapData(Bitmap bitmap)
        {
            PixelFormat = bitmap.PixelFormat;
            BitmapWidth = bitmap.Width;
            BitmapHeight = bitmap.Height;

            Bitmap copyBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            
            BytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            PixelPtr = bitmapData.Scan0;
            Height = bitmapData.Height;
            Width = bitmapData.Width * BytesPerPixel;
            Stride = Math.Abs(bitmapData.Stride);

            //Bytes = new byte[Stride * Height];
            //Marshal.Copy(bitmapData.Scan0, Bytes, 0, Bytes.Length);
            bitmap.UnlockBits(bitmapData);
        }

        public Bitmap WriteToBitmap()
        {
            //GCHandle pinned = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            //IntPtr bytesPtr = pinned.AddrOfPinnedObject();
            
            Bitmap bitmap = new Bitmap(BitmapWidth, BitmapHeight, PixelFormat);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, BitmapWidth, BitmapHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat);

            Marshal.Copy(Bytes, 0, bitmapData.Scan0, Bytes.Length);

            bitmap.UnlockBits(bitmapData);
            //pinned.Free();
            return bitmap;
        }
    }
}
