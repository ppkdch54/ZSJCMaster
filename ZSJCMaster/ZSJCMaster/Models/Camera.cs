using FirstFloor.ModernUI.Windows.Controls;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using ZSJCMaster.Helpers;

namespace ZSJCMaster.Models
{

    public class Camera : BindableBase
    {
        private int id;

        /// <summary>
        /// 相机编号
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


        private string name = "新相机";

        /// <summary>
        /// 相机名称
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

        private string ip = "0.0.0.0";

        /// <summary>
        /// 相机IP
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

        private int controlPadNo;

        /// <summary>
        /// 控制板编号
        /// </summary>
        public int ControlPadNo
        {
            get { return controlPadNo; }
            set
            {
                controlPadNo = value;
                this.RaisePropertyChanged("ControlPadNo");
            }
        }

        private int beltNo;

        /// <summary>
        /// 皮带编号
        /// </summary>
        public int BeltNo
        {
            get { return beltNo; }
            set
            {
                beltNo = value;
                this.RaisePropertyChanged("BeltNo");
            }
        }

        private int netPortNum;

        /// <summary>
        /// 网口编号
        /// </summary>
        public int NetPortNum
        {
            get { return netPortNum; }
            set
            {
                netPortNum = value;
                this.RaisePropertyChanged("NetPortNum");
            }
        }

        private string alarmPicDir;

        /// <summary>
        /// 报警图片存储路径
        /// </summary>
        public string AlarmPicDir
        {
            get { return alarmPicDir; }
            set
            {
                alarmPicDir = value;
                this.RaisePropertyChanged("AlarmPicDir");
            }
        }

        private bool isSwitching;

        /// <summary>
        /// 是否正在切换
        /// </summary>
        public bool IsSwitching
        {
            get { return isSwitching; }
            set
            {
                isSwitching = value;
                this.RaisePropertyChanged("IsSwitching");
            }
        }



        public Camera()
        {
            LoadPara();
        }

        private void LoadPara()
        {
            //XmlConfigHelper config = new XmlConfigHelper();
            //config.Load("Application.config");
            //No = int.Parse(config.ReadNodeValue("no"));
        }

        

        public void SavePara()
        {

        }

        public static void AddCamera(Camera camera)
        {
            XDocument doc = XDocument.Load("Application.config");
            var controlpad = doc.Descendants("controlpads").Descendants("controlpad").
                SingleOrDefault(p => p.Attribute("id").Value == camera.ControlPadNo.ToString());
            if (controlpad == null) { return; }
            var camerasNode = controlpad.Descendants("cameras");
            //如果为空，说明是第一次添加相机
            if(camerasNode.Count() <= 0)
            {
                controlpad.Add(new XElement("cameras"));
            }
            //重新获取
            var cameras = controlpad.Descendants("cameras").Single();
            //添加相机
            var cameraElement = new XElement("camera");
            cameraElement.SetAttributeValue("id", camera.Id);
            cameraElement.SetAttributeValue("name", camera.Name);
            cameraElement.Add(new XElement("ip", camera.IP));
            cameraElement.Add(new XElement("beltNo", camera.BeltNo));
            cameraElement.Add(new XElement("netPortNum", camera.NetPortNum));
            cameraElement.Add(new XElement("controlPadNo", camera.ControlPadNo));
            cameraElement.Add(new XElement("alarmPicDir", camera.AlarmPicDir));
            cameras.Add(cameraElement);
            //save
            doc.Save("Application.config");
        }

        public static void UpdateCamera(Camera camera)
        {
            XDocument doc = XDocument.Load("Application.config");
            var controlpad = doc.Descendants("controlpads").Descendants("controlpad").
                SingleOrDefault(p => p.Attribute("id").Value == camera.ControlPadNo.ToString());
            if (controlpad == null) { return; }
            var cameras = controlpad.Descendants("cameras").Descendants("camera");
            if (cameras == null) { return; }
            var existCamera = cameras.SingleOrDefault(c => c.Attribute("id").Value == camera.Id.ToString());
            if (existCamera == null) { return; }
            existCamera.Attribute("name").SetValue(camera.Name);
            var nodes = existCamera.Descendants().ToList();
            nodes.Single(p => p.Name == "ip").SetValue(camera.IP);
            nodes.Single(p => p.Name == "beltNo").SetValue(camera.BeltNo);
            nodes.Single(p => p.Name == "netPortNum").SetValue(camera.NetPortNum);
            nodes.Single(p => p.Name == "controlPadNo").SetValue(camera.ControlPadNo);
            nodes.Single(p => p.Name == "alarmPicDir").SetValue(camera.AlarmPicDir);
            //save
            doc.Save("Application.config");
        }

        public static bool DeleteCamera(int cameraId,int controlpadId)
        {
            XDocument doc = XDocument.Load("Application.config");
            var controlpad = doc.Descendants("controlpads").Descendants("controlpad").
                SingleOrDefault(p => p.Attribute("id").Value == controlpadId.ToString());
            if (controlpad == null) { return false; }
            var cameras = controlpad.Descendants("cameras").Descendants("camera");
            if (cameras == null) { return false; }
            var existCamera = cameras.SingleOrDefault(c => c.Attribute("id").Value == cameraId.ToString());
            if (existCamera == null) { return false; }
            var result = ModernDialog.ShowMessage("确实要删除该相机吗？", "提示", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                existCamera.Remove();
                doc.Save("Application.config");
                return true;
            }
            return false;
            
        }

    }
}
