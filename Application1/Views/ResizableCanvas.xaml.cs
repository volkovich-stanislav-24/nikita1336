using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Application1.Views
{
    public partial class ResizableCanvas : ScrollViewer
    {
        public ResizableCanvas()
        {
            InitializeComponent();
        }

        FrameworkElement? __dragging;

        void rectangle_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            __dragging = (FrameworkElement)sender;
        }

        void rectangle_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            __dragging = null;
        }

        void rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (__dragging != null)
            {
                var mouse_position = e.GetPosition(canvas);
                double y = mouse_position.Y - __dragging.ActualHeight * .5, x = mouse_position.X - __dragging.ActualWidth * .5;
                double y_w_offset = y - VerticalOffset, x_w_offset = x - HorizontalOffset;
                tb.Text = $"y: {Math.Round(y)}; x: {Math.Round(x)}.";
                tb_offsets.Text = $"y: {Math.Round(y_w_offset)}; x: {Math.Round(x_w_offset)}.";
                if (y_w_offset < 0)
                    ScrollToVerticalOffset(VerticalOffset + y_w_offset);
                else if (y_w_offset + __dragging.ActualHeight > ViewportHeight)
                    ScrollToVerticalOffset(VerticalOffset - ViewportHeight + y_w_offset + __dragging.ActualHeight);
                if (x_w_offset < 0)
                    ScrollToHorizontalOffset(HorizontalOffset + x_w_offset);
                else if (x_w_offset + __dragging.ActualWidth > ViewportWidth)
                    ScrollToHorizontalOffset(HorizontalOffset - ViewportWidth + x_w_offset + __dragging.ActualWidth);
                Canvas.SetTop(__dragging, double.Clamp(y, 0, canvas.ActualHeight - __dragging.ActualHeight));
                Canvas.SetLeft(__dragging, double.Clamp(x, 0, canvas.ActualWidth - __dragging.ActualWidth));
            }
        }
    }
}
