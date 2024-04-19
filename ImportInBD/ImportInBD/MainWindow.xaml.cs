using ImportInBD.Properties;
using ImportInBD.ContolEntities;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Controls;
using ImportInBD.Entities;
using System.Windows.Controls.Primitives;



namespace ImportInBD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int index;//удалить
        public MainWindow()
        {

            Thread.Sleep(1000);

            InitializeComponent();

        }

        private void DgAccountingByOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Width = Properties.Settings.Default.Width;
            this.Height = Properties.Settings.Default.Height;

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }

        private void dgAccountingByOrders_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            ComboBox box = (ComboBox)dataGrid.CurrentCell.Column.GetCellContent(e.Item);
            index = box.SelectedIndex;
        }

        private void dgAccountingByOrders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                DataGrid dataGrid = (DataGrid)sender;
                ComboBox box = (ComboBox)dataGrid.CurrentCell.Column.GetCellContent(dataGrid.CurrentCell.Item);
                box.SelectedIndex = index;
            }
        }
    }

}
