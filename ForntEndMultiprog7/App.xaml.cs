using LKDSFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ForntEndMultiprog7
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] Args = null;
        static bool SimpleFlag = false;
        [STAThread]
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            Args = e.Args;
            wnd.Show();
            // запускать разные окна (наверное) в зависимости от набора аргументов
        }
    }
}
