using FirstFloor.ModernUI.Windows.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;
using ZSJCMaster.DB;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class AlarmPageViewModel : MainWindowViewModel
    {
        private AlarmLamp alarmLamp;
        private AlarmInfoOperator ope;
        private AlarmInfo currentItem;

        /// <summary>
        /// 表格当前项
        /// </summary>
        public AlarmInfo CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                this.RaisePropertyChanged("CurrentItem");
            }
        }

        private ObservableCollection<AlarmInfo> alarmInfos;

        /// <summary>
        /// 报警信息集合
        /// </summary>
        public ObservableCollection<AlarmInfo> AlarmInfos
        {
            get { return alarmInfos; }
            set
            {
                alarmInfos = value;
                this.RaisePropertyChanged("AlarmInfos");
            }
        }

        public DelegateCommand ClearAlarmListCommand { get; set; }
        private void ClearAlamList()
        {
            this.AlarmInfos.Clear();
        }
        public AlarmPageViewModel()
        {
            alarmLamp = new AlarmLamp();
            ope = new AlarmInfoOperator();
            ClearAlarmListCommand = new DelegateCommand(ClearAlamList);
            this.AlarmInfos = new ObservableCollection<AlarmInfo>();
            if (App.Current == null) { return; }
            if(this.ControlPads == null)
            {
                this.ControlPads = ControlPad.GetAllControlPads();
            }
            //此处需判断是不是服务器
            Task.Run(()=> 
            {
                //获取服务器
                XDocument doc = XDocument.Load("Application.config");
                var server = doc.Descendants("server").Single();
                string serverIP = server.Attribute("ip").Value;
                int serverPort = int.Parse(server.Attribute("port").Value);
                //获取客户机
                var clients = doc.Descendants("clients").Descendants("client");
                //获取本机IPv4地址
                string name = Dns.GetHostName();
                IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
                //判断是否包含服务器的IP地址
                bool b = ipadrlist.Contains(IPAddress.Parse(serverIP));
                if (b)
                {
                    //如果是服务器才向控制板发送数据
                    foreach (var controlpad in this.ControlPads)
                    {
                        this.Cameras = controlpad.GetCameras();
                        try
                        {
                            ControlPad pad = new ControlPad(controlpad.Id, (AlarmInfo[] info, bool[] flags) =>
                            {
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    for (int i = 0; i < flags.Length; i++)
                                    {
                                        if (!IsEmpty(info[i]))
                                        {
                                            if (controlpad != null)
                                            {
                                                var camera = this.Cameras.SingleOrDefault(c => c.Id == info[i].CameraNo);
                                                if (camera != null)
                                                {
                                                    info[i].CameraName = camera.Name;
                                                }

                                            }
                                            AlarmInfos.Insert(0,info[i]);
                                            //始终保持界面上的报警记录不得多于50条
                                            if(AlarmInfos.Count == 50+1)
                                            {
                                                AlarmInfos.RemoveAt(AlarmInfos.Count - 1);
                                            }
                                            //向报警器串口发送命令
                                            Task.Run(() =>
                                                {
                                                    alarmLamp.AlarmMusicAndFlash();
                                                    Thread.Sleep(2000);
                                                    alarmLamp.StopAllAlarm();
                                                });

                                            CurrentItem = info[i];

                                            //向其他客户机发送报警数据
                                            if (clients.Count() > 0)
                                            {
                                                foreach (var client in clients)
                                                {
                                                    string ip = client.Attribute("ip").Value;
                                                    int port = int.Parse(client.Attribute("port").Value);
                                                    if (ip != serverIP)
                                                    {
                                                        UdpComm comm = new UdpComm(ip, port);
                                                        XmlSerializer xs = new XmlSerializer(typeof(AlarmInfo));
                                                        var ms = new MemoryStream();
                                                        xs.Serialize(ms, CurrentItem);
                                                        comm.SendData(ms.ToArray());
                                                        ms.Close();
                                                        ms.Dispose();
                                                    }
                                                }
                                            }
                                            //写入数据库

                                            ope.Add(ope.AlarmInfoToAlarmInfoForDB(controlpad.Id, info[i]));
                                            
                                        }
                                    }
                                });
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
                }
                else
                {
                    //如果不是服务器，开启线程来接受服务器发来的报警信息
                    Task.Run(()=> 
                    {
                        //先找到本机的IP和端口
       
                        foreach (var client in clients)
                        {
                            string ip = client.Attribute("ip").Value;
                            int port = int.Parse(client.Attribute("port").Value);
                            if (ipadrlist.Contains(IPAddress.Parse(ip)))
                            {
                                //找到本机
                                Task.Run(() => 
                                {
                                    UdpComm comm = new UdpComm(port);
                                    XmlSerializer xs = new XmlSerializer(typeof(AlarmInfo));
                                    while (true)
                                    {
                                        byte[] data = comm.Receive(serverIP, serverPort);
                                        App.Current.Dispatcher.Invoke(() =>
                                        {
                                            using (var ms = new MemoryStream())
                                            {
                                                ms.Write(data, 0, data.Length);
                                                ms.Position = 0;
                                                AlarmInfos.Add(xs.Deserialize(ms) as AlarmInfo);
                                            }
                                        });
                                    }
                                    
                                });
                                break;
                            }
                        }
                       
                        
                    });
                }
            });

        }

        private bool IsEmpty(AlarmInfo alarmInfo)
        {
            if (alarmInfo.CameraNo != 0 || alarmInfo.X != 0 || alarmInfo.Y != 0 || alarmInfo.Width != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    
}
