using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using ZSJCMaster.Helpers;

namespace ZSJCMaster.Models
{
    public class ControlPad: BindableBase
    {
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

        private int portName;

        /// <summary>
        /// 控制板TCP端口
        /// </summary>
        public int PortName
        {
            get { return portName; }
            set
            {
                portName = value;
                this.RaisePropertyChanged("PortName");
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

        }

        public void LoadPara()
        {

        }

        public void SavePara()
        {

        }


    }
}
