using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Input;

namespace Application1.Converters
{
    abstract class ConnectionViewLineYXConverter
    {
        protected double ValueAsDouble(object value)
            => value is double && !double.IsNaN((double)value) ? (double)value : 0;
    }
    sealed class ConnectionViewLineYConverter : ConnectionViewLineYXConverter, IMultiValueConverter
    {
        // values[0] - height
        // values[1] - y
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => ValueAsDouble(values[1]) + ValueAsDouble(values[0]);
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    sealed class ConnectionViewLineXConverter : ConnectionViewLineYXConverter, IMultiValueConverter
    {
        // values[0] - width
        // values[1] - x
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => ValueAsDouble(values[1]) + ValueAsDouble(values[0]) * .5;
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

namespace Application1.Views
{
    public partial class ConnectionView : Canvas
    {
        const string __DELETE_TITLE = "Удаление соединения между {0} и {1}";
        const string __DELETE_TEXT = "Вы точно хотите удалить соединение между {0} и {1}?";

        public static readonly DependencyProperty SourceDeviceViewProperty = DependencyProperty.Register(
            nameof(SourceDeviceView),
            typeof(DeviceView),
            typeof(ConnectionView)
        );
        public DeviceView SourceDeviceView
        {
            get => (DeviceView)GetValue(SourceDeviceViewProperty);
            set => SetValue(SourceDeviceViewProperty, value);
        }
        public static readonly DependencyProperty DestinationDeviceViewProperty = DependencyProperty.Register(
            nameof(DestinationDeviceView),
            typeof(DeviceView),
            typeof(ConnectionView)
        );
        public DeviceView DestinationDeviceView
        {
            get => (DeviceView)GetValue(DestinationDeviceViewProperty);
            set => SetValue(DestinationDeviceViewProperty, value);
        }

        public ConnectionView()
            => InitializeComponent();

        // line

        void line_KeyUp(object sender, KeyEventArgs e)
        {
            var valid_sender = (Line)sender;
            if (e.Key == Key.Delete
                && MessageBox.Show(
                    string.Format(__DELETE_TEXT, SourceDeviceView.ViewModel.Model.Name, DestinationDeviceView.ViewModel.Model.Name),
                    string.Format(__DELETE_TITLE, SourceDeviceView.ViewModel.Model.Name, DestinationDeviceView.ViewModel.Model.Name),
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes
            )
                SourceDeviceView.ViewModel.Model.Disconnect(DestinationDeviceView.ViewModel.Model);
            e.Handled = true;
        }

        void line_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            var valid_sender = (Line)sender;
            valid_sender.Focus();
            e.Handled = true;
        }
    }
}
