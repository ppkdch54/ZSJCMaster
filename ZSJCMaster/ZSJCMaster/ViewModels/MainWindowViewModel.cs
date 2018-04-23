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
    }
}
