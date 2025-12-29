using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion


        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, int lowerThreshold, int upperThreshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte pixelValue = inputImage.Data[y, x, 0];
                    if (pixelValue <= lowerThreshold)
                    {
                        result.Data[y, x, 0] = 0; // Negru
                    }
                    else if (pixelValue >= upperThreshold)
                    {
                        result.Data[y, x, 0] = 255; // Alb
                    }
                    else
                    {
                        result.Data[y, x, 0] = 128; // Gri
                    }
                }
            }
            return result;
        }
        #endregion



        #region Crop
        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage, Rectangle rect)
        {
            return inputImage.GetSubRect(rect);
        }

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, Rectangle rect)
        {
            return inputImage.GetSubRect(rect);
        }

        public static double CalculateMean(Image<Gray, byte> croppedImage)
        {
            double sum = 0;
            for (int y = 0; y < croppedImage.Height; ++y)
            {
                for (int x = 0; x < croppedImage.Width; ++x)
                {
                    sum += croppedImage.Data[y, x, 0];
                }
            }
            return sum / (croppedImage.Width * croppedImage.Height);
        }

        public static double CalculateStandardDeviation(Image<Gray, byte> croppedImage, double mean)
        {
            double sum = 0;
            for (int y = 0; y < croppedImage.Height; ++y)
            {
                for (int x = 0; x < croppedImage.Width; ++x)
                {
                    sum += Math.Pow(croppedImage.Data[y, x, 0] - mean, 2);
                }
            }
            return Math.Sqrt(sum / (croppedImage.Width * croppedImage.Height));
        }

        public static (double mean, double stdDev) CalculateMeanAndStdDev(Image<Bgr, byte> croppedImage, int channel)
        {
            double sum = 0;
            for (int y = 0; y < croppedImage.Height; ++y)
            {
                for (int x = 0; x < croppedImage.Width; ++x)
                {
                    sum += croppedImage.Data[y, x, channel];
                }
            }
            double mean = sum / (croppedImage.Width * croppedImage.Height);

            sum = 0;
            for (int y = 0; y < croppedImage.Height; ++y)
            {
                for (int x = 0; x < croppedImage.Width; ++x)
                {
                    sum += Math.Pow(croppedImage.Data[y, x, channel] - mean, 2);
                }
            }
            double stdDev = Math.Sqrt(sum / (croppedImage.Width * croppedImage.Height));

            return (mean, stdDev);
        }
        #endregion


        #region Mirror
        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, inputImage.Width - x - 1, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, inputImage.Width - x - 1, 0] = inputImage.Data[y, x, 0];
                    result.Data[y, inputImage.Width - x - 1, 1] = inputImage.Data[y, x, 1];
                    result.Data[y, inputImage.Width - x - 1, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }
        #endregion
        //(x, y) la noua poziție(W - x - 1, y).

        #region Rotate
        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(new Size(inputImage.Height, inputImage.Width));

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - y - 1, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(new Size(inputImage.Height, inputImage.Width));

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - y - 1, 0] = inputImage.Data[y, x, 0];
                    result.Data[x, inputImage.Height - y - 1, 1] = inputImage.Data[y, x, 1];
                    result.Data[x, inputImage.Height - y - 1, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }

        public static Image<Gray, byte> RotateAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(new Size(inputImage.Height, inputImage.Width));

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - x - 1, y, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(new Size(inputImage.Height, inputImage.Width));

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - x - 1, y, 0] = inputImage.Data[y, x, 0];
                    result.Data[inputImage.Width - x - 1, y, 1] = inputImage.Data[y, x, 1];
                    result.Data[inputImage.Width - x - 1, y, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }
        #endregion
    }
}
