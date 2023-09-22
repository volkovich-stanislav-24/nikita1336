using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Application1.Views
{
    [ContentProperty(nameof(Children))]
    public partial class ResizableCanvas : ScrollViewer
    {
        public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(
            nameof(Children),
            typeof(UIElementCollection),
            typeof(ResizableCanvas),
            new PropertyMetadata()
        );
        public UIElementCollection Children
        {
            get => (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
            private init => SetValue(ChildrenProperty, value);
        }

        public ResizableCanvas()
        {
            InitializeComponent();
            Children = canvas.Children;
        }

        FrameworkElement? __dragging;

        public void BeginDrag(FrameworkElement dragging)
        {
            __dragging = dragging;
        }

        void canvas_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            __dragging = null;
        }

        void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (__dragging != null)
            {
                var mouse_position = e.GetPosition(canvas);
                double y = mouse_position.Y - __dragging.ActualHeight * .5, x = mouse_position.X - __dragging.ActualWidth * .5;
                double y_w_offset = y - VerticalOffset, x_w_offset = x - HorizontalOffset;
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
