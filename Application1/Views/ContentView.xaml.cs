using Application1.Models;
using System.Windows;
using System.Windows.Controls;

namespace Application1.Views
{
    public partial class ContentView : ScrollViewer
    {
        public ContentView()
        {
            InitializeComponent();
        }

        void PC_Click(object sender, RoutedEventArgs e)
        {
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
