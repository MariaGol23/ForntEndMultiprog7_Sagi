using LKDSFramework;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForntEndMultiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageConnect.xaml
    /// </summary>
    public partial class PageConnect : Page
    {
        string FlagCloud = "-cloud", FlagLU = "-lu", FlagPass = "-pass";

        List<string> ParamsList = new List<string>();
        public PageConnect()
        {
            InitializeComponent();
        }

        private void BtnConnectLB_Click(object sender, RoutedEventArgs e)
        {
            if (ChBCloudUse.IsChecked.Equals(true))
            {
                ParamsList.Add(FlagCloud);
            }
            if(TBLU.Text.Length > 0)
            {
                ParamsList.Add(FlagLU + TBLU.Text);
            }
            if(TBPass.Text.Length > 0)
            {
                ParamsList.Add(FlagPass + TBPass.Text);
            }
            string[] ParamsToConnect = new string[ParamsList.Count];
            for (int i = 0; i < ParamsList.Count; i++)
            {
                ParamsToConnect[i] = ParamsList[i];
            }
            App.Args = ParamsToConnect;
            NavigationService.Navigate(new Pages.PageMain());
        }
    }
}
