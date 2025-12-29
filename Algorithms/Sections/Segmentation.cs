using Emgu.CV.Structure;
using Emgu.CV;
using System.Collections.Generic;
using Emgu.CV.CvEnum;
using System;

namespace Algorithms.Sections
{
    public class Segmentation
    {
        #region K-Means Clustering
        public static Image<Gray, byte> ApplyKMeansClustering(Image<Gray, byte> inputImage, int k)
        {
            var data = new List<float[]>();
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte intensity = inputImage.Data[y, x, 0];
                    data.Add(new float[] { intensity });
                }
            }

            Matrix<float> points = new Matrix<float>(data.Count, 1, 1);
            for (int i = 0; i < data.Count; i++)
            {
                points[i, 0] = data[i][0]; 
            }

            Mat labels = new Mat();
            Mat centers = new Mat();

            CvInvoke.Kmeans(points, k, labels, new MCvTermCriteria(50, 0.1), 3, KMeansInitType.PPCenters, centers);

            int[] labelData = new int[labels.Rows];
            labels.CopyTo(labelData);

            float[] centerData = new float[centers.Rows * centers.Cols];
            centers.CopyTo(centerData);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int label = labelData[y * inputImage.Width + x];
                    byte intensity = (byte)centerData[label]; 
                    result.Data[y, x, 0] = intensity;
                }
            }

            return result;
        }
        #endregion
    }
}