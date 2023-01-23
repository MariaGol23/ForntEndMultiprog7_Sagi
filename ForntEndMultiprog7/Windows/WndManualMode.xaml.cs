using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FrontEndMultiprog7.Windows
{
    /// <summary>
    /// Логика взаимодействия для WndManualMode.xaml
    /// </summary>
    public partial class WndManualMode : Window
    {
        public WndManualMode()
        {
            InitializeComponent();
        }

        private void BtnMinimezeBox_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnCloseBox_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
