using System;
using System.Collections.Generic;
using System.IO.Ports;
/* 
警报器 通讯协议V2基本控制指令

请用十六进制进行发送，波特率为9600，以下指令已含CRC校验码

0110001A000101CE18  声音1+闪光
0110001A0001040E1B  声音2+闪光
0110001A0001028E19  只有闪光
0110001A0001034FD9  只有声音1
0110001A000105CFDB  只有声音2
0110001A0001000FD8  全关闭
*/

namespace ZSJCMaster.Models
{
    public class AlarmLamp
    {
        string portName;
        int baudRate;
        System.IO.Ports.Parity parity;
        int dataBits;
        StopBits stopBits;

        SerialComm serialComm;

        public int ReceivedTotalLength { get; private set; }
        public int ReceivedLoopCount { get; private set; }

        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;
            }
        }
        public int BaudRate
        {
            get { return baudRate; }
            set
            {
                baudRate = value;
            }
        }
        public Parity Parity
        {
            get { return parity; }
            set
            {
                parity = value;
            }
        }
        public int DataBits
        {
            get { return dataBits; }
            set
            {
                dataBits = value;
            }
        }
        public StopBits StopBits
        {
            get { return stopBits; }
            set
            {
                stopBits = value;
            }
        }

        //构造函数,打开串口
        public AlarmLamp()
        {
            LoadPara();//载入串口配置参数
            serialComm = new SerialComm(PortName,BaudRate,Parity,DataBits,StopBits);
            //serialComm.Serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);//DataReceived事件委托
            //OpenSerial();
        }

        private void LoadPara()
        {
            //XmlConfigHelper config = new XmlConfigHelper();
            //config.Load("Application.config");
            //PortName = config.ReadNodeValue("portName");
            //BaudRate = int.Parse(config.ReadNodeValue("baudRate"));
            //Parity = (Parity)int.Parse(config.ReadNodeValue("parity"));
            //DataBits = int.Parse(config.ReadNodeValue("dataBits"));
            //StopBits = (StopBits)int.Parse(config.ReadNodeValue("stopBits"));
        }

        public void SavePara()
        {
            /*
            Properties.Settings.Default.PortName=PortName;
            Properties.Settings.Default.BaudRate=BaudRate;
            Properties.Settings.Default.Parity=int.Parse(Parity.ToString());
            Properties.Settings.Default.DataBits=DataBits;
            Properties.Settings.Default.StopBits=int.Parse(StopBits.ToString());
            */
        }

        public void OpenSerial()
        {
            serialComm.Open();
        }
        /// <summary>
        /// 开始闪光加声音警报
        /// </summary>
        public void AlarmMusicAndFlash()
        {
            //01 10 00 1A 00 01 01 CE 18
            byte[] protocol = new byte[] { 0x01, 0x10, 0x00, 0x1A, 0x00, 0x01, 0x01, 0xCE, 0x18 };
            serialComm.SendData(protocol);
        }

        /// <summary>
        /// 停止所有警报
        /// </summary>
        public void StopAllAlarm()
        {
            //01 10 00 1A 00 01 00 0F D8
            byte[] protocol = new byte[] { 0x01, 0x10, 0x00, 0x1A, 0x00, 0x01, 0x00, 0x0F, 0xD8 };
            serialComm.SendData(protocol);
        }
    }
}
