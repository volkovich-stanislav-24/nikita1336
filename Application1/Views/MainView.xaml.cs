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

        void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

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
                __connection_line.IsHitTestVisible = false;
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
            e.Handled = true;
        }

        public MainView()
        {
            InitializeComponent();
            Device.OnCreated += (d) =>
            {
                var dv = new DeviceView();
                Canvas.SetLeft(dv, __new_device_view_point.X - dv.Width * .5);
                Canvas.SetTop(dv, __new_device_view_point.Y - dv.Height * .5);
                dv.ViewModel = DeviceViewModel.One(d);
                Children.Add(dv);
            };
            Device.OnConnect += (d1, d2) => {
                var cv = new ConnectionView();
                cv.FirstDeviceView = __DeviceView(DeviceViewModel.One(d1));
                cv.SecondDeviceView = __DeviceView(DeviceViewModel.One(d2));
                Children.Add(cv);
            };
            Device.OnDisconnect += (d1, d2) => {
                Children.Remove(__ConnectionView(__DeviceView(DeviceViewModel.One(d1)), __DeviceView(DeviceViewModel.One(d2))));
            };
            Device.OnDeleted += (d) => {
                Children.Remove(__DeviceView(DeviceViewModel.One(d)));
            };

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

        Point __new_device_view_point;

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            __new_device_view_point = e.GetPosition(this);
            string name = "New PC";
            string id = "";
            long id_as_long = 1;
            do
            {
                try
                {
                    new PC(name + id);
                    e.Handled = true;
                    return;
                }
                catch (PC.NameError)
                {
                    id = id_as_long.ToString();
                    ++id_as_long;
                }
            } while (true);
        }
    }
}
