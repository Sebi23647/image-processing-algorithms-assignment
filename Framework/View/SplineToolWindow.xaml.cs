using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Framework.View
{
    /// <summary>
    /// Interaction logic for SplineToolWindow.xaml
    /// </summary>
    public partial class SplineToolWindow : Window
    {
        public List<Point> SelectedPoints { get; set; } = new List<Point>();

        public SplineToolWindow()
        {
            InitializeComponent();
            // Setam DataContext-ul pentru a putea gasi comanda din view model (MenuCommands)
            this.DataContext = Application.Current.MainWindow.DataContext;
        }

        private void LeftDrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickedPoint = e.GetPosition(LeftDrawingCanvas);
            if (clickedPoint.X >= 20 && clickedPoint.X <= 275 && clickedPoint.Y >= 20 && clickedPoint.Y <= 275)
            {
                if (SelectedPoints.Count < 5)
                {
                    SelectedPoints.Add(new Point(clickedPoint.X - 20, 275 - clickedPoint.Y));
                    DrawPoint(clickedPoint);
                }
                else
                {
                    MessageBox.Show("You can select up to 5 points only.", "Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DrawPoint(Point point)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(ellipse, point.X - 2.5);
            Canvas.SetTop(ellipse, point.Y - 2.5);
            LeftDrawingCanvas.Children.Add(ellipse);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPoints.Clear();
            LeftDrawingCanvas.Children.Clear();

            DrawCoordinateSystem(LeftDrawingCanvas);
        }

        public void DrawCoordinateSystem(Canvas canvas)
        {
            Line xAxis = new Line
            {
                X1 = 20,
                Y1 = 275,
                X2 = 275,
                Y2 = 275,
                Stroke = Brushes.Black
            };
            canvas.Children.Add(xAxis);

            Line yAxis = new Line
            {
                X1 = 20,
                Y1 = 0,
                X2 = 20,
                Y2 = 275,
                Stroke = Brushes.Black
            };
            canvas.Children.Add(yAxis);

            TextBlock text0 = new TextBlock
            {
                Text = "0",
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(text0, 0);
            Canvas.SetTop(text0, 275);
            canvas.Children.Add(text0);

            TextBlock text255X = new TextBlock
            {
                Text = "255",
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(text255X, 265);
            Canvas.SetTop(text255X, 275);
            canvas.Children.Add(text255X);

            TextBlock text255Y = new TextBlock
            {
                Text = "255",
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(text255Y, 0);
            Canvas.SetTop(text255Y, 0);
            canvas.Children.Add(text255Y);

            Line diagonal = new Line
            {
                X1 = 20,
                Y1 = 275,
                X2 = 275,
                Y2 = 20,
                Stroke = Brushes.Gray,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };
            canvas.Children.Add(diagonal);
        }

        public void DrawHermiteSpline(Canvas canvas, List<Point> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point p0 = points[i];
                Point p1 = points[i + 1];

                // Tangente: 0 la capete, centrale în rest
                Vector m0 = (i == 0) ? new Vector(0, 0) : (points[i + 1] - points[i - 1]) * 0.5;
                Vector m1 = (i == points.Count - 2) ? new Vector(0, 0) : (points[i + 2] - points[i]) * 0.5;

                for (double t = 0; t <= 1; t += 0.01)
                {
                    //functii baza hermite
                    double h00 = 2 * t * t * t - 3 * t * t + 1;
                    double h10 = t * t * t - 2 * t * t + t;
                    double h01 = -2 * t * t * t + 3 * t * t;
                    double h11 = t * t * t - t * t;

                    //coordonatele punctului interpolat
                    double x = h00 * p0.X + h10 * m0.X + h01 * p1.X + h11 * m1.X;
                    double y = h00 * p0.Y + h10 * m0.Y + h01 * p1.Y + h11 * m1.Y;

                    x = Math.Max(0, Math.Min(255, x));
                    y = Math.Max(0, Math.Min(255, y));

                    double canvasX = x + 20;
                    double canvasY = 275 - y;

                    Ellipse ellipse = new Ellipse
                    {
                        Width = 3,
                        Height = 3,
                        Fill = Brushes.Blue
                    };

                    Canvas.SetLeft(ellipse, canvasX);
                    Canvas.SetTop(ellipse, canvasY);
                    canvas.Children.Add(ellipse);
                }
            }
        }
    }
}




//...
//using Algorithms.Sections;
//using Framework.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

//namespace Framework.View
//{
//    /// <summary>
//    /// Interaction logic for SplineToolWindow.xaml
//    /// </summary>

//    public partial class SplineToolWindow : Window
//    {
//        public List<Point> SelectedPoints { get; set; } = new List<Point>();

//        public SplineToolWindow()
//        {
//            InitializeComponent();
//        }

//        private void LeftDrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            Point clickedPoint = e.GetPosition(LeftDrawingCanvas);
//            if (clickedPoint.X >= 20 && clickedPoint.X <= 275 && clickedPoint.Y >= 20 && clickedPoint.Y <= 275)
//            {
//                if (SelectedPoints.Count < 5)
//                {
//                    SelectedPoints.Add(new Point(clickedPoint.X - 20, 275 - clickedPoint.Y));
//                    DrawPoint(clickedPoint);
//                }
//                else
//                {
//                    MessageBox.Show("You can select up to 5 points only.", "Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//            }
//        }

//        private void DrawPoint(Point point)
//        {
//            Ellipse ellipse = new Ellipse
//            {
//                Width = 5,
//                Height = 5,
//                Fill = Brushes.Red
//            };
//            Canvas.SetLeft(ellipse, point.X - 2.5);
//            Canvas.SetTop(ellipse, point.Y - 2.5);
//            LeftDrawingCanvas.Children.Add(ellipse);
//        }

//        private void ClearButton_Click(object sender, RoutedEventArgs e)
//        {
//            SelectedPoints.Clear();
//            LeftDrawingCanvas.Children.Clear();

//            DrawCoordinateSystem(LeftDrawingCanvas);
//        }

//        private void DrawSplineButton_Click(object sender, RoutedEventArgs e)
//        {
//            var mainWindow = (MainWindow)Application.Current.MainWindow;
//            var command = ((MainVM)mainWindow.DataContext).MenuCommands.ApplyLUTAndDrawSplineCommand;
//            if (command.CanExecute(this))
//            {
//                command.Execute(this);
//            }
//        }

//        public void DrawCoordinateSystem(Canvas canvas)
//        {
//            Line xAxis = new Line
//            {
//                X1 = 20,
//                Y1 = 275,
//                X2 = 275,
//                Y2 = 275,
//                Stroke = Brushes.Black
//            };
//            canvas.Children.Add(xAxis);

//            Line yAxis = new Line
//            {
//                X1 = 20,
//                Y1 = 0,
//                X2 = 20,
//                Y2 = 275,
//                Stroke = Brushes.Black
//            };
//            canvas.Children.Add(yAxis);

//            TextBlock text0 = new TextBlock
//            {
//                Text = "0",
//                Foreground = Brushes.Black
//            };
//            Canvas.SetLeft(text0, 0);
//            Canvas.SetTop(text0, 275);
//            canvas.Children.Add(text0);

//            TextBlock text255X = new TextBlock
//            {
//                Text = "255",
//                Foreground = Brushes.Black
//            };
//            Canvas.SetLeft(text255X, 265);
//            Canvas.SetTop(text255X, 275);
//            canvas.Children.Add(text255X);

//            TextBlock text255Y = new TextBlock
//            {
//                Text = "255",
//                Foreground = Brushes.Black
//            };
//            Canvas.SetLeft(text255Y, 0);
//            Canvas.SetTop(text255Y, 0);
//            canvas.Children.Add(text255Y);

//            Line diagonal = new Line
//            {
//                X1 = 20,
//                Y1 = 275,
//                X2 = 275,
//                Y2 = 20,
//                Stroke = Brushes.Gray,
//                StrokeDashArray = new DoubleCollection { 2, 2 }
//            };
//            canvas.Children.Add(diagonal);
//        }








//        public void DrawHermiteSpline(Canvas canvas, List<Point> points)
//        {
//            for (int i = 0; i < points.Count - 1; i++)
//            {
//                Point p0 = points[i];
//                Point p1 = points[i + 1];

//                // Tangente: 0 la capete, centrale în rest
//                Vector m0 = (i == 0) ? new Vector(0, 0) : (points[i + 1] - points[i - 1]) * 0.5;
//                Vector m1 = (i == points.Count - 2) ? new Vector(0, 0) : (points[i + 2] - points[i]) * 0.5;

//                for (double t = 0; t <= 1; t += 0.01)
//                {
//                    // Funcții bază Hermite
//                    double h00 = 2 * t * t * t - 3 * t * t + 1;
//                    double h10 = t * t * t - 2 * t * t + t;
//                    double h01 = -2 * t * t * t + 3 * t * t;
//                    double h11 = t * t * t - t * t;

//                    double x = h00 * p0.X + h10 * m0.X + h01 * p1.X + h11 * m1.X;
//                    double y = h00 * p0.Y + h10 * m0.Y + h01 * p1.Y + h11 * m1.Y;

//                    // Clamp la sistemul nostru de coordonate (X: 0–255, Y: 0–255)
//                    x = Math.Max(0, Math.Min(255, x));
//                    y = Math.Max(0, Math.Min(255, y));

//                    // Transformare coordonate în canvas (offset: 20 px, invers Y)
//                    double canvasX = x + 20;
//                    double canvasY = 275 - y;

//                    Ellipse ellipse = new Ellipse
//                    {
//                        Width = 3,
//                        Height = 3,
//                        Fill = Brushes.Blue
//                    };

//                    Canvas.SetLeft(ellipse, canvasX);
//                    Canvas.SetTop(ellipse, canvasY);
//                    canvas.Children.Add(ellipse);
//                }
//            }
//        }


//    }











//        //......
//        //    public void DrawHermiteSpline(Canvas canvas, List<Point> points)
//        //    {
//        //        for (int i = 0; i < points.Count - 1; i++)
//        //        {
//        //            Point p0 = points[i];
//        //            Point p1 = points[i + 1];

//        //            Vector m0 = i == 0 ? new Vector(0, 0) : (points[i + 1] - points[i - 1]) * 0.5;
//        //            Vector m1 = i == points.Count - 2 ? new Vector(0, 0) : (points[i + 2] - points[i]) * 0.5;

//        //            for (double t = 0; t < 1; t += 0.01)
//        //            {
//        //                double h00 = 2 * t * t * t - 3 * t * t + 1;
//        //                double h10 = t * t * t - 2 * t * t + t;
//        //                double h01 = -2 * t * t * t + 3 * t * t;
//        //                double h11 = t * t * t - t * t;

//        //                double x = h00 * p0.X + h10 * m0.X + h01 * p1.X + h11 * m1.X;
//        //                double y = h00 * p0.Y + h10 * m0.Y + h01 * p1.Y + h11 * m1.Y;

//        //                Ellipse ellipse = new Ellipse
//        //                {
//        //                    Width = 10,
//        //                    Height = 10,
//        //                    Fill = Brushes.Blue
//        //                };
//        //                Canvas.SetLeft(ellipse, x + 20);
//        //                Canvas.SetTop(ellipse, 275 - y);
//        //                canvas.Children.Add(ellipse);
//        //            }
//        //        }
//        //    }
//        //}



//    }