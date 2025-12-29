using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;


namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        #region

        
        public static Image<Gray, byte> ApplyLUT(Image<Gray, byte> inputImage, double[] lut)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte pixelValue = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = (byte)Math.Min(255, Math.Max(0, lut[pixelValue]));
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ApplyLUT(Image<Bgr, byte> inputImage, double[] lut)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int c = 0; c < 3; ++c)
                    {
                        byte pixelValue = inputImage.Data[y, x, c];
                        result.Data[y, x, c] = (byte)Math.Min(255, Math.Max(0, lut[pixelValue]));
                    }
                }
            }

            return result;
        }
        #endregion
    }
}

