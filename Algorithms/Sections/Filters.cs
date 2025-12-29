using Emgu.CV.Structure;
using Emgu.CV;
using System;
using Emgu.CV.Util;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<Gray, byte> MedianFilter(Image<Gray, byte> inputImage, int maskSize)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;
            Image<Gray, byte> resultImage = new Image<Gray, byte>(width, height);
            int radius = maskSize / 2;

            if (maskSize == 1)
            {
                return inputImage.Copy();
            }

            int[][] columnHistograms = new int[width][];
            for (int x = 0; x < width; x++)
            {
                columnHistograms[x] = new int[256];
                for (int y = 0; y <= radius && y < height; y++)
                {
                    byte pixel = inputImage.Data[y, x, 0];
                    columnHistograms[x][pixel]++;
                }
            }
            //slide vertical
            for (int i = 0; i < height; i++)
            {
                if (i > 0)
                {
                    int oldRow = i - radius - 1;   
                    int newRow = i + radius;         
                    for (int x = 0; x < width; x++)
                    {
                        if (oldRow >= 0)
                        {
                            byte oldPixel = inputImage.Data[oldRow, x, 0];
                            columnHistograms[x][oldPixel]--;
                        }
                        if (newRow < height)
                        {
                            byte newPixel = inputImage.Data[newRow, x, 0];
                            columnHistograms[x][newPixel]++;
                        }
                    }
                }

                 
                int[] kernelHistogram = new int[256];
                for (int x = 0; x <= radius && x < width; x++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        kernelHistogram[k] += columnHistograms[x][k];
                    }
                }
                //slide orizontal
                for (int j = 0; j < width; j++)
                {
                    if (j > 0)
                    {
                        int leftCol = j - radius - 1;
                        int rightCol = j + radius;
                        if (leftCol >= 0)
                        {
                            for (int k = 0; k < 256; k++)
                            {
                                kernelHistogram[k] -= columnHistograms[leftCol][k];
                            }
                        }
                        if (rightCol < width)
                        {
                            for (int k = 0; k < 256; k++)
                            {
                                kernelHistogram[k] += columnHistograms[rightCol][k];
                            }
                        }
                    }

                    int totalPixels = maskSize * maskSize;  
                    int median = FindMedian(kernelHistogram, totalPixels);
                    resultImage.Data[i, j, 0] = (byte)median;
                }
            }

            return resultImage;
        }

        private static int FindMedian(int[] histogram, int totalPixels)
        {
            int medianPos = totalPixels / 2;
            int count = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                count += histogram[i];
                if (count > medianPos)
                    return i;
            }
            return 0;
        }


        public static Image<Bgr, byte> MedianFilter(Image<Bgr, byte> inputImage, int maskSize)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;
            Image<Bgr, byte> resultImage = new Image<Bgr, byte>(width, height);
            int radius = maskSize / 2;

            
            if (maskSize == 1)
            {
                return inputImage.Copy();
            }

            
            int[][][] columnHistograms = new int[width][][];
            for (int x = 0; x < width; x++)
            {
                columnHistograms[x] = new int[3][];
                for (int c = 0; c < 3; c++)
                {
                    columnHistograms[x][c] = new int[256];
                    for (int y = 0; y <= radius && y < height; y++)
                    {
                        byte pixel = inputImage.Data[y, x, c];
                        columnHistograms[x][c][pixel]++;
                    }
                }
            }

            for (int i = 0; i < height; i++)
            {
                if (i > 0)
                {
                    int oldRow = i - radius - 1;
                    int newRow = i + radius;
                    for (int x = 0; x < width; x++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            if (oldRow >= 0)
                            {
                                byte oldPixel = inputImage.Data[oldRow, x, c];
                                columnHistograms[x][c][oldPixel]--;
                            }
                            if (newRow < height)
                            {
                                byte newPixel = inputImage.Data[newRow, x, c];
                                columnHistograms[x][c][newPixel]++;
                            }
                        }
                    }
                }

                int[][] kernelHistograms = new int[3][];
                for (int c = 0; c < 3; c++)
                {
                    kernelHistograms[c] = new int[256];
                    for (int x = 0; x <= radius && x < width; x++)
                    {
                        for (int k = 0; k < 256; k++)
                        {
                            kernelHistograms[c][k] += columnHistograms[x][c][k];
                        }
                    }
                }

                for (int j = 0; j < width; j++)
                {
                    if (j > 0)
                    {
                        int leftCol = j - radius - 1;
                        int rightCol = j + radius;
                        for (int c = 0; c < 3; c++)
                        {
                            if (leftCol >= 0)
                            {
                                for (int k = 0; k < 256; k++)
                                {
                                    kernelHistograms[c][k] -= columnHistograms[leftCol][c][k];
                                }
                            }
                            if (rightCol < width)
                            {
                                for (int k = 0; k < 256; k++)
                                {
                                    kernelHistograms[c][k] += columnHistograms[rightCol][c][k];
                                }
                            }
                        }
                    }

                    int totalPixels = maskSize * maskSize;  
                    for (int c = 0; c < 3; c++)
                    {
                        int median = FindMedian(kernelHistograms[c], totalPixels);
                        resultImage.Data[i, j, c] = (byte)median;
                    }
                }
            }

            return resultImage;
        }

        #region LoG
        public static Image<Gray, byte> LoG(Image<Gray, byte> inputImage, double threshold)
        {
            Image<Gray, byte> blurredImage = inputImage.SmoothGaussian(5, 5, 1.0, 1.0);

            //evidentiza zonele cu schimbari bruste de intensitate
            Image<Gray, float> laplacianImage = blurredImage.Laplace(3);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 1; y < laplacianImage.Height - 1; y++)
            {
                for (int x = 1; x < laplacianImage.Width - 1; x++)
                {
                    float current = laplacianImage.Data[y, x, 0];
                    bool isZeroCrossing =
                        (current > 0 && (laplacianImage.Data[y - 1, x, 0] < 0 || laplacianImage.Data[y + 1, x, 0] < 0 || laplacianImage.Data[y, x - 1, 0] < 0 || laplacianImage.Data[y, x + 1, 0] < 0)) ||
                        (current < 0 && (laplacianImage.Data[y - 1, x, 0] > 0 || laplacianImage.Data[y + 1, x, 0] > 0 || laplacianImage.Data[y, x - 1, 0] > 0 || laplacianImage.Data[y, x + 1, 0] > 0));
                    result.Data[y, x, 0] = (byte)(isZeroCrossing && Math.Abs(current) >= threshold ? 255 : 0);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> LoGColor(Image<Bgr, byte> inputImage, double threshold)
        {
            Image<Gray, byte>[] channels = inputImage.Split();

            Image<Gray, byte> blueChannel = LoG(channels[0], threshold);
            Image<Gray, byte> greenChannel = LoG(channels[1], threshold);
            Image<Gray, byte> redChannel = LoG(channels[2], threshold);

            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            CvInvoke.Merge(new VectorOfMat(blueChannel.Mat, greenChannel.Mat, redChannel.Mat), result);

            return result;
        }


        #endregion


    }
}
