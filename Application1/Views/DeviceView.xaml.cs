using Application1.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Application1.Converters
{
    class DeviceViewIsOnImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valid_parameter = (ResourceDictionary)parameter;
            return (bool)value ? valid_parameter["On"] : valid_parameter["Off"];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    class DeviceViewConnectionsCurMaxTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => string.Format("{0}/{1}", values[0], values[1]);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    class ConnectionViewEllipseFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valid_parameter = (ResourceDictionary)parameter;
            return (bool)value ? valid_parameter["On"] : valid_parameter["Off"];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    class ConnectionViewEllipseXConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var width = (double)values[0];
            var line_x1 = (double)values[1];
            return line_x1 - width * .5;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
    class ConnectionViewEllipseYConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var height = (double)values[0];
            var line_y1 = (double)values[1];
            return line_y1 - height * .5;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

namespace Application1.Views
{
    public partial class DeviceView : Grid
    {
        bool __is_dragging;
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(DeviceViewModel),
            typeof(DeviceView)
        );
        public DeviceViewModel ViewModel
        {
            get => (DeviceViewModel)GetValue(ViewModelProperty);
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }
        public DeviceView()
        {
            InitializeComponent();
        }

        void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var valid_sender = (DeviceView)sender;
            valid_sender.Focus();
            valid_sender.__is_dragging = true;
        }
        void root_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var valid_sender = (DeviceView)sender;
            valid_sender.__is_dragging = false;
        }
        void root_MouseMove(object sender, MouseEventArgs e)
        {
            var valid_sender = (DeviceView)sender;
            if (valid_sender.__is_dragging)
            {
                var mouse_position = e.GetPosition((UIElement)valid_sender.Parent);
                Canvas.SetLeft(valid_sender, mouse_position.X - valid_sender.Width * .5);
                Canvas.SetTop(valid_sender, mouse_position.Y - valid_sender.Height * .5);
            }
        }

        void Connections_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        void Connections_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.Model.IsOn = !ViewModel.Model.IsOn;
            e.Handled = true;
        }

        void root_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && MessageBox.Show($"Вы точно хотите удалить {ViewModel.Model.Name}?", $"Удаление {ViewModel.Model.Name}", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                ViewModel.Model.Delete();
        }

        void TextBox_KeyUp(object sender, KeyEventArgs e)
            => e.Handled = true;

        /*void OnDelete(DeviceViewModel device_view) => ((Canvas)Parent).Children.Remove(this);*/

        void Ellipse_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ViewModel.Model.IsOn = !ViewModel.Model.IsOn;
            e.Handled = true;
        }

        void Name_Error(object sender, ValidationErrorEventArgs e)
        {
            var valid_sender = (TextBox)sender;
            ToolTip tool_tip = new();
            tool_tip.Placement = PlacementMode.Center;
            tool_tip.PlacementTarget = valid_sender;
            
            tool_tip.Content = e.Error.Exception.Message;
            Binding bind = new Binding();
            bind.Source = tool_tip;
            bind.Path = new PropertyPath("ActualHeight");
            bind.Mode = BindingMode.OneWay;
            tool_tip.SetBinding(System.Windows.Controls.ToolTip.VerticalOffsetProperty, bind);
            valid_sender.ToolTip = tool_tip;
            tool_tip.StaysOpen = false;
            tool_tip.IsOpen = true;
            
            var bindingExpression = BindingOperations.GetBindingExpression(valid_sender, TextBox.TextProperty);
            if (bindingExpression != null && bindingExpression.ValidationError != null)
                Validation.ClearInvalid(bindingExpression);
            var a = Validation.GetHasError(valid_sender);
            valid_sender.Text = ViewModel.Model.Name;
        }
    }
}
