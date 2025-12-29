using Emgu.CV.Structure;
using Emgu.CV;
using System;

namespace Algorithms.Sections
{
    public class GeometricTransformations
    {

        #region Spherical deformation

        public static Image<Gray, byte> SphericalDeformation(Image<Gray, byte> input, double rho, double rMax)
        {
            int width = input.Width;
            int height = input.Height;
            var output = new Image<Gray, byte>(width, height);

            double x0 = width / 2.0;
            double y0 = height / 2.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double dx = x - x0;
                    double dy = y - y0;
                    double r = Math.Sqrt(dx * dx + dy * dy);

                    if (r <= rMax)
                    {
                        //z=adancimea supraffetei sferice
                        double z = Math.Sqrt(rMax * rMax - r * r);

                        double denomX = Math.Sqrt(dx * dx + z * z);
                        double denomY = Math.Sqrt(dy * dy + z * z);
                        //unghiuri de refractie
                        double betaX = (1 - 1.0 / rho) * Math.Asin(dx / denomX);
                        double betaY = (1 - 1.0 / rho) * Math.Asin(dy / denomY);

                        double srcX = x - z * Math.Tan(betaX);
                        double srcY = y - z * Math.Tan(betaY);

                        if (srcX >= 0 && srcX + 1 < width && srcY >= 0 && srcY + 1 < height)
                        {
                            int x1 = (int)Math.Floor(srcX);
                            int x2 = x1 + 1;
                            int y1 = (int)Math.Floor(srcY);
                            int y2 = y1 + 1;

                            double dx1 = srcX - x1;
                            double dy1 = srcY - y1;

                            byte q11 = input.Data[y1, x1, 0];
                            byte q21 = input.Data[y1, x2, 0];
                            byte q12 = input.Data[y2, x1, 0];
                            byte q22 = input.Data[y2, x2, 0];

                            double interpolatedValue = (1 - dx1) * (1 - dy1) * q11 +
                                                       dx1 * (1 - dy1) * q21 +
                                                       (1 - dx1) * dy1 * q12 +
                                                       dx1 * dy1 * q22;

                            output.Data[y, x, 0] = (byte)Math.Round(interpolatedValue);
                        }
                    }
                    else
                    {
                        output.Data[y, x, 0] = input.Data[y, x, 0];
                    }
                }
            }

            return output;
        }

        public static Image<Bgr, byte> SphericalDeformation(Image<Bgr, byte> input, double rho, double rMax)
        {
            int width = input.Width;
            int height = input.Height;
            var output = new Image<Bgr, byte>(width, height);

            double x0 = width / 2.0;
            double y0 = height / 2.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double dx = x - x0;
                    double dy = y - y0;
                    double r = Math.Sqrt(dx * dx + dy * dy);

                    if (r <= rMax)
                    {
                        double z = Math.Sqrt(rMax * rMax - r * r);
                        double denomX = Math.Sqrt(dx * dx + z * z);
                        double denomY = Math.Sqrt(dy * dy + z * z);
                        double betaX = (1 - 1.0 / rho) * Math.Asin(dx / denomX);
                        double betaY = (1 - 1.0 / rho) * Math.Asin(dy / denomY);

                        double srcX = x - z * Math.Tan(betaX);
                        double srcY = y - z * Math.Tan(betaY);

                        if (srcX >= 0 && srcX + 1 < width && srcY >= 0 && srcY + 1 < height)
                        {
                            int x1 = (int)Math.Floor(srcX);
                            int x2 = x1 + 1;
                            int y1 = (int)Math.Floor(srcY);
                            int y2 = y1 + 1;

                            double dx1 = srcX - x1;
                            double dy1 = srcY - y1;

                            for (int channel = 0; channel < 3; channel++)
                            {
                                byte q11 = input.Data[y1, x1, channel];
                                byte q21 = input.Data[y1, x2, channel];
                                byte q12 = input.Data[y2, x1, channel];
                                byte q22 = input.Data[y2, x2, channel];

                                double interpolatedValue = (1 - dx1) * (1 - dy1) * q11 +
                                                           dx1 * (1 - dy1) * q21 +
                                                           (1 - dx1) * dy1 * q12 +
                                                           dx1 * dy1 * q22;

                                output.Data[y, x, channel] = (byte)Math.Round(interpolatedValue);
                            }
                        }
                    }
                    else
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            output.Data[y, x, channel] = input.Data[y, x, channel];
                        }
                    }
                }
            }

            return output;
        }
        #endregion
    }
}