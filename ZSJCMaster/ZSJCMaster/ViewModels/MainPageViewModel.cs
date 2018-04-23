﻿using Prism.Commands;
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
        private List<Camera> cameras;

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
        public DelegateCommand RemoteDesktopCommand { get; set; }
        #endregion
        //RoutedPropertyChangedEventArgs<object>
        #region command functions
        private void PageLoaded(ExCommandParameter param)
        {
            var sender = param.Sender as UserControl;
            var tvControlPads = sender.FindName("tvControlPads") as TreeView;
            var cboControlPads = sender.FindName("cboControlPads") as ComboBox;
            this.ControlPads = ControlPad.GetAllControlPads();
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
                pad.Cameras = pad.GetCameras(pad.Id);
            }
            else if (sender.SelectedItem is Camera)
            {
                if (param.EventArgs is MouseButtonEventArgs)
                {
                    //只有在选中相机并单击右键时，才显示快捷菜单
                    if (sender.SelectedItem == null) { return; }
                    var contextMenu = new ContextMenu();
                    contextMenu.Items.Add(new MenuItem { Header = "连接", Command = this.RemoteDesktopCommand });
                    contextMenu.Items.Add(new MenuItem { Header = "开启检测" });
                    contextMenu.IsOpen = true;
                }
            }

        }

        private void RemoteDesktop()
        {
            try
            {
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
            this.RemoteDesktopCommand = new DelegateCommand(RemoteDesktop);
        }
    }
}