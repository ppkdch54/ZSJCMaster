using FirstFloor.ModernUI.Windows.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using ZSJCMaster.DB;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class MainPageViewModel: MainWindowViewModel
    {
        private ControlPad currentPad;  //当前选中的控制板
        private string copyImagePath;
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

            XDocument doc = XDocument.Load("Application.config");
            var path = doc.Descendants("copyImagePath").Single();
            this.copyImagePath = path.Attribute("path").Value;
        }
        private void ComboBoxSelectChanged(ExCommandParameter param)
        {
            var sender = param.Sender as ComboBox;
            var pad = sender.SelectedItem as ControlPad;
            if (pad == null) { return; }
            this.Cameras = pad.GetCameras();
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
                pad.Cameras = pad.GetCameras();
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
                    menuItem.Tag = (sender.SelectedItem as Camera).Id;
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
            var camera = sender.SelectedItem as Camera;
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
            int no = camera.NetPortNum;
            try
            {
                ControlPad pad = new ControlPad(camera.ControlPadNo,null);
                pad.SwitchNetPort(no);
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke(()=> 
                {
                    camera.IsSwitching = false;
                    ModernDialog.ShowMessage(ex.Message, "提示", MessageBoxButton.OK);
                });
                
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
                    camera = this.Cameras.SingleOrDefault(c => c.Id == no);
                }
                else if(param.Sender is MenuItem)
                {
                    var sender = param.Sender as MenuItem;
                    no = int.Parse(sender.Tag.ToString());
                    if (this.currentPad == null) { return; }
                    camera = this.currentPad.Cameras.SingleOrDefault(c => c.Id == no);         
                }
                if (camera == null) { return; }
                Task.Run(()=> 
                {
                    camera.IsSwitching = true;
                    SwitchCameraNetPort(camera);
                    System.Threading.Thread.Sleep(3000);
                    camera.IsSwitching = false;
                    
                    Task.Run(() =>
                    {
                        try
                        {
                            //从下位机获取图片
                            string root = "\\\\" + camera.IP + "\\Alarm_Pic";
                            DateTime now = DateTime.Now;
                            DateTime yesterday = now.AddDays(-1);
                            yesterday = now;
                            string picDir = Path.Combine(root, "pic_" + yesterday.ToString("yyyy_MM_dd"));
                            using (SharedTool tool = new SharedTool("Admin", "",camera.IP))
                            {
                                //采集图片
                                camera.IsDownloadingImage = true;
                                string[] dirs = Directory.GetDirectories(picDir);
                                foreach (var dir in dirs)
                                {
                                    if(Path.GetFileName(dir).StartsWith("下位机")&& dir.EndsWith("报警截图"))
                                    {
                                        camera.IsDownloadingImage = true;
                                        while (true)
                                        {
                                            string[] images = Directory.GetFiles(dir, "*.jpg");
                                            foreach (var img in images)
                                            {
                                                string parentDir = Path.GetDirectoryName(img);
                                                string rootDir = Path.GetPathRoot(parentDir);
                                                picDir = parentDir.Substring(parentDir.IndexOf(rootDir) + 1 + rootDir.Length);
                                                string localPath = Path.Combine(this.copyImagePath, picDir);
                                                if (!Directory.Exists(localPath))
                                                {
                                                    Directory.CreateDirectory(localPath);
                                                }
                                                string localImg = Path.Combine(localPath, Path.GetFileName(img));
                                                if (!File.Exists(localImg))
                                                {
                                                    File.Copy(img, localImg);
                                                }

                                            }
                                            Thread.Sleep(2000);
                                        }
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                ModernDialog.ShowMessage(ex.Message+",图片采集已中止！", "提示", MessageBoxButton.OK);
                            });

                        }
                        finally
                        {
                            camera.IsDownloadingImage = false;
                        }
                        
                    });

                    //启动远程桌面,使用mstsc命令读取rdp文件
                    string rdpFile = camera.Name + ".rdp";
                    if (!File.Exists(rdpFile))
                    {
                        ExecuteCmd($"mstsc /admin");
                    }
                    else
                    {
                        ExecuteCmd($"mstsc /admin \"{rdpFile}\"");
                    }
                   
                    
                });
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ModernDialog.ShowMessage(ex.Message, "提示", MessageBoxButton.OK);
                });
            }

        }

        public string ExecuteCmd(params string[] cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.AutoFlush = true;
            for (int i = 0; i < cmd.Length; i++)
            {
                p.StandardInput.WriteLine(cmd[i].ToString());
            }
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return strRst;
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
