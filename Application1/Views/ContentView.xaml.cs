using Application1.Models;
using Application1.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Application1.Converters
{
    public sealed class ContentViewDeviceTypeViewHeightConverter
    {

    }
}

namespace Application1.Views
{
    public partial class ContentView : ScrollViewer
    {
        public ContentView()
        {
            Resources["PCTypeViewModel"] = DeviceTypeViewModel.One(typeof(PC));
            InitializeComponent();
        }

        // Device Type View

        void __DeviceTypeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop(this, sender, DragDropEffects.Copy);
        }
    }
}
