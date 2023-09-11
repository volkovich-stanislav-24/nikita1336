using Application1.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Application1.Views
{
    public partial class DeviceTypeView : Grid
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(DeviceTypeViewModel),
            typeof(DeviceTypeView)
        );
        public DeviceTypeViewModel ViewModel
        {
            get => (DeviceTypeViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public DeviceTypeView()
            => InitializeComponent();
    }
}
