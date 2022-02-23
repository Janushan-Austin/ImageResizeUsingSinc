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

            ImageResizeResultForm cannyPhasesForm = new ImageResizeResultForm();
            Bitmap copyBitmap = new Bitmap((ImagePictureBox.Image));
            cannyPhasesForm.OriginalCanvas.Image = ImagePictureBox.Image;

            //grayscale phase
            copyBitmap = ConvertToGrayScale(copyBitmap);
            cannyPhasesForm.GrayscaleCanvas.Image = copyBitmap;

            //smooth Phase
            copyBitmap = SmoothImage(copyBitmap);
            cannyPhasesForm.SmoothedCanvas.Image = copyBitmap;

            //gradients phase
            CannyEdgeCandidate[,] candidates = CalculateGradients(copyBitmap);

            Bitmap gradientsBM = new Bitmap(copyBitmap.Width, copyBitmap.Height);
            int max = candidates[0, 0].Gradient.ManhattanDistance;
            int min = candidates[0,0].Gradient.ManhattanDistance;
            for (int y = 0; y < gradientsBM.Height; y++)
            {
                for (int x = 0; x < gradientsBM.Width; x++)
                {
                    int manhattanDistance = candidates[y, x].Gradient.ManhattanDistance;
                    if (max < manhattanDistance)
                    {
                        max = manhattanDistance;
                    }
                    if(min > manhattanDistance)
                    {
                        min = manhattanDistance;
                    }
                }
            }

            for (int y = 0; y < gradientsBM.Height; y++)
            {
                for(int x = 0; x < gradientsBM.Width; x++)
                {
                    int gray = Map(Math.Abs(candidates[y, x].Gradient.ManhattanDistance), min, max, 0, 255);
                    //gradientsBM.SetPixel(x, y, Color.FromArgb(r, g, 0));
                    gradientsBM.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            cannyPhasesForm.GradientsCanvas.Image = gradientsBM;

            //NonMaximal Suppression phase
            //use manhattan distance to determine if p(x,y) is edge and not actual magnitude
            NonMaximalSuppression(candidates);
            gradientsBM = new Bitmap(copyBitmap.Width, copyBitmap.Height);
            max = 0;
            for (int y = 0; y < gradientsBM.Height; y++)
            {
                for (int x = 0; x < gradientsBM.Width; x++)
                {
                    if (candidates[y, x].EdgeState == CannyPixelEdgeState.PotentialEdge)
                    {
                        int manhattanDistance = candidates[y, x].Gradient.ManhattanDistance;
                        if (max < manhattanDistance)
                        {
                            max = manhattanDistance;
                        }
                        if (min > manhattanDistance)
                        {
                            min = manhattanDistance;
                        }
                    }
                }
            }

            for (int y = 0; y < gradientsBM.Height; y++)
            {
                for (int x = 0; x < gradientsBM.Width; x++)
                {
                    if (candidates[y, x].EdgeState == CannyPixelEdgeState.PotentialEdge)
                    {
                        int gray = Map(Math.Abs(candidates[y, x].Gradient.ManhattanDistance), min, max, 0, 255);
                        gradientsBM.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                    }
                    else
                    {
                        gradientsBM.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            cannyPhasesForm.NonMaximalSuppressionCanvas.Image = gradientsBM;

            ThresholdingHysterisis(candidates);
            cannyPhasesForm.ThresholdingHysterisisCanvas.Image = DrawBitMapWithEdges(copyBitmap, candidates);

            cannyPhasesForm.Text = $"{OpenImageFilename} Canny Edge Phases";
            cannyPhasesForm.PhaseTabsControl.SelectedIndex = cannyPhasesForm.PhaseTabsControl.TabCount - 1;
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
        
        private Bitmap SmoothImage(Bitmap original)
        {
            Bitmap smoothedBitmap = new Bitmap(original.Width, original.Height);
            var originalData = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, original.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(original.PixelFormat) / 8;

            int height = originalData.Height;
            int width = originalData.Width * bytesPerPixel;
            int stride = Math.Abs(originalData.Stride);

            byte[] transformationBytes = new byte[stride * height];
            Marshal.Copy(originalData.Scan0, transformationBytes, 0, transformationBytes.Length);
            original.UnlockBits(originalData);
            int rowm2 = 0;
            int rowm1 = stride;
            int row = 2 * stride;
            int rowp1 = 3 * stride;
            int rowp2 = 4 * stride;
            //skip first 2 rows and cols
            for (int y = 2; y < height - 2; y++)
            {
                int colm2 = 0;
                int colm1 = bytesPerPixel;
                int col = 2 * bytesPerPixel;
                int colp1 = 3 * bytesPerPixel;
                int colp2 = 4 * bytesPerPixel;
                for (int x = 2 * bytesPerPixel; x < width - 2 * bytesPerPixel; x += bytesPerPixel)
                {
                    int sum1 = transformationBytes[rowm2 + colm2] + transformationBytes[rowm2 + colp2] + transformationBytes[rowp2 + colm2] + transformationBytes[rowp2 + colp2];
                    int sum4 = transformationBytes[rowm2 + colm1] + transformationBytes[rowm2 + colp1] + transformationBytes[rowp2 + colm1] + transformationBytes[rowp2 + colp1];
                    sum4 += transformationBytes[rowm1 + colm2] + transformationBytes[rowm1 + colp2] + transformationBytes[rowp1 + colm2] + transformationBytes[rowp1 + colp2];
                    int sum7 = transformationBytes[rowm2 + col] + transformationBytes[row + colp2] + transformationBytes[rowp2 + col] + transformationBytes[row + colm2];
                    int sum16 = transformationBytes[rowm1 + colm1] + transformationBytes[rowm1 + colp1] + transformationBytes[rowp1 + colm1] + transformationBytes[rowp1 + colp1];
                    int sum26 = transformationBytes[rowm1 + col] + transformationBytes[row + colp1] + transformationBytes[rowp1 + col] + transformationBytes[row + colm1];

                    int multipliedValue = sum1 + (sum4 << 2) + sum7 * 7 + sum16 * 16 + sum26 * 26 + transformationBytes[row + col] * 41;
                    byte smoothedValue = (byte)((multipliedValue + 136) / 273);
                    transformationBytes[row + x] = smoothedValue;
                    transformationBytes[row + x + 1] = smoothedValue;
                    transformationBytes[row + x + 2] = smoothedValue;

                    colm2 += bytesPerPixel;
                    colm1 += bytesPerPixel;
                    col += bytesPerPixel;
                    colp1 += bytesPerPixel;
                    colp2 += bytesPerPixel;
                }
                rowm2 += stride;
                rowm1 += stride;
                row += stride;
                rowp1 += stride;
                rowp2 += stride;
            }
            var smoothedData = smoothedBitmap.LockBits(new Rectangle(0, 0, original.Width, original.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, original.PixelFormat);
            Marshal.Copy(transformationBytes, 0, smoothedData.Scan0, transformationBytes.Length);
            smoothedBitmap.UnlockBits(smoothedData);
            return smoothedBitmap;
        }
        
        private CannyEdgeCandidate[,] CalculateGradients(Bitmap original)
        {
            CannyEdgeCandidate[,] candidates = new CannyEdgeCandidate[original.Height, original.Width];
            var originalData = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, original.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(original.PixelFormat) / 8;

            int height = originalData.Height;
            int width = originalData.Width * bytesPerPixel;
            int stride = Math.Abs(originalData.Stride);

            byte[] transformationBytes = new byte[stride * height];
            Marshal.Copy(originalData.Scan0, transformationBytes, 0, transformationBytes.Length);
            original.UnlockBits(originalData);
            int y = 0;
            int prevY = -stride;
            int nextY = stride;
            int heightStride = height * stride;
            for (int row = 0; y < heightStride; prevY = y, y  = nextY, nextY += stride, row++)
            {
                int prevX = -bytesPerPixel;
                int x = 0;
                int nextX = bytesPerPixel;
                for (int col = 0; x < width; prevX = x, x =nextX, nextX += bytesPerPixel, col++)
                {
                    int dx, dy;
                    if(prevX >= 0 && nextX < width)
                    {
                        dx = (transformationBytes[y + nextX] - transformationBytes[y + prevX]) / 2;
                    }
                    else if(prevX >=0)
                    {
                        dx = transformationBytes[y + x] - transformationBytes[y + prevX];
                        //dx = 0;
                    }
                    else
                    {
                        dx = transformationBytes[y + nextX] - transformationBytes[y + x];
                        //dx = 0;
                    }

                    if (prevY >= 0 && nextY < heightStride)
                    {
                        dy = (transformationBytes[prevY + x] - transformationBytes[nextY + x]) / 2;
                    }
                    else if (prevY >= 0)
                    {
                        dy = transformationBytes[prevY + x] - transformationBytes[y + x];
                        //dy = 0;
                    }
                    else
                    {
                        dy = transformationBytes[y + x] - transformationBytes[nextY + x];
                        //dy = 0;
                    }
                    candidates[row, col] = new CannyEdgeCandidate(new PixelGradient(col, row, dx, dy));
                    candidates[row, col].x = col;
                    candidates[row, col].y = row;
                }
            }

            return candidates;
        }

        private void NonMaximalSuppression(CannyEdgeCandidate[,] candidates)
        {
            int totalCount = 0;
            int potentialEdgeCount = 0;
            for(int row = 1; row < candidates.GetLength(0)-1; row++)
            {
                for (int col = 1; col < candidates.GetLength(1)-1; col++)
                {
                    CannyEdgeCandidate candidate = candidates[row, col];
                    candidate.EdgeState = CannyPixelEdgeState.NotAnEdge;
                    int rightMagnitude = -1;
                    int leftMagnitude = -1;
                    int absDx = Math.Abs(candidate.Gradient.Dx);
                    int absDy = Math.Abs(candidate.Gradient.Dy);
                    int axisPointColOffset = Math.Max(0, Math.Min(1, absDx - absDy));
                    int axisPointRowOffset = Math.Max(0, Math.Min(1, absDy - absDx));
                    if (absDx < 2 && absDy < 2)
                    {
                        //continue;
                    }
                    if (candidate.Gradient.Dx * candidate.Gradient.Dy >= 0)
                    {
                        if (axisPointColOffset == 0 && axisPointRowOffset == 0 && candidate.Gradient.Dx == 0 && candidate.Gradient.Dy == 0)
                        {
                            continue;
                        }
                        else if (axisPointColOffset == 0 && axisPointRowOffset == 0 && candidate.Gradient.Dx == 0)
                        {
                            rightMagnitude = candidates[row + 1, col].Gradient.ManhattanDistance;
                            leftMagnitude = candidates[row - 1, col].Gradient.ManhattanDistance;
                        }
                        else if (axisPointColOffset == 0 && axisPointRowOffset == 0 && candidate.Gradient.Dy == 0)
                        {
                            rightMagnitude = candidates[row, col + 1].Gradient.ManhattanDistance;
                            leftMagnitude = candidates[row, col - 1].Gradient.ManhattanDistance;
                        }
                        else if (axisPointColOffset == 0 && axisPointRowOffset == 0)
                        {
                            rightMagnitude = candidates[row - 1, col + 1].Gradient.ManhattanDistance;
                            leftMagnitude = candidates[row + 1, col - 1].Gradient.ManhattanDistance;
                        }
                        else
                        {
                            rightMagnitude = candidates[row - 1, col + 1].Gradient.ManhattanDistance + candidates[row - axisPointRowOffset, col + axisPointColOffset].Gradient.ManhattanDistance;
                            rightMagnitude = (rightMagnitude + 1) / 2;
                            leftMagnitude = candidates[row + 1, col - 1].Gradient.ManhattanDistance + candidates[row + axisPointRowOffset, col - axisPointColOffset].Gradient.ManhattanDistance;
                            leftMagnitude = (leftMagnitude + 1) / 2;
                        }
                    }
                    else
                    {
                        if (axisPointColOffset == 0 && axisPointRowOffset == 0)
                        {
                            rightMagnitude = candidates[row + 1, col + 1].Gradient.ManhattanDistance;
                            leftMagnitude = candidates[row - 1, col - 1].Gradient.ManhattanDistance;
                        }
                        else
                        {
                            rightMagnitude = candidates[row + 1, col + 1].Gradient.ManhattanDistance + candidates[row + axisPointRowOffset, col + axisPointColOffset].Gradient.ManhattanDistance;
                            rightMagnitude = (rightMagnitude + 1) / 2;
                            leftMagnitude = candidates[row - 1, col - 1].Gradient.ManhattanDistance + candidates[row - axisPointRowOffset, col - axisPointColOffset].Gradient.ManhattanDistance;
                            leftMagnitude = (leftMagnitude + 1) / 2;
                        }
                    }

                    totalCount++;
                    if(candidate.Gradient.ManhattanDistance > rightMagnitude && candidate.Gradient.ManhattanDistance > leftMagnitude)
                    {
                        potentialEdgeCount++;
                        candidate.EdgeState = CannyPixelEdgeState.PotentialEdge;
                    }
                }
            }
            float PotentialEdgePercentage = potentialEdgeCount * 100 / (float)totalCount;
        }
        
        private void ThresholdingHysterisis(CannyEdgeCandidate[,] candidates)
        {
            List<CannyEdgeCandidate> sortedEdgeCandidates = new List<CannyEdgeCandidate>();
            for(int r = 0; r < candidates.GetLength(0); r++)
            {
                for(int c = 0; c < candidates.GetLength(1); c++)
                {
                    if (candidates[r, c].EdgeState == CannyPixelEdgeState.PotentialEdge)
                    {
                        sortedEdgeCandidates.Add(candidates[r, c]);
                    }
                }
            }

            sortedEdgeCandidates.Sort((g1, g2) => g2.Gradient.ManhattanDistance.CompareTo(g1.Gradient.ManhattanDistance));
            int totalPossibleDefiniteEdgeCandidates = sortedEdgeCandidates.Count / 20;
            int rangeOfDefiniteEdgeCandidates = (int)(sortedEdgeCandidates.Count * .12);
            for (int i = sortedEdgeCandidates.Count - totalPossibleDefiniteEdgeCandidates + 1; i < sortedEdgeCandidates.Count; i++)
            {
                candidates[sortedEdgeCandidates[i].y, sortedEdgeCandidates[i].x].EdgeState = CannyPixelEdgeState.NotAnEdge;
            }
            sortedEdgeCandidates = sortedEdgeCandidates.Take(sortedEdgeCandidates.Count - totalPossibleDefiniteEdgeCandidates).ToList();

            Queue<CannyEdgeCandidate> bfsPixels = new Queue<CannyEdgeCandidate>();
            CannyEdgeCandidate[,] propagationCandidates = new CannyEdgeCandidate[candidates.GetLength(0), candidates.GetLength(1)];
            for(int r = 0; r < propagationCandidates.GetLength(0); r++)
            {
                for(int c = 0; c < propagationCandidates.GetLength(1); c++)
                {
                    propagationCandidates[r, c] = new CannyEdgeCandidate(candidates[r, c].Gradient);
                    propagationCandidates[r, c].EdgeState = candidates[r, c].EdgeState;
                }
            }
            int choosenEdgeCandidates = 0;
            for (int i = 0; i < rangeOfDefiniteEdgeCandidates && choosenEdgeCandidates < totalPossibleDefiniteEdgeCandidates; i++)
            {
                CannyEdgeCandidate candidate = candidates[sortedEdgeCandidates[i].y, sortedEdgeCandidates[i].x];
                if (candidate.EdgeState == CannyPixelEdgeState.PotentialEdge)
                {
                    candidate.EdgeState = CannyPixelEdgeState.IsAnEdge;
                    propagationCandidates[candidate.y, candidate.x].EdgeState = CannyPixelEdgeState.IsAnEdge;
                    //BFS
                    bfsPixels.Enqueue(candidate);
                    while (bfsPixels.Count > 0)
                    {
                        CannyEdgeCandidate knownEdge = bfsPixels.Dequeue();
                        for (int y = Math.Max(0, knownEdge.y - 1); y < Math.Min(candidates.GetLength(0), knownEdge.y + 2); y++)
                        {
                            for (int x = Math.Max(0, knownEdge.x - 1); x < Math.Min(candidates.GetLength(1), knownEdge.x + 2); x++)
                            {
                                if (candidates[y, x].EdgeState == CannyPixelEdgeState.PotentialEdge)
                                {
                                    candidates[y, x].EdgeState = CannyPixelEdgeState.IsAnEdge;
                                    bfsPixels.Enqueue(candidates[y, x]);
                                }
                            }
                        }
                    }
                }
            }
            //CannyEdgePhaseForm initialChoosenEdges = new CannyEdgePhaseForm();
            //initialChoosenEdges.Canvas.Image = DrawBitMapWithEdges(new Bitmap(ImagePictureBox.Image), propagationCandidates);
            //initialChoosenEdges.Text = "Initial Choosen Edges";
            //initialChoosenEdges.Show();
        }
        
        private Bitmap DrawBitMapWithEdges(Bitmap original, CannyEdgeCandidate[,] candidates)
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
                for (int x = 0, candidateX = 0; x < width; x += bytesPerPixel, candidateX++)
                {
                    if(candidates[y, candidateX].EdgeState == CannyPixelEdgeState.IsAnEdge)
                    {
                        transformationBytes[row + x] = 255;
                        transformationBytes[row + x + 1] = 255;
                        transformationBytes[row + x + 2] = 255;
                    }
                    else
                    {
                        transformationBytes[row + x] = 0;
                        transformationBytes[row + x + 1] = 0;
                        transformationBytes[row + x + 2] = 0;
                    }
                }
            }

            var alteredData = copyBitmap.LockBits(new Rectangle(0, 0, original.Width, original.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, original.PixelFormat);
            Marshal.Copy(transformationBytes, 0, alteredData.Scan0, transformationBytes.Length);
            copyBitmap.UnlockBits(alteredData);
            return copyBitmap;
        }
    }

    public enum CannyPixelEdgeState
    {
        NotAnEdge = 0,
        PotentialEdge = 1,
        IsAnEdge = 2,
    }
    public class CannyEdgeCandidate
    {
        public PixelGradient Gradient;
        public CannyPixelEdgeState EdgeState;
        public int x;
        public int y;
        public CannyEdgeCandidate(PixelGradient pixelGradient)
        {
            Gradient = pixelGradient;
        }
    }
    public struct PixelGradient
    {
        public int Dx;
        public int Dy;
        public int x;
        public int y;
        public int ManhattanDistance;

        public PixelGradient(int x, int y, int dx, int dy)
        {
            this.x = x;
            this.y = y;
            Dx = dx;
            Dy = dy;
            ManhattanDistance = Math.Abs(dx) + Math.Abs(dy);
        }

    }
}
