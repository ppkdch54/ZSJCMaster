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
    class MainPageViewModel: MainWindowViewModel
    {
        private ControlPad currentPad;  //当前选中的控制板
        private List<Camera> cameras;
        private Camera camera;
        /// <summary>
        /// 相机集合
        /// </summary>
        public List<Camera> Cameras
        {
            get { return cameras; }
            set
            {
                cameras = value;
                this.RaisePropertyChanged("Cameras");
            }
        }

        #region commands
        public DelegateCommand<ExCommandParameter> PageLoadedCommand { get; set; }
        public DelegateCommand<ExCommandParameter> ComboBoxSelectChangedCommand { get; set; }
        public DelegateCommand<ExCommandParameter> TreeViewSelectChangedCommand { get; set; }
        public DelegateCommand<ExCommandParameter> ListBoxSelectChangedCommand { get; set; }
        public DelegateCommand<ExCommandParameter> RemoteDesktopCommand { get; set; }
        #endregion
        //RoutedPropertyChangedEventArgs<object>
        #region command functions
        private void PageLoaded(ExCommandParameter param)
        {
            var sender = param.Sender as UserControl;
            var tvControlPads = sender.FindName("tvControlPads") as TreeView;
            var cboControlPads = sender.FindName("cboControlPads") as ComboBox;
            this.ControlPads = ControlPad.GetAllControlPads();
            this.Cameras = null;
        }
        private void ComboBoxSelectChanged(ExCommandParameter param)
        {
            var sender = param.Sender as ComboBox;
            var pad = sender.SelectedItem as ControlPad;
            if (pad == null) { return; }
            this.Cameras = pad.GetCameras(pad.Id);
        }
        private void TreeViewSelectChanged(ExCommandParameter param)
        {
            var sender = param.Sender as TreeView;
            if (sender.SelectedItem is ControlPad)
            {
                //屏蔽控制板的右键事件
                if (param.EventArgs is MouseButtonEventArgs)
                {
                    return;
                }
                var pad = sender.SelectedItem as ControlPad;
                this.currentPad = pad;
                pad.Cameras = pad.GetCameras(pad.Id);
            }
            else if (sender.SelectedItem is Camera)
            {
                if (param.EventArgs is MouseButtonEventArgs)
                {
                    //只有在选中相机并单击右键时，才显示快捷菜单
                    if (sender.SelectedItem == null) { return; }
                    //显示右键菜单
                    var contextMenu = new ContextMenu();
                    //contextMenu.Items.Add(new MenuItem { Header = "连接", Command = this.RemoteDesktopCommand });
                    var menuItem = new MenuItem { Header = "连接" };
                    menuItem.Name = "menuConnect";
                    menuItem.Tag = (sender.SelectedItem as Camera).No;
                    menuItem.Command = this.RemoteDesktopCommand;
                    var parameter = new ExCommandParameter();
                    parameter.Sender = menuItem;
                    menuItem.CommandParameter = parameter;
                    contextMenu.Items.Add(menuItem);

                    contextMenu.Items.Add(new MenuItem { Header = "开启检测" });
                    contextMenu.IsOpen = true;
                }
            }

        }

        
        private void ListBoxSelectChanged(ExCommandParameter param)
        {
            var sender = param.Sender as ListBox;
            var args = param.EventArgs as SelectionChangedEventArgs;
            camera = sender.SelectedItem as Camera;
            if (camera!=null)
            {
                SwitchCameraNetPort(camera);
            }
            
        }

        /// <summary>
        /// 切换相机网口
        /// </summary>
        /// <param name="camera"></param>
        private void SwitchCameraNetPort(Camera camera)
        {
            int no = camera.No;
            try
            {
                ControlPad pad = new ControlPad(camera.ControlPadNo);
                pad.SwitchNetPort(no);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private void RemoteDesktop(ExCommandParameter param)
        {
            try
            {   //先切换网口
                int no = 0;
                Camera camera = null;
                if(param.Sender is Button)
                {
                    var sender = param.Sender as Button;
                    no = int.Parse(sender.Tag.ToString());
                    camera = this.Cameras.SingleOrDefault(c => c.No == no);
                }
                else if(param.Sender is MenuItem)
                {
                    var sender = param.Sender as MenuItem;
                    no = int.Parse(sender.Tag.ToString());
                    if (this.currentPad == null) { return; }
                    camera = this.currentPad.Cameras.SingleOrDefault(c => c.No == no);
                    
                }
                if (camera == null) { return; }
                SwitchCameraNetPort(camera);
                //启动远程桌面
                Process p = Process.Start("mstsc.exe");
                p.WaitForExit();//关键，等待外部程序退出后才能往下执行
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion command functions

        public MainPageViewModel()
        {
            this.PageLoadedCommand = new DelegateCommand<ExCommandParameter>(PageLoaded);
            this.ComboBoxSelectChangedCommand = new DelegateCommand<ExCommandParameter>(ComboBoxSelectChanged);
            this.TreeViewSelectChangedCommand = new DelegateCommand<ExCommandParameter>(TreeViewSelectChanged);
            this.ListBoxSelectChangedCommand = new DelegateCommand<ExCommandParameter>(ListBoxSelectChanged);
            this.RemoteDesktopCommand = new DelegateCommand<ExCommandParameter>(RemoteDesktop);
        }
    }
}
