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
        byte[] sendCommand;
        byte[] recData;
        TcpComm tcpComm;
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
            tcpComm = new TcpComm();
            sendCommand = new byte[30];
            sendCommand[0] = 0x87;
            sendCommand[29] = 0x0a;

        }

        public void LoadPara()
        {

        }

        public void SavePara()
        {

        }

        public void SwitchNetPort(int tcpPortNo)
        {
            sendCommand[6] = (byte)tcpPortNo;
            tcpComm.SendData(sendCommand);
        }


    }
}
