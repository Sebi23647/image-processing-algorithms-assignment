using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte pixelValue = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = (byte)(pixelValue > threshold ? 255 : 0);
                }
            }
            return result;
        }
        #endregion
        #region Preprocess
        public static Image<Gray, byte> ApplyGaussianBlur(Image<Gray, byte> inputImage, int kernelSize)
        {
            return inputImage.SmoothGaussian(kernelSize);
        }

        public static Image<Gray, byte> ApplyMorphologicalOperations(Image<Gray, byte> inputImage, int iterations)
        {
            var element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            var result = new Image<Gray, byte>(inputImage.Size);
            CvInvoke.MorphologyEx(inputImage, result, Emgu.CV.CvEnum.MorphOp.Open, element, new Point(-1, -1), iterations, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(1));
            return result;
        }
        #endregion
    }


}