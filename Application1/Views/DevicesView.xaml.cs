using Application1.Models;
using Application1.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace Application1.Views
{
    public partial class DevicesView : UserControl
    {
        IReadOnlyList<DeviceView> __DeviceViews
        {
            get
            {
                List<DeviceView> to_return = new(canvas.Children.Count);
                foreach (UIElement child in canvas.Children)
                    if (child is DeviceView)
                        to_return.Add((DeviceView)child);
                return to_return;
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
                List<ConnectionView> to_return = new(canvas.Children.Count);
                foreach (UIElement child in canvas.Children)
                    if (child is ConnectionView)
                        to_return.Add((ConnectionView)child);
                return to_return;
            }
        }
        ConnectionView? __ConnectionView(DeviceView source, DeviceView destination)
        {
            foreach (var connection_view in __ConnectionViews)
                if (
                    connection_view.SourceDeviceView == source || connection_view.DestinationDeviceView == destination
                    || connection_view.SourceDeviceView == destination || connection_view.DestinationDeviceView == source
                )
                    return connection_view;
            return null;
        }

        public DevicesView()
        {
            InitializeComponent();
            Device.OnCreated += (d) =>
            {
                var dv = new DeviceView();
                dv.ViewModel = DeviceViewModel.One(d);
                Canvas.SetLeft(dv, __new_device_view_point.X - dv.Width * .5);
                Canvas.SetTop(dv, __new_device_view_point.Y - dv.Height * .5);
                canvas.Children.Add(dv);
            };
            Device.OnDeleted += (d) => {
                canvas.Children.Remove(__DeviceView(DeviceViewModel.One(d)));
            };
            Device.OnConnect += (d1, d2) => {
                var cv = new ConnectionView();
                cv.SourceDeviceView = __DeviceView(DeviceViewModel.One(d1));
                cv.DestinationDeviceView = __DeviceView(DeviceViewModel.One(d2));
                canvas.Children.Add(cv);
            };
            Device.OnDisconnect += (d1, d2) => {
                canvas.Children.Remove(__ConnectionView(__DeviceView(DeviceViewModel.One(d1)), __DeviceView(DeviceViewModel.One(d2))));
            };
        }

        Point __new_device_view_point;

        void canvas_Drop(object sender, DragEventArgs e)
        {
            var dtv = (DeviceTypeView)e.Data.GetData(typeof(DeviceTypeView));
            __new_device_view_point = e.GetPosition(this);
            var name = $"New {dtv.ViewModel.Model.Name} ";
            var id = 1;
            do
            {
                try
                {
                    Activator.CreateInstance(
                        dtv.ViewModel.Model,
                        name + id
                    );
                    e.Handled = true;
                    return;
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    ++id;
                }
            } while (true);
        }

        DeviceView? __source;
        Line? __connection_line;

        public void BeginConect(DeviceView source)
        {
            __source = source;
        }

        public bool EndConnect(DeviceView destination)
        {
            if (__source != null)
            {
                canvas.Children.Remove(__connection_line);
                __connection_line = null;
                try
                {
                    __source.ViewModel.Model.Connect(destination.ViewModel.Model);
                    
                }
                catch (Device.ConnectionError error)
                {
                    var error_view = new ToolTip();
                    error_view.Content = error.Message;
                    destination.ToolTip = error_view;
                    error_view.StaysOpen = false;
                    error_view.IsOpen = true;
                }
                __source = null;
                return true;
            }
            return false;
        }

        void canvas_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            if (__connection_line != null)
            {
                canvas.Children.Remove(__connection_line);
                __connection_line = null;
                __source = null;
            }
        }

        void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (__source != null && __connection_line == null)
            {
                __connection_line = new Line();
                __connection_line.IsHitTestVisible = false;
                __connection_line.Stroke = (Brush)Resources["ConnectionLineColor"];
                __connection_line.StrokeThickness = (double)Resources["ConnectionLineSize"];
                __connection_line.X1 = Canvas.GetLeft(__source) + __source.ActualWidth * .5;
                __connection_line.Y1 = Canvas.GetTop(__source) + __source.ActualHeight;
                var mouse_position = e.GetPosition(this);
                __connection_line.X2 = mouse_position.X;
                __connection_line.Y2 = mouse_position.Y;
                canvas.Children.Add(__connection_line);
                e.Handled = true;
            }
            else if (__connection_line != null)
            {
                var mouse_position = e.GetPosition(this);
                __connection_line.X2 = mouse_position.X;
                __connection_line.Y2 = mouse_position.Y;
                e.Handled = true;
            }
        }
    }
}
