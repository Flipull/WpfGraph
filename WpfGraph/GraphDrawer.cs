using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfGraph.Formula;

namespace WpfGraph
{
    class GraphDrawer
    {
        private Canvas canvas = null;
        public Vector2 originPoint { get; set; }
        private double zoom = 1;
        public List<FormulaPart> Formulae { get; set; }
        
        private double gridThickness = 2;

        private Vector2 TranslateCoordsToCanvas(double x, double y)
        {
            return Vector2.Add(new Vector2(x, y).Multiply(zoom), originPoint);
        }
        private Vector2 TranslateCoordsToCanvas(Vector2 v)
        {
            return Vector2.Add(new Vector2(v.X, v.Y).Multiply(zoom), originPoint);
        }
        private Vector2 TranslatePointToVector(Vector2 v)
        {

            return Vector2.Subtract(v, originPoint).Divide(zoom);
        }

        public GraphDrawer(Canvas c)
        {
            canvas = c;
            originPoint = new Vector2(c.ActualWidth / 2d, c.ActualHeight / 2d);
            zoom = 1;
            Formulae = new List<FormulaPart>();
        }

        public double GetZoom()
        {
            return zoom;
        }
        public void Zoom(double delta)
        {
            zoom += 0.1 * zoom * delta;
        }

        private Line CreateLine(Brush brush, double thickness, Vector2 topleft, Vector2 bottomright)
        {
            return CreateLine(brush, thickness, topleft.X, topleft.Y, bottomright.X, bottomright.Y);
        }
        private Line CreateLine(Brush brush, double thickness, double x_left, double y_top, double x_right, double y_bottom)
        {
            Line linestub = new Line();
            linestub.Stroke = brush;
            linestub.StrokeThickness = thickness;
            linestub.X1 = x_left;
            linestub.Y1 = y_top;
            linestub.X2 = x_right;
            linestub.Y2 = y_bottom;
            return linestub;
        }
        
        private TextBlock CreateText(Vector2 p, string text)
        {
            TextBlock column_caption = new TextBlock();
            column_caption.Text = text;
            column_caption.FontSize = 8;
            Canvas.SetLeft(column_caption, p.X);
            Canvas.SetTop(column_caption, p.Y);
            return column_caption;
        }
        private Tuple<Vector2, Vector2> DrawingRegion()
        {
            return new Tuple<Vector2, Vector2>(
                new Vector2(0,0),
                new Vector2(canvas.ActualWidth, canvas.ActualHeight)
                );
        }
        private Tuple<Vector2, Vector2> DrawingCoords()
        {
            Tuple<Vector2, Vector2> drawing_region = DrawingRegion();

            Tuple<Vector2, Vector2> translated_coords = new Tuple<Vector2, Vector2>(
                TranslatePointToVector(drawing_region.Item1),
                TranslatePointToVector(drawing_region.Item2)
                );

            return translated_coords;
        }

        public void Empty()
        {
            canvas.Children.Clear();
        }
        public void DrawAxis()
        {
            Line y_axis = CreateLine(System.Windows.Media.Brushes.LightSteelBlue, gridThickness,
                originPoint.X, 0, originPoint.X, canvas.ActualHeight
                );
            Line x_axis = CreateLine(System.Windows.Media.Brushes.LightSteelBlue, gridThickness,
                0, originPoint.Y, canvas.ActualWidth, originPoint.Y
                );
            canvas.Children.Add(y_axis);
            canvas.Children.Add(x_axis);
        }



