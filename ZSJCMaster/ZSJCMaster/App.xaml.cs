using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ZSJCMaster
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "ZSJCMaster", out ret);

            if (!ret)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ModernDialog.ShowMessage("程序已经在运行!", "提示", MessageBoxButton.OK);
                });
                Environment.Exit(0);
            }
        }
    }
}
