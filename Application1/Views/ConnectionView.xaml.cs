using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Input;

namespace Application1.Converters
{
    class ConnectionViewLineXConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = values[0] is double && !double.IsNaN((double)values[1]) ? (double)values[0] : 0;
            double x = values[1] is double && !double.IsNaN((double)values[1]) ? (double)values[1] : 0;
            return x + width * .5;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    class ConnectionViewLineYConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double height = values[0] is double && !double.IsNaN((double)values[1]) ? (double)values[0] : 0;
            double y = values[1] is double && !double.IsNaN((double)values[1]) ? (double)values[1] : 0;
            return y + height;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

namespace Application1.Views
{
    public partial class ConnectionView : Canvas
    {
        const string DELETE_TITLE = "Удаление соединения между {0} и {1}";
        const string DELETE_TEXT = "Вы точно хотите удалить соединение между {0} и {1}?";

        public static readonly DependencyProperty FirstDeviceViewProperty = DependencyProperty.Register(
            nameof(FirstDeviceView),
            typeof(DeviceView),
            typeof(ConnectionView)
        );
        public DeviceView FirstDeviceView
        {
            get => (DeviceView)GetValue(FirstDeviceViewProperty);
            set => SetValue(FirstDeviceViewProperty, value);
        }
        public static readonly DependencyProperty SecondDeviceViewProperty = DependencyProperty.Register(
            nameof(SecondDeviceView),
            typeof(DeviceView),
            typeof(ConnectionView)
        );
        public DeviceView SecondDeviceView
        {
            get => (DeviceView)GetValue(SecondDeviceViewProperty);
            set => SetValue(SecondDeviceViewProperty, value);
        }
        public ConnectionView()
        {
            InitializeComponent();
        }

        void line_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var valid_sender = ((Line)sender);
            valid_sender.Focus();
            e.Handled = true;
        }
        void line_KeyUp(object sender, KeyEventArgs e)
        {
            var valid_sender = ((Line)sender);
            if (e.Key == Key.Delete
                && MessageBox.Show(
                    string.Format(DELETE_TEXT, FirstDeviceView.ViewModel.Model.Name, SecondDeviceView.ViewModel.Model.Name),
                    string.Format(DELETE_TITLE, FirstDeviceView.ViewModel.Model.Name, SecondDeviceView.ViewModel.Model.Name),
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes
            )
                FirstDeviceView.ViewModel.Model.Disconnect(SecondDeviceView.ViewModel.Model);
            e.Handled = true;
        }
    }
}
