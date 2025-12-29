using Emgu.CV;
using Emgu.CV.Structure;

using System.Windows;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using Framework.View;
using static Framework.Utilities.DataProvider;
using static Framework.Utilities.DrawingHelper;
using static Framework.Utilities.FileHelper;
using static Framework.Converters.ImageConverter;

using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;
using Framework.Utilities;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace Framework.ViewModel
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;

        public MenuCommands(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private ImageSource InitialImage
        {
            get => _mainVM.InitialImage;
            set => _mainVM.InitialImage = value;
        }

        private ImageSource ProcessedImage
        {
            get => _mainVM.ProcessedImage;
            set => _mainVM.ProcessedImage = value;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayscaleImageCommand;
        public RelayCommand LoadGrayscaleImageCommand
        {
            get
            {
                if (_loadGrayscaleImageCommand == null)
                    _loadGrayscaleImageCommand = new RelayCommand(LoadGrayscaleImage);
                return _loadGrayscaleImageCommand;
            }
        }

        private void LoadGrayscaleImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a grayscale picture");
            if (fileName != null)
            {
                GrayInitialImage = new Image<Gray, byte>(fileName);
                InitialImage = Convert(GrayInitialImage);
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a color picture");
            if (fileName != null)
            {
                ColorInitialImage = new Image<Bgr, byte>(fileName);
                InitialImage = Convert(ColorInitialImage);
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                GrayProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                ColorProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                Process.Start(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes from initial canvas
        private ICommand _removeInitialDrawnShapesCommand;
        public ICommand RemoveInitialDrawnShapesCommand
        {
            get
            {
                if (_removeInitialDrawnShapesCommand == null)
                    _removeInitialDrawnShapesCommand = new RelayCommand(RemoveInitialDrawnShapes);
                return _removeInitialDrawnShapesCommand;
            }
        }

        private void RemoveInitialDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from processed canvas
        private ICommand _removeProcessedDrawnShapesCommand;
        public ICommand RemoveProcessedDrawnShapesCommand
        {
            get
            {
                if (_removeProcessedDrawnShapesCommand == null)
                    _removeProcessedDrawnShapesCommand = new RelayCommand(RemoveProcessedDrawnShapes);
                return _removeProcessedDrawnShapesCommand;
            }
        }

        private void RemoveProcessedDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from both canvases
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            var canvases = (object[])parameter;
            RemoveUiElements(canvases[0] as Canvas);
            RemoveUiElements(canvases[1] as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != Application.Current.MainWindow)
                {
                    window.Close();
                }
            }

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0] as Canvas);
            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (MagnifierOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
        }
        #endregion

        #region Visualize color levels

        #region Row color levels
        private ICommand _rowColorLevelsCommand;
        public ICommand RowColorLevelsCommand
        {
            get
            {
                if (_rowColorLevelsCommand == null)
                    _rowColorLevelsCommand = new RelayCommand(RowColorLevels);
                return _rowColorLevelsCommand;
            }
        }

        private void RowColorLevels(object parameter)
        {
            if (RowColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Row);
            window.Show();
        }
        #endregion

        #region Column color levels
        private ICommand _columnColorLevelsCommand;
        public ICommand ColumnColorLevelsCommand
        {
            get
            {
                if (_columnColorLevelsCommand == null)
                    _columnColorLevelsCommand = new RelayCommand(ColumnColorLevels);
                return _columnColorLevelsCommand;
            }
        }

        private void ColumnColorLevels(object parameter)
        {
            if (ColumnColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Column);
            window.Show();
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (InitialHistogramOn == true) return;
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            HistogramWindow window = null;

            if (ColorInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialColor);
            }
            else if (GrayInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialGray);
            }

            window.Show();
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (ProcessedHistogramOn == true) return;
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            HistogramWindow window = null;

            if (ColorProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedColor);
            }
            else if (GrayProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedGray);
            }

            window.Show();
        }
        #endregion

        #endregion

        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("It is possible to convert only color images!");
            }
        }
        #endregion



        #region Binary image
        private ICommand _binaryImageCommand;
        public ICommand BinaryImageCommand
        {
            get
            {
                if (_binaryImageCommand == null)
                    _binaryImageCommand = new RelayCommand(BinaryImage);
                return _binaryImageCommand;
            }
        }
        private void BinaryImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> labels = new List<string>
            {
                "Lower threshold value (10 - 245):",
                "Upper threshold value (10 - 245):"
            };

            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            if (values.Count < 2 || values[0] < 10 || values[0] > 245 || values[1] < 10 || values[1] > 245)
            {
                MessageBox.Show("Invalid threshold values. Please enter values between 10 and 245.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int lowerThreshold = (int)values[0];
            int upperThreshold = (int)values[1];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Binary(GrayInitialImage, lowerThreshold, upperThreshold);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("Binary transformation can only be applied to grayscale images!");
            }
        }
        #endregion

        #region Crop image
        private ICommand _cropImageCommand;
        public ICommand CropImageCommand
        {
            get
            {
                if (_cropImageCommand == null)
                    _cropImageCommand = new RelayCommand(CropImage);
                return _cropImageCommand;
            }
        }

        private void CropImage(object parameter)
        {
            try
            {
                if (GrayInitialImage == null && ColorInitialImage == null)
                {
                    MessageBox.Show("Please add an image!");
                    return;
                }

                if (MouseClickCollection == null || MouseClickCollection.Count < 2)
                {
                    MessageBox.Show("Please select a region by clicking at least twice on the image.");
                    return;
                }

                var canvases = (object[])parameter;
                var initialCanvas = canvases[0] as Canvas;
                var processedCanvas = canvases[1] as Canvas;

                ClearProcessedCanvas(initialCanvas);

                Rectangle cropRect = GetRectangleFromMouseClicks();

                if (GrayInitialImage != null)
                {
                    if (IsValidRectangle(cropRect, GrayInitialImage.Width, GrayInitialImage.Height))
                    {
                        GrayProcessedImage = Tools.Crop(GrayInitialImage, cropRect);
                        ProcessedImage = Convert(GrayProcessedImage);

                        var mean = Tools.CalculateMean(GrayProcessedImage);
                        var stdDev = Tools.CalculateStandardDeviation(GrayProcessedImage, mean);

                        MessageBox.Show($"Grayscale: Mean = {mean:F2}, StdDev = {stdDev:F2}", "Grayscale Statistics", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Invalid crop rectangle. Please ensure it is within the image bounds.");
                    }
                }
                else if (ColorInitialImage != null)
                {
                    if (IsValidRectangle(cropRect, ColorInitialImage.Width, ColorInitialImage.Height))
                    {
                        ColorProcessedImage = Tools.Crop(ColorInitialImage, cropRect);
                        ProcessedImage = Convert(ColorProcessedImage);

                        var (meanB, stdDevB) = Tools.CalculateMeanAndStdDev(ColorProcessedImage, 0);
                        var (meanG, stdDevG) = Tools.CalculateMeanAndStdDev(ColorProcessedImage, 1);
                        var (meanR, stdDevR) = Tools.CalculateMeanAndStdDev(ColorProcessedImage, 2);

                        MessageBox.Show($"Blue: Mean = {meanB:F2}, StdDev = {stdDevB:F2}\n" +
                                        $"Green: Mean = {meanG:F2}, StdDev = {stdDevG:F2}\n" +
                                        $"Red: Mean = {meanR:F2}, StdDev = {stdDevR:F2}", "Color Statistics", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Invalid crop rectangle. Please ensure it is within the image bounds.");
                    }
                }
                else
                {
                    MessageBox.Show("Crop transformation can be applied to both grayscale and color images!");
                }

                DrawingHelper.DrawRectangle(initialCanvas, new System.Windows.Point(cropRect.X, cropRect.Y), new System.Windows.Point(cropRect.X + cropRect.Width, cropRect.Y + cropRect.Height), 2, System.Windows.Media.Brushes.Red, 1.0);

                MouseClickCollection.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private Rectangle GetRectangleFromMouseClicks()
        {
            if (MouseClickCollection == null || MouseClickCollection.Count < 2)
            {
                throw new InvalidOperationException("Please select a region by clicking at least twice on the image.");
            }

            var point1 = MouseClickCollection[MouseClickCollection.Count - 2];
            var point2 = MouseClickCollection[MouseClickCollection.Count - 1];

            int x1 = (int)Math.Min(point1.X, point2.X);
            int y1 = (int)Math.Min(point1.Y, point2.Y);
            int x2 = (int)Math.Max(point1.X, point2.X);
            int y2 = (int)Math.Max(point1.Y, point2.Y);
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        private bool IsValidRectangle(Rectangle rect, int imageWidth, int imageHeight)
        {
            return rect.X >= 0 && rect.Y >= 0 &&
                   rect.Right <= imageWidth && rect.Bottom <= imageHeight;
        }

        #endregion

        #region Mirror image
        private ICommand _mirrorImageCommand;
        public ICommand MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                return _mirrorImageCommand;
            }
        }

        private void MirrorImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Mirror(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Mirror(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else
            {
                MessageBox.Show("Mirror transformation can be applied to both grayscale and color images!");
            }
        }
        #endregion



        #region Rotate image
        private ICommand _rotateClockwiseImageCommand;
        public ICommand RotateClockwiseImageCommand
        {
            get
            {
                if (_rotateClockwiseImageCommand == null)
                    _rotateClockwiseImageCommand = new RelayCommand(RotateClockwiseImage);
                return _rotateClockwiseImageCommand;
            }
        }

        private void RotateClockwiseImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else
            {
                MessageBox.Show("Rotate transformation can be applied to both grayscale and color images!");
            }
        }

        private ICommand _rotateAntiClockwiseImageCommand;
        public ICommand RotateAntiClockwiseImageCommand
        {
            get
            {
                if (_rotateAntiClockwiseImageCommand == null)
                    _rotateAntiClockwiseImageCommand = new RelayCommand(RotateAntiClockwiseImage);
                return _rotateAntiClockwiseImageCommand;
            }
        }

        private void RotateAntiClockwiseImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateAntiClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateAntiClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else
            {
                MessageBox.Show("Rotate transformation can be applied to both grayscale and color images!");
            }
        }
        #endregion



        #endregion




        #region Pointwise operations
        #region Open Spline Tool
        private ICommand _openSplineToolCommand;
        public ICommand OpenSplineToolCommand
        {
            get
            {
                if (_openSplineToolCommand == null)
                    _openSplineToolCommand = new RelayCommand(OpenSplineTool);
                return _openSplineToolCommand;
            }
        }

        private void OpenSplineTool(object parameter)
        {
            SplineToolWindow splineToolWindow = new SplineToolWindow();
            splineToolWindow.Show();
        }
        #endregion

        #region Apply LUT and Draw Spline
        private ICommand _applyLUTAndDrawSplineCommand;
        public ICommand ApplyLUTAndDrawSplineCommand
        {
            get
            {
                if (_applyLUTAndDrawSplineCommand == null)
                    _applyLUTAndDrawSplineCommand = new RelayCommand(ApplyLUTAndDrawSpline);
                return _applyLUTAndDrawSplineCommand;
            }
        }


        public static double[] CalculateLUT(List<System.Windows.Point> points)
        {
            double[] lut = new double[256];

            for (int i = 0; i < 256; i++)
                lut[i] = -1;

            for (int i = 0; i < points.Count - 1; i++)
            {
                System.Windows.Point p0 = points[i];
                System.Windows.Point p1 = points[i + 1];

                // Tangente Hermite: zero la capete, altfel centrala
                Vector m0 = (i == 0) ? new Vector(0, 0) : (points[i + 1] - points[i - 1]) * 0.5;
                Vector m1 = (i == points.Count - 2) ? new Vector(0, 0) : (points[i + 2] - points[i]) * 0.5;

                int xStart = (int)Math.Round(p0.X);
                int xEnd = (int)Math.Round(p1.X);

                // Protectie sa nu depasim 0–255
                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(255, xEnd);

                for (int x = xStart; x <= xEnd; x++)
                {
                    double t = (x - p0.X) / (p1.X - p0.X);

                    // Baza Hermite
                    double h00 = 2 * t * t * t - 3 * t * t + 1;
                    double h10 = t * t * t - 2 * t * t + t;
                    double h01 = -2 * t * t * t + 3 * t * t;
                    double h11 = t * t * t - t * t;

                    double y = h00 * p0.Y + h10 * m0.Y + h01 * p1.Y + h11 * m1.Y;

                    lut[x] = Math.Min(255, Math.Max(0, y));
                }
            }

            double lastValid = 0;
            for (int i = 0; i < 256; i++)
            {
                if (lut[i] == -1)
                {
                    lut[i] = lastValid;
                }
                else
                {
                    lastValid = lut[i];
                }
            }

            return lut;
        }
        private void ApplyLUTAndDrawSpline(object parameter)
        {
            var splineToolWindow = parameter as SplineToolWindow;
            if (splineToolWindow == null || splineToolWindow.SelectedPoints.Count < 1)
            {
                MessageBox.Show("Please select at least 1 point.", "Not Enough Points", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            splineToolWindow.SelectedPoints.Sort((p1, p2) => p1.X.CompareTo(p2.X));

            splineToolWindow.SelectedPoints.Insert(0, new System.Windows.Point(0, 0));
            splineToolWindow.SelectedPoints.Add(new System.Windows.Point(255, 255));

            splineToolWindow.RightDrawingCanvas.Children.Clear();

            splineToolWindow.DrawCoordinateSystem(splineToolWindow.RightDrawingCanvas);

            splineToolWindow.DrawHermiteSpline(splineToolWindow.RightDrawingCanvas, splineToolWindow.SelectedPoints);

            double[] lut = CalculateLUT(splineToolWindow.SelectedPoints);

            ApplyLUTToImage(lut);
        }

        private void ApplyLUTToImage(double[] lut)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.ApplyLUT(GrayInitialImage, lut);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.ApplyLUT(ColorInitialImage, lut);
                ProcessedImage = Convert(ColorProcessedImage);
            }

        }
        #endregion
        #endregion

        #region Thresholding
        #region Binary Triangle image
        private ICommand _binaryTriangleCommand;
        public ICommand BinaryTriangleCommand
        {
            get
            {
                if (_binaryTriangleCommand == null)
                    _binaryTriangleCommand = new RelayCommand(BinaryTriangleImage);
                return _binaryTriangleCommand;
            }
        }
        private void BinaryTriangleImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                int threshold = CalculateTriangleThreshold(GrayInitialImage);

                GrayProcessedImage = Thresholding.Binary(GrayInitialImage,threshold);

                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("Binary transformation can only be applied to grayscale images!");
            }
        }
        private int CalculateTriangleThreshold(Image<Gray, byte> inputImage)
        {
            int[] histogram = new int[256];
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte pixelValue = inputImage.Data[y, x, 0];
                    histogram[pixelValue]++;
                }
            }

            int min = 0, max = 255;
            while (histogram[min] == 0) min++;
            while (histogram[max] == 0) max--;

            if (min >= max)
                return min;

            int peak = min;
            for (int i = min + 1; i <= max; i++)
            {
                if (histogram[i] > histogram[peak])
                    peak = i;
            }

            double maxDistance = 0;
            int threshold = min;
            double slope, intercept;
            int start, end;

            if ((peak - min) < (max - peak))
            {
                start = peak;
                end = max;
                slope = (double)(histogram[end] - histogram[start]) / (end - start);
                intercept = histogram[start] - slope * start;

                for (int i = start; i <= end; i++)
                {
                    double distance = Math.Abs(slope * i - histogram[i] + intercept) / Math.Sqrt(slope * slope + 1);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        threshold = i;
                    }
                }
            }
            else
            {
                start = min;
                end = peak;
                slope = (double)(histogram[end] - histogram[start]) / (end - start);
                intercept = histogram[start] - slope * start;

                for (int i = start; i <= end; i++)
                {
                    double distance = Math.Abs(slope * i - histogram[i] + intercept) / Math.Sqrt(slope * slope + 1);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        threshold = i;
                    }
                }
            }

            return threshold;
        }

        #endregion
        #endregion


        #region Filters
        #region Median Filter
        private ICommand _medianFilterCommand;
        public ICommand MedianFilterCommand
        {
            get
            {
                if (_medianFilterCommand == null)
                    _medianFilterCommand = new RelayCommand(MedianFilter);
                return _medianFilterCommand;
            }
        }

        private void MedianFilter(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> labels = new List<string>
            {
                "Filter mask size (odd number):"
            };

            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            if (values.Count < 1 || values[0] < 1 || values[0] % 2 == 0)
            {
                MessageBox.Show("Invalid mask size. Please enter an odd number greater than 0.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double maskSize = values[0];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.MedianFilter(GrayInitialImage, (int)maskSize);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Filters.MedianFilter(ColorInitialImage, (int)maskSize);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else
            {
                MessageBox.Show("Median filter can only be applied to grayscale or color images!");
            }
        }
        #endregion

        #region LoG Filter
        private ICommand _loGFilterCommand;
        public ICommand LoGFilterCommand
        {
            get
            {
                if (_loGFilterCommand == null)
                    _loGFilterCommand = new RelayCommand(ApplyLoGFilter);
                return _loGFilterCommand;
            }
        }

        private void ApplyLoGFilter(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> labels = new List<string>
            {
                "Threshold value:"
            };

            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            if (values.Count < 1)
            {
                MessageBox.Show("Invalid threshold value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double threshold = values[0];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.LoG(GrayInitialImage, threshold);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Filters.LoGColor(ColorInitialImage, threshold);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else
            {
                MessageBox.Show("LoG transformation can be applied to both grayscale and color images!");
            }
        }
        #endregion
        #endregion

        #region Connected Components
        

        #region Connected Components
        private ICommand _connectedComponentsCommand;
        public ICommand ConnectedComponentsCommand
        {
            get
            {
                if (_connectedComponentsCommand == null)
                    _connectedComponentsCommand = new RelayCommand(ConnectedComponents);
                return _connectedComponentsCommand;
            }
        }

        private void ConnectedComponents(object parameter)
        {
            if (GrayInitialImage == null)
            {
                MessageBox.Show("Please add a binary image!");
                return;
            }

            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                int threshold = CalculateTriangleThreshold(GrayInitialImage);

                GrayProcessedImage = Thresholding.Binary(GrayInitialImage, threshold);

                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("Binary transformation can only be applied to grayscale images!");
            }
            ColorProcessedImage = MorphologicalOperations.ConnectedComponents(GrayProcessedImage);
            ProcessedImage = Convert(ColorProcessedImage);
        }
        #endregion
        #endregion

        #region Geometric transformations
        #region Spherical deformation

        private ICommand _sphericalDeformationCommand;
        public ICommand SphericalDeformationCommand
        {
            get
            {
                if (_sphericalDeformationCommand == null)
                    _sphericalDeformationCommand = new RelayCommand(ExecuteSphericalDeformation);
                return _sphericalDeformationCommand;
            }
        }

        private void ExecuteSphericalDeformation(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            var labels = new List<string>
            {
                "Indicele de refracție (ρ):",
                "Raza lentilei (r_max):"
            };
            var dialog = new DialogWindow(_mainVM, labels);
            dialog.ShowDialog();
            List<double> values = dialog.GetValues();
            if (values.Count < 2 || values[0] <= 0 || values[1] <= 0)
            {
                MessageBox.Show("Invalid parameters. ρ and r_max must be positive numbers.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double rho = values[0];
            double rMax = values[1];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = GeometricTransformations.SphericalDeformation(GrayInitialImage, rho, rMax);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = GeometricTransformations.SphericalDeformation(ColorInitialImage, rho, rMax);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else
            {
                MessageBox.Show("Spherical deformation can be applied to both grayscale and color images!");
                return;
            }
        }

        #endregion

        #endregion

        #region Segmentation
        #region K-Means Clustering
        private ICommand _kMeansClusteringCommand;
        public ICommand KMeansClusteringCommand
        {
            get
            {
                if (_kMeansClusteringCommand == null)
                    _kMeansClusteringCommand = new RelayCommand(KMeansClustering);
                return _kMeansClusteringCommand;
            }
        }

        private void KMeansClustering(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> labels = new List<string>
            {
                "Number of clusters (k):"
            };

            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            if (values.Count < 1 || values[0] < 1)
            {
                MessageBox.Show("Invalid number of clusters. Please enter a positive integer value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int k = (int)values[0];

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Segmentation.ApplyKMeansClustering(GrayInitialImage, k);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion

        #endregion

        #region Use processed image as initial image
        private ICommand _useProcessedImageAsInitialImageCommand;
        public ICommand UseProcessedImageAsInitialImageCommand
        {
            get
            {
                if (_useProcessedImageAsInitialImageCommand == null)
                    _useProcessedImageAsInitialImageCommand = new RelayCommand(UseProcessedImageAsInitialImage);
                return _useProcessedImageAsInitialImageCommand;
            }
        }

        private void UseProcessedImageAsInitialImage(object parameter)
        {
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0] as Canvas);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
                InitialImage = Convert(GrayInitialImage);
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
                InitialImage = Convert(ColorInitialImage);
            }

            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion
    }
}