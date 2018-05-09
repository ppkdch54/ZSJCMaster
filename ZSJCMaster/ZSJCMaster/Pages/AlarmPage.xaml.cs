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
using ZSJCMaster.ViewModels;

namespace ZSJCMaster.Pages
{
    /// <summary>
    /// Interaction logic for AlarmPage.xaml
    /// </summary>
    public partial class AlarmPage : UserControl
    {
        public AlarmPage()
        {
            InitializeComponent();
            this.DataContext = new AlarmPageViewModel();
        }

        private void dgvControlPads_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if(dg.Items.Count > 0)
            {
                dg.ScrollIntoView(dg.Items[dg.Items.Count - 1]);
            }
        }
    }
}
