using Application1.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Application1.Converters
{
    sealed class DeviceViewIsOnImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valid_parameter = (ResourceDictionary)parameter;
            return (bool)value ? valid_parameter["On"] : valid_parameter["Off"];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    sealed class DeviceViewConnectionsCurMaxTextConverter : IMultiValueConverter
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
}

namespace Application1.Views
{
    public partial class DeviceView : Grid
    {
        const string DELETE_TITLE = "Удаление {0}";
        const string DELETE_TEXT = "Вы точно хотите удалить {0}?";

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
            => InitializeComponent();

        // root

        void root_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete
                && MessageBox.Show(
                    string.Format(DELETE_TEXT, ViewModel.Model.Name),
                    string.Format(DELETE_TITLE, ViewModel.Model.Name),
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes
            )
                ViewModel.Model.Delete();
            e.Handled = true;
        }

        void root_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            var valid_sender = (DeviceView)sender;
            valid_sender.Focus();
            valid_sender.__is_dragging = true;
            e.Handled = true;
        }

        void root_MouseLeftUp(object sender, MouseButtonEventArgs e)
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
                double y = mouse_position.Y - valid_sender.Height * .5, x = mouse_position.X - valid_sender.Width * .5;
                Canvas.SetTop(valid_sender, y);
                Canvas.SetLeft(valid_sender, x);
                e.Handled = true;
            }
        }

        // name

        void name_Error(object sender, ValidationErrorEventArgs e)
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

            /*var bindingExpression = BindingOperations.GetBindingExpression(valid_sender, TextBox.TextProperty);
            if (bindingExpression != null && bindingExpression.ValidationError != null)
                Validation.ClearInvalid(bindingExpression);
            valid_sender.Text = ViewModel.Model.Name;*/
        }

        // connections

        void connections_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ViewModel.Model.IsOn = !ViewModel.Model.IsOn;
            e.Handled = true;
        }

        void connections_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            ((MainView)((Canvas)Parent).Parent).BeginConect(this);
            e.Handled = true;
        }

        void connections_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            if (!((MainView)((Canvas)Parent).Parent).EndConnect(this))
                ViewModel.Model.IsOn = !ViewModel.Model.IsOn;
        }


        // max connections

        const string __MAX_CONNECTIONS_ERROR = "Максимальное количество соединений не может быть меньше текущего.";

        void max_connections_Error(object sender, ValidationErrorEventArgs e)
        {
            var valid_sender = (TextBox)sender;
            ToolTip tool_tip = new();
            tool_tip.Placement = PlacementMode.Center;
            tool_tip.PlacementTarget = valid_sender;
            tool_tip.Content = __MAX_CONNECTIONS_ERROR;
            Binding bind = new();
            bind.Source = tool_tip;
            bind.Mode = BindingMode.OneWay;
            bind.Path = new PropertyPath("ActualHeight");
            tool_tip.SetBinding(System.Windows.Controls.ToolTip.VerticalOffsetProperty, bind);
            valid_sender.ToolTip = tool_tip;
            tool_tip.StaysOpen = false;
            tool_tip.IsOpen = true;

            /*var bindingExpression = BindingOperations.GetBindingExpression(valid_sender, TextBox.TextProperty);
            if (bindingExpression != null && bindingExpression.ValidationError != null)
                Validation.ClearInvalid(bindingExpression);
            valid_sender.Text = ViewModel.Model.MaxConnections.ToString();*/
        }
    }
}
