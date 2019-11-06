using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfGraph.Formula;

namespace WpfGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GraphDrawer drawer;
        public MainWindow()
        {
            InitializeComponent();
        }

        Vector2 DragStartVector = null;
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas box = (sender as Canvas);
            DragStartVector = new Vector2(e.GetPosition(box).X, e.GetPosition(box).Y);
            /*
            //TOREMOVE TEMPORARY FUNCTION FOR TOUCHPAD MOUSE, WHICH DOESNT HAVE SCROLLWHEEL
            for (byte i = 0; i < 25; i++)
            {
                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        {
                            drawer.Zoom(1);
                            break;
                        }
                    case MouseButton.Right:
                        {
                            drawer.Zoom(-1);
                            break;
                        }

                }
                Title = drawer.GetZoom().ToString();
                ReDrawGraph();
                //InvalidateVisual();
                //System.Threading.Thread.Sleep(100);
            }
            */
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas box = (sender as Canvas);
            Vector2 CurrentVector = new Vector2(e.GetPosition(box).X, e.GetPosition(box).Y);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Vector2 diff = Vector2.Subtract(DragStartVector, CurrentVector);
                drawer.originPoint.Subtract(diff);
                ReDrawGraph();
            }
            else 
                if (e.RightButton == MouseButtonState.Pressed)
            {
                ReDrawGraph(CurrentVector);
            }
            else
                ReDrawGraph();

            DragStartVector = CurrentVector;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            drawer.Zoom(e.Delta/120);
            ReDrawGraph();
            Title = drawer.GetZoom().ToString();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (drawer != null) {
                drawer.originPoint.Add(new Vector2((e.NewSize.Width - e.PreviousSize.Width) / 2, 0) );
                drawer.originPoint.Add(new Vector2(0, (e.NewSize.Height - e.PreviousSize.Height) / 2) );
                ReDrawGraph();
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            drawer = new GraphDrawer(DrawingCanvas);
            ReDrawGraph();
        }

        private void ReDrawGraph(Vector2 popup_point = null)
        {
            drawer.Empty();
            drawer.DrawGrid();
            drawer.DrawAxis();
            drawer.DrawFormulae();
            if (popup_point != null)
                drawer.DrawPopup(popup_point);
        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void DrawingCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        Dictionary<TextBox, string> editboxes = new Dictionary<TextBox, string>();
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (drawer == null) return;

            if (editboxes.ContainsKey((sender as TextBox)))
                editboxes.Remove((sender as TextBox));
            
            editboxes.Add((sender as TextBox), (sender as TextBox).Text);

            Dictionary<TextBox,String>.Enumerator iterator = editboxes.GetEnumerator();
            List<FormulaPart> formulae = new List<FormulaPart>();
            try
            {
                while (iterator.MoveNext())
                    formulae.Add(FormulaPart.ProcessString(iterator.Current.Value));

            }
            catch (ArgumentException ex)
            {
                Title = ex.Message;
            }

            drawer.Formulae = formulae;
            ReDrawGraph();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }

        private void DrawingCanvas_Initialized(object sender, EventArgs e)
        {

        }
    }
}
