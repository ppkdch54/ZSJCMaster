using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Xml.Linq;
using System.Linq;
using ZSJCMaster.Helpers;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using FirstFloor.ModernUI.Windows.Controls;

namespace ZSJCMaster.Models
{
    public class ControlPad: BindableBase
    {
        TcpComm tcpComm;
        byte[] command;

        #region 属性
        private int id;

        /// <summary>
        /// 控制板编号
        /// </summary>
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                this.RaisePropertyChanged("Id");
            }
        }

        private string name;

        /// <summary>
        /// 控制板名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        private string ip;

        /// <summary>
        /// 控制板IP
        /// </summary>
        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                this.RaisePropertyChanged("IP");
            }
        }

        private int portNum;

        /// <summary>
        /// 控制板TCP端口
        /// </summary>
        public int PortNum
        {
            get { return portNum; }
            set
            {
                portNum = value;
                this.RaisePropertyChanged("PortNum");
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
        #endregion

        public ControlPad() { }

        //构造函数,打开串口
        public ControlPad(int controlpadId,TcpRecvDelegate tcpRecv)
        {
            try
            {
                //读取参数
                LoadPara(controlpadId);
                //初始化TCP连接
                tcpComm = new TcpComm(IP, PortNum, tcpRecv);
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        tcpComm.SendData(new byte[] { 0x89, 0x00, 0x0a });
                    }
                });
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        #region 操作配置文件
        public void LoadPara(int padId = 1)
        {
            XDocument doc = XDocument.Load("Application.config");
            var controlpads = doc.Descendants("controlpads").SingleOrDefault();
            if (controlpads == null) { return; }
            var controlpad = controlpads.Descendants("controlpad").SingleOrDefault(p=>p.Attribute("id").Value == padId.ToString());
            if (controlpad == null) { return; }
            this.Id = int.Parse(controlpad.Attribute("id").Value);
            this.Name = controlpad.Attribute("name").Value;
            this.IP = controlpad.Attribute("ip").Value;
            this.PortNum = int.Parse(controlpad.Attribute("port").Value);
        }

        /// <summary>
        /// 从配置文件读取指定编号的控制板的所有相机信息
        /// </summary>
        /// <returns>相机集合</returns>
        public ObservableCollection<Camera> GetCameras()
        {
            XDocument doc = XDocument.Load("Application.config");
            var controlpad = doc.Descendants("controlpads").Descendants("controlpad").
                SingleOrDefault(p => p.Attribute("id").Value == this.Id.ToString());
            if (controlpad == null) { return null; }
            var cameras = controlpad.Descendants("cameras").Descendants("camera");
            if (cameras == null) { return null; }
            ObservableCollection<Camera> list = new ObservableCollection<Camera>();
            foreach (var item in cameras)
            {
                var props = item.Descendants();
                Camera camera = new Camera()
                {
                    Id = int.Parse(item.Attribute("id").Value),
                    Name = item.Attribute("name").Value,
                    IP = props.SingleOrDefault(p => p.Name == "ip").Value,
                    BeltNo = int.Parse(props.SingleOrDefault(p => p.Name == "beltNo").Value),
                    NetPortNum = int.Parse(props.SingleOrDefault(p => p.Name == "netPortNum").Value),
                    ControlPadNo = int.Parse(props.SingleOrDefault(p=>p.Name == "controlPadNo").Value),
                    AlarmPicDir = props.SingleOrDefault(p => p.Name == "alarmPicDir").Value
                };
                list.Add(camera);
            }
            return list;
        }

        public static ObservableCollection<ControlPad> GetAllControlPads()
        {
            XDocument doc = XDocument.Load("Application.config");
            var controlpads = doc.Descendants("controlpads").Descendants("controlpad");
            if (controlpads == null) { return null; }
            ObservableCollection<ControlPad> list = new ObservableCollection<ControlPad>();
            foreach (var item in controlpads)
            {
                var attrs = item.Attributes();
                ControlPad pad = new ControlPad()
                {
                    Id = int.Parse(attrs.SingleOrDefault(a => a.Name == "id").Value),
                    Name = attrs.SingleOrDefault(a => a.Name == "name").Value,
                    IP = attrs.SingleOrDefault(a => a.Name == "ip").Value,
                    PortNum = int.Parse(attrs.SingleOrDefault(a => a.Name == "port").Value)
                };

                list.Add(pad);
            }
            return list;
        }

        public static void AddControlPad(ControlPad pad)
        {
            XDocument doc = XDocument.Load("Application.config");
            //找到controlpads节点
            var controlpads = doc.Descendants("controlpads").SingleOrDefault();
            if (controlpads == null) { return; }
            var newPad = new XElement("controlpad");
            //attribute
            newPad.SetAttributeValue("id", pad.Id);
            newPad.SetAttributeValue("name", pad.Name);
            newPad.SetAttributeValue("ip", pad.IP);
            newPad.SetAttributeValue("port", pad.PortNum);
            //add
            
            controlpads.Add(newPad);
            //save
            doc.Save("Application.config");
        }

        public static void UpdateControlPad(ControlPad pad)
        {
            XDocument doc = XDocument.Load("Application.config");
            //找到controlpads节点
            var controlpads = doc.Descendants("controlpads").SingleOrDefault();
            if (controlpads == null) { return; }
            var controlpad = controlpads.Descendants("controlpad").
                SingleOrDefault(p => p.Attribute("id").Value == pad.id.ToString());  
            if (controlpad == null) { return; }
            controlpad.SetAttributeValue("name",pad.Name);
            controlpad.SetAttributeValue("ip",pad.IP);
            controlpad.SetAttributeValue("port", pad.PortNum);
            //save
            doc.Save("Application.config");
        }

        public static bool DeleteControlPad(int padId)
        {
            XDocument doc = XDocument.Load("Application.config");
            //找到controlpads节点
            var controlpads = doc.Descendants("controlpads").SingleOrDefault();
            if (controlpads == null) { return false; }
            var controlpad = controlpads.Descendants("controlpad").
                SingleOrDefault(p => p.Attribute("id").Value == padId.ToString());
            int cameraCount = controlpad.Descendants("camera").Count();
            if(cameraCount > 0)
            {
                ModernDialog.ShowMessage("该控制板下存在相机，不能删除!","提示",MessageBoxButton.OK);
                return false;
            }else
            {
                var result = ModernDialog.ShowMessage("确实要删除该控制板吗？", "提示", MessageBoxButton.OKCancel);
                if(result == MessageBoxResult.OK)
                {
                    controlpad.Remove();
                    doc.Save("Application.config");
                    return true;
                }
                return false;
                
            }
            
        }
        #endregion

        //切换网口
        public void SwitchNetPort(int netPort)
        {
            if (tcpComm!=null)
            {
                command = new byte[5];
                command[0] = 0x87;
                command[1] = (byte)(netPort - 1);
                command[4] = 0x0a;
                tcpComm.SendData(command);
                
            }

        }

    }
}
