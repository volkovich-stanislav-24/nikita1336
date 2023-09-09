using Application1.Models;
using Application1.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Application1.Views
{
    public partial class MainView : Canvas
    {
        IReadOnlyList<DeviceView> __DeviceViews
        {
            get
            {
                List<DeviceView> to_return = new(Children.Count);
                foreach (UIElement child in Children)
                    if (child is DeviceView)
                        to_return.Add((DeviceView)child);
                return to_return.AsReadOnly();
            }
        }
        DeviceView? __DeviceView(DeviceViewModel view_model)
        {
            foreach (var device_view in __DeviceViews)
                if (device_view.ViewModel == view_model)
                    return device_view;
            return null;
        }
        IReadOnlyList<ConnectionView> __ConnectionViews
        {
            get
            {
                List<ConnectionView> to_return = new(Children.Count);
                foreach (UIElement child in Children)
                    if (child is ConnectionView)
                        to_return.Add((ConnectionView)child);
                return to_return.AsReadOnly();
            }
        }
        ConnectionView? __ConnectionView(DeviceView source, DeviceView destination)
        {
            foreach (var connection_view in __ConnectionViews)
                if (
                    connection_view.FirstDeviceView == source || connection_view.SecondDeviceView == destination
                    || connection_view.FirstDeviceView == destination || connection_view.SecondDeviceView == source
                )
                    return connection_view;
            return null;
        }

        public DeviceView? is_trying_connect;
        Line? __connection_line;

        public void EndConnect(DeviceView destination)
        {
            Children.Remove(__connection_line);
            __connection_line = null;
            var source = is_trying_connect;
            is_trying_connect = null;
            source.ViewModel.Model.Connect(destination.ViewModel.Model);
        }

        void Canvas_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (__connection_line != null)
            {
                Children.Remove(__connection_line);
                __connection_line = null;
                is_trying_connect = null;
            }
        }

        void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (is_trying_connect != null && __connection_line == null)
            {
                __connection_line = new Line();
                __connection_line.Stroke = new SolidColorBrush(Colors.Yellow);
                __connection_line.StrokeThickness = 2;
                __connection_line.X1 = Canvas.GetLeft(is_trying_connect) + is_trying_connect.ActualWidth * .5;
                __connection_line.Y1 = Canvas.GetTop(is_trying_connect) + is_trying_connect.ActualHeight;
                var mouse_position = e.GetPosition(this);
                __connection_line.X2 = mouse_position.X;
                __connection_line.Y2 = mouse_position.Y;
                Children.Add(__connection_line);
            }
            else if (__connection_line != null)
            {
                var mouse_position = e.GetPosition(this);
                __connection_line.X2 = mouse_position.X;
                __connection_line.Y2 = mouse_position.Y;
            }
        }

        public MainView()
        {
            InitializeComponent();
            Device.OnConnect += (d1, d2) => {
                var cv = new ConnectionView();
                cv.FirstDeviceView = __DeviceView(DeviceViewModel.One(d1));
                cv.SecondDeviceView = __DeviceView(DeviceViewModel.One(d2));
                Children.Add(cv);
            };
            Device.OnDisconnect += (d1, d2) => {
                Children.Remove(__ConnectionView(__DeviceView(DeviceViewModel.One(d1)), __DeviceView(DeviceViewModel.One(d2))));
            };
            Device.OnDelete += (d) => {
                Children.Remove(__DeviceView(DeviceViewModel.One(d)));
            };

            // Test

            var d1 = new PC("PC 1");
            d1.MaxConnections = 8;
            DeviceView1.ViewModel = new DeviceViewModel(d1);
            var d2 = new PC("PC 2");
            d2.MaxConnections = 8;
            d2.MaxConnections = 16;
            DeviceView2.ViewModel = new DeviceViewModel(d2);
            d2.MaxConnections = 32;
            d1.Connect(d2);

            var d3 = new PC("PC 3");
            d3.MaxConnections = 8;
            DeviceView3.ViewModel = new DeviceViewModel(d3);
            var d4 = new PC("PC 4");
            d4.MaxConnections = 8;
            d4.MaxConnections = 16;
            DeviceView4.ViewModel = new DeviceViewModel(d4);
            d4.MaxConnections = 32;
            d3.Connect(d2);

            // Test

            /*foreach (var device_view in __DeviceViews)
            {
                foreach (var connected_device in device_view.ViewModel.Model.Connections)
                {
                    var connection_view = __ConnectionView(__DeviceView(DeviceViewModel.One(connected_device)));
                    if (connection_view == null)
                    {
                        connection_view = new ConnectionView();
                        Children.Add(connection_view);
                        connection_view.FirstDeviceView = device_view;
                        break;
                    }
                    else
                    {
                        connection_view.SecondDeviceView = device_view;
                        break;
                    }
                }
            }*/
        }

        void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }
    }
}
