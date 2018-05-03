using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;
using ZSJCMaster.Views;

namespace ZSJCMaster.ViewModels
{
    class MainWindowViewModel: BindableBase
    {
        private ObservableCollection<ControlPad> controlPads;

        /// <summary>
        /// 控制板集合
        /// </summary>
        public ObservableCollection<ControlPad> ControlPads
        {
            get { return controlPads; }
            set
            {
                controlPads = value;
                this.RaisePropertyChanged("ControlPads");
            }
        }

        public MainWindowViewModel()
        {
            User user = User.GetInstance();
            if(string.IsNullOrEmpty(user.UserName))
            {
                LoginWindow login = new LoginWindow();
                login.ShowDialog();
                //重新检测
                user = User.GetInstance();
                if(string.IsNullOrEmpty(user.UserName))
                {
                    App.Current.Shutdown();
                }
            }
        }
    }
}
