using Application1.Models;
using Application1.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Application1.Converters
{
    sealed class ContentViewDeviceTypeViewHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (double)value * .25;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

namespace Application1.Views
{
    public partial class ContentView : ScrollViewer
    {
        public ContentView()
        {
            Resources["PCTypeViewModel"] = DeviceTypeViewModel.One(typeof(PC));
            Resources["SwitchTypeViewModel"] = DeviceTypeViewModel.One(typeof(Switch));
            InitializeComponent();
        }

        // Device Type View

        void DeviceTypeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragDrop.DoDragDrop(this, sender, DragDropEffects.Copy);
    }
}
