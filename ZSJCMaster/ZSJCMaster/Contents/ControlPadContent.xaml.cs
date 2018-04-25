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

namespace ZSJCMaster.Contents
{
    /// <summary>
    /// Interaction logic for ControlPadContent.xaml
    /// </summary>
    public partial class ControlPadContent : UserControl
    {
        public ControlPadContent()
        {
            InitializeComponent();
            this.DataContext = new ControlPadContentViewModel();
        }
    }
}
