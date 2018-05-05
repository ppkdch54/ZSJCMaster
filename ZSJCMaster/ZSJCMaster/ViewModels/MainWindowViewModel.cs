using FirstFloor.ModernUI.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
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
using System.Windows.Media;
using System.Xml.Linq;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;
using ZSJCMaster.Views;

namespace ZSJCMaster.ViewModels
{
    class MainWindowViewModel: BindableBase
    {
        TaskbarIcon icon;
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

        private ObservableCollection<Camera> cameras;
        /// <summary>
        /// 相机集合
        /// </summary>
        public ObservableCollection<Camera> Cameras
        {
            get { return cameras; }
            set
            {
                cameras = value;
                this.RaisePropertyChanged("Cameras");
            }
        }

        public DelegateCommand<ExCommandParameter> WindowLoadedCommand { get; set; }
        public DelegateCommand ShowCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        private void WindowLoaded(ExCommandParameter param)
        {
            User user = User.GetInstance();
            if (string.IsNullOrEmpty(user.UserName))
            {
                LoginWindow login = new LoginWindow();
                login.ShowDialog();
                //重新检测
                user = User.GetInstance();
                if (string.IsNullOrEmpty(user.UserName))
                {
                    Environment.Exit(0);
                }else
                {
                    icon = new TaskbarIcon();
                    icon.Visibility = Visibility.Visible;
                    icon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "/Icons/ddckUI.ico");
                    icon.ToolTipText = (param.Sender as Window).Title;
                    icon.MenuActivation = PopupActivationMode.RightClick;
                    icon.PopupActivation = PopupActivationMode.DoubleClick;
                    ContextMenu menu = new ContextMenu();
                    menu.Items.Add(new MenuItem() { Header="显示",Command=ShowCommand });
                    menu.Items.Add(new MenuItem() { Header="退出",Command=ExitCommand });
                    icon.ContextMenu = menu;
                    
                }
            }
        }

        private void Show()
        {
            App.Current.MainWindow.Show();
            App.Current.MainWindow.ShowInTaskbar = true;
        }
        private void Exit()
        {
            if (ModernDialog.ShowMessage("确定要退出吗?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                icon.Visibility = Visibility.Hidden;
                Environment.Exit(0);
            }
        }
        public MainWindowViewModel()
        {
            
            this.WindowLoadedCommand = new DelegateCommand<ExCommandParameter>(WindowLoaded);
            this.ShowCommand = new DelegateCommand(Show);
            this.ExitCommand = new DelegateCommand(Exit);
        }
    }
}
