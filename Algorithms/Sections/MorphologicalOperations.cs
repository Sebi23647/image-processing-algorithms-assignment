using Emgu.CV.Structure;
using Emgu.CV;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;
using Emgu.CV.CvEnum;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {

        #region Connected Components using Disjoint Sets

        public static Image<Bgr, byte> ConnectedComponents(Image<Gray, byte> inputImage)
        {
            int rows = inputImage.Height;
            int cols = inputImage.Width;

            int[,] labels = new int[rows, cols];
            Dictionary<int, int> parent = new Dictionary<int, int>();
            int nextLabel = 1;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (inputImage.Data[y, x, 0] != 0)  
                    {
                        var neighbors = GetNeighborLabels(labels, x, y);

                        if (neighbors.Count == 0)
                        {
                            labels[y, x] = nextLabel;
                            parent[nextLabel] = nextLabel;
                            nextLabel++;
                        }
                        else
                        {
                            int smallestLabel = neighbors.Min();
                            labels[y, x] = smallestLabel;

                            foreach (var neighborLabel in neighbors)
                            {
                                Union(parent, smallestLabel, neighborLabel);
                            }
                        }
                    }
                }
            }

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (labels[y, x] != 0)
                    {
                        labels[y, x] = Find(parent, labels[y, x]);
                    }
                }
            }

            Dictionary<int, int> area = new Dictionary<int, int>();
            Dictionary<int, Rectangle> boundingBoxes = new Dictionary<int, Rectangle>();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int label = labels[y, x];
                    if (label != 0)
                    {
                        if (!area.ContainsKey(label))
                        {
                            area[label] = 0;
                            boundingBoxes[label] = new Rectangle(x, y, 1, 1);
                        }

                        area[label]++;

                        Rectangle box = boundingBoxes[label];
                        int minX = Math.Min(box.X, x);
                        int minY = Math.Min(box.Y, y);
                        int maxX = Math.Max(box.Right - 1, x);
                        int maxY = Math.Max(box.Bottom - 1, y);
                        boundingBoxes[label] = Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
                    }
                }
            }

            int largestLabel = area.OrderByDescending(kv => kv.Value).First().Key;
            Rectangle largestBox = boundingBoxes[largestLabel];

            Dictionary<int, Bgr> colors = new Dictionary<int, Bgr>();
            Random random = new Random();
            Image<Bgr, byte> result = new Image<Bgr, byte>(cols, rows);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int label = labels[y, x];
                    if (label != 0)
                    {
                        if (!colors.ContainsKey(label))
                        {
                            colors[label] = new Bgr(random.Next(256), random.Next(256), random.Next(256));
                        }

                        Bgr color = colors[label];
                        result.Data[y, x, 0] = (byte)color.Blue;
                        result.Data[y, x, 1] = (byte)color.Green;
                        result.Data[y, x, 2] = (byte)color.Red;
                    }
                }
            }

             
            CvInvoke.Rectangle(result, largestBox, new MCvScalar(0, 0, 255), 2);

            return result;
        }

        private static List<int> GetNeighborLabels(int[,] labels, int x, int y)
        {
            List<int> neighbors = new List<int>();

            if (x > 0 && labels[y, x - 1] > 0) neighbors.Add(labels[y, x - 1]);  
            if (y > 0 && labels[y - 1, x] > 0) neighbors.Add(labels[y - 1, x]);  
            if (x > 0 && y > 0 && labels[y - 1, x - 1] > 0) neighbors.Add(labels[y - 1, x - 1]);  
            if (x < labels.GetLength(1) - 1 && y > 0 && labels[y - 1, x + 1] > 0) neighbors.Add(labels[y - 1, x + 1]);   

            return neighbors;
        }

        private static int Find(Dictionary<int, int> parent, int label)
        {
            if (parent[label] != label)
            {
                parent[label] = Find(parent, parent[label]); 
            }
            return parent[label];
        }

        private static void Union(Dictionary<int, int> parent, int label1, int label2)
        {
            int root1 = Find(parent, label1);
            int root2 = Find(parent, label2);

            if (root1 != root2)
            {
                parent[root2] = root1;
            }
        }

        #endregion



    }
}