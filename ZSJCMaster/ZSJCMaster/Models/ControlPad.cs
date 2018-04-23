﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Xml.Linq;
using System.Linq;
using ZSJCMaster.Helpers;
using System.Collections.ObjectModel;

namespace ZSJCMaster.Models
{
    public class ControlPad: BindableBase
    {
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

        //构造函数,打开串口
        public ControlPad()
        {
            //LoadPara();
        }

        //public void LoadPara()
        //{
        //    XDocument doc = XDocument.Load("Application.config");
        //    var controlpad = doc.Descendants("controlpad").SingleOrDefault();
        //    if (controlpad == null) { return; }
        //    this.Id = int.Parse(controlpad.Attribute("id").Value);
        //    this.Name = controlpad.Attribute("name").Value;
        //    this.IP = controlpad.Attribute("ip").Value;

        //    this.Cameras = new List<Camera>();
        //    var cameras = controlpad.Descendants("cameras").Descendants("camera");
        //    if (cameras == null) { return; }
        //    foreach (var item in cameras)
        //    {
        //        var props = item.Descendants();
        //        Camera camera = new Camera()
        //        {
        //            No = int.Parse(item.Attribute("id").Value),
        //            Name = item.Attribute("name").Value,
        //            IP = props.SingleOrDefault(p => p.Name == "ip").Value,
        //            BeltNo = int.Parse(props.SingleOrDefault(p=>p.Name == "beltNo").Value),
        //            NetPortNum = int.Parse(props.SingleOrDefault(p=>p.Name == "netPortNum").Value),
        //            AlarmPicDir = props.SingleOrDefault(p=>p.Name == "alarmPicDir").Value
        //        };
        //        this.Cameras.Add(camera);
        //    }
        //}

        /// <summary>
        /// 从配置文件读取指定编号的控制板的所有相机信息
        /// </summary>
        /// <param name="controlPadNo">控制板编号</param>
        /// <returns>相机集合</returns>
        public List<Camera> GetCameras(int controlPadNo)
        {
            XDocument doc = XDocument.Load("Application.config");
            var controlpad = doc.Descendants("controlpads").Descendants("controlpad").
                SingleOrDefault(p => p.Attribute("id").Value == controlPadNo.ToString());
            if (controlpad == null) { return null; }
            var cameras = controlpad.Descendants("cameras").Descendants("camera");
            if (cameras == null) { return null; }
            List<Camera> list = new List<Camera>();
            foreach (var item in cameras)
            {
                var props = item.Descendants();
                Camera camera = new Camera()
                {
                    No = int.Parse(item.Attribute("id").Value),
                    Name = item.Attribute("name").Value,
                    IP = props.SingleOrDefault(p => p.Name == "ip").Value,
                    BeltNo = int.Parse(props.SingleOrDefault(p => p.Name == "beltNo").Value),
                    NetPortNum = int.Parse(props.SingleOrDefault(p => p.Name == "netPortNum").Value),
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

        public void SavePara()
        {

        }


    }
}