        private double DecimalCounter(double value)
        {
            byte counter = 0;
            if (value >= 1)
            {
                while (value > 10)
                {
                    value /= 10;
                    counter++;
                }
            }
            else
            {
                while (value <= 0.1)
                {
                    value *= 10;
                    counter--;
                }
            }
            return counter;
        }
        private void drawgridlines(double grid_spacing, bool drawnumbers=true, double alpha=1)
        {
            Tuple<Vector2, Vector2> coords = DrawingCoords();
            Color color = Color.FromArgb( (byte)(255*alpha), 0, 0, 0);
            SolidColorBrush brush = new SolidColorBrush(color);
            double draw_row = Math.Ceiling(coords.Item1.Y / grid_spacing) * grid_spacing;
            double draw_row_end = Math.Floor(coords.Item2.Y / grid_spacing) * grid_spacing;
            while (draw_row <= draw_row_end)
            {
                Line gridline = CreateLine(brush, gridThickness,
                    TranslateCoordsToCanvas(coords.Item1.X, draw_row),
                    TranslateCoordsToCanvas(coords.Item2.X, draw_row)
                );
                canvas.Children.Add(gridline);

                if (drawnumbers)
                    canvas.Children.Add(
                        CreateText(TranslateCoordsToCanvas(0, draw_row), draw_row.ToString())
                        );

                draw_row += grid_spacing;
            }

            double draw_column = Math.Ceiling(coords.Item1.X / grid_spacing) * grid_spacing;
            double draw_column_end = Math.Floor(coords.Item2.X / grid_spacing) * grid_spacing;
            while (draw_column <= draw_column_end)
            {
                Line gridline = CreateLine(brush, gridThickness,
                    TranslateCoordsToCanvas(draw_column, coords.Item1.Y),
                    TranslateCoordsToCanvas(draw_column, coords.Item2.Y)
                );
                canvas.Children.Add(gridline);
                

                if (drawnumbers)
                {
                    TextBlock number = CreateText(TranslateCoordsToCanvas(draw_column, 0), draw_column.ToString());
                    Canvas.SetZIndex(number, 10);
                    canvas.Children.Add(number);
                }

                draw_column += grid_spacing;
            }
        }
        public void DrawGrid()
        {
            
            //double grid_spacing = Math.Pow(10, DecimalCounter(zoom) )  *  gridMinSpacing;
            //Console.WriteLine(grid_spacing);
            Tuple<Vector2, Vector2> coords = DrawingCoords();
            double window_space = DecimalCounter((coords.Item2.X - coords.Item1.X));
            double distance_plusspace = (coords.Item2.X - coords.Item1.X) / Math.Pow(10, window_space+1);

            double grid_spacing = Math.Pow(10, window_space-1);
            drawgridlines(grid_spacing, distance_plusspace < 0.3, 1- distance_plusspace);
            
            grid_spacing = Math.Pow(10, window_space);
            drawgridlines(grid_spacing, distance_plusspace >= 0.3, distance_plusspace);

        }
        public void DrawFormulae()
        {
            foreach(FormulaPart formula in Formulae)
            {
                //get screen-region
                Tuple<Vector2, Vector2> window = DrawingRegion();

                List<Tuple<string, double>> variables = new List<Tuple<string, double>>();

                Vector2 lastpoint = null;
                //enumerate from x1 to x2
                for (double w = window.Item1.X-1; w <= window.Item2.X+1; w++)
                {
                    double graph_xcoord = TranslatePointToVector(new Vector2(w, 0) ).X;

                    variables.Clear();
                    variables.Add(new Tuple<string, double>("x", graph_xcoord));
                    double graph_ycoord = formula.Evaluate(variables);
                    if (graph_ycoord > double.NegativeInfinity &&
                        graph_ycoord < double.PositiveInfinity)
                    {
                        Vector2 canvaspoint = TranslateCoordsToCanvas(new Vector2(graph_xcoord, graph_ycoord));
                        if (lastpoint != null)
                        {
                            Line line = CreateLine(new SolidColorBrush(Colors.Red), 2,
                                lastpoint, canvaspoint);

                            canvas.Children.Add(line);

                        }
                        lastpoint = canvaspoint;

                    } else
                    {
                        lastpoint = null;
                    }
                }
            }

        }
        public void DrawPopup(Vector2 point)
        {
            Vector2 graph_coords = TranslatePointToVector(point);
            FormulaPart closestgraph = null;
            double closestvalue = double.PositiveInfinity;
            double closestvalue_dx = double.PositiveInfinity;

            List<Tuple<string, double>> variables = new List<Tuple<string, double>>();
            variables.Add(new Tuple<string, double>("x", graph_coords.X));
            List<Tuple<string, double>> variables_prime = new List<Tuple<string, double>>();
            variables_prime.Add(new Tuple<string, double>("x", graph_coords.X + 1e-6));
            
            foreach (FormulaPart formula in Formulae)
            {
                double answer = formula.Evaluate(variables);
                if (Math.Abs(answer - graph_coords.Y) < Math.Abs(closestvalue - graph_coords.Y) )
                {
                    closestvalue = answer;
                    closestvalue_dx = formula.Evaluate(variables_prime);
                    closestgraph = formula;
                }
            }
            if (closestgraph == null) return;
            

            Ellipse dot = new Ellipse();
            dot.Width = 6;
            dot.Height = 6;
            dot.StrokeThickness = 2;
            dot.Stroke = new SolidColorBrush(Colors.Blue);

            Vector2 canvaspoint = TranslateCoordsToCanvas(graph_coords.X, closestvalue);
            Canvas.SetLeft(dot, canvaspoint.X);
            Canvas.SetTop(dot, canvaspoint.Y);
            Canvas.SetZIndex(dot, 10);

            canvas.Children.Add(dot);

            Rectangle rect = new Rectangle();
            rect.Width = 100;
            rect.Height = 100;

            rect.Fill = new SolidColorBrush(Colors.SteelBlue);
            rect.StrokeThickness = 4;
            rect.Stroke = new SolidColorBrush(Colors.Blue);
            Canvas.SetLeft(rect, 10);
            Canvas.SetTop(rect, 10);
            canvas.Children.Add(rect);

            canvas.Children.Add(
                CreateText(new Vector2(20, 20), "Trace:")
                );
            canvas.Children.Add(
                CreateText(new Vector2(20, 30), "X     : " + graph_coords.X)
                );
            canvas.Children.Add(
                CreateText(new Vector2(20, 40), "f(X)  : " + closestvalue)
                );
            canvas.Children.Add(
                CreateText(new Vector2(20, 50), "f(X)' : " + (closestvalue_dx - closestvalue)/1e-6 )
                );
            


        }

    }
}
