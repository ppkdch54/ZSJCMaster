using Prism.Mvvm;
using SimpleTCP;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using ZSJCMaster.Helpers;

namespace ZSJCMaster.Models
{
    public class SerialComm: BindableBase
    {
        string portName;
        int baudRate;
        System.IO.Ports.Parity parity;
        int dataBits;
        StopBits stopBits;
        

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
        SerialPort serial;
        //public EventHandler DataReceived { get; set{} }
        public SerialPort Serial
        {
            get { return serial; }
            set
            {
                serial = value;
                this.RaisePropertyChanged("Serial");
            }
        }

        public SerialComm()
        {
            LoadPara();//载入串口配置参数
        }

        public void InitialSerial()
        {
            NewMethod();
            serial.ReceivedBytesThreshold = 4;//设置 DataReceived 事件发生前内部输入缓冲区中的字节数
            Open();
        }

        private void NewMethod()
        {
            serial = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
        }

        ~SerialComm()
        {
            if(serial != null)
            {
                if (serial.IsOpen)
                {
                    try
                    {
                        serial.Close();
                        serial.Dispose();
                    }
                    catch (Exception err)
                    {
                        throw new Exception(err.Message);
                    }
                }
            }
            
        }

        public void Open()
        {
            //if (!serial.IsOpen)
            // {
            try
            {
                serial.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                
           // }

        }

        public void SendData(Byte[] data)
        {
            if (serial.IsOpen)
            {
                serial.Write(data, 0, data.Length);
            }
        }

        private void LoadPara()
        {
            XmlConfigHelper config = new XmlConfigHelper();
            config.Load("Application.config");
            PortName = config.ReadNodeValue("portName");
            BaudRate = int.Parse(config.ReadNodeValue("baudRate"));
            Parity = (Parity)int.Parse(config.ReadNodeValue("parity"));
            DataBits = int.Parse(config.ReadNodeValue("dataBits"));
            StopBits = (StopBits)int.Parse(config.ReadNodeValue("stopBits"));
        }
    }

    public delegate void TcpRecvDelegate(AlarmInfo[] alarms, bool[] flags);

    public class TcpComm : BindableBase
    {
        SimpleTcpClient simpleTcp;
        string ip;
        int port;

        private TcpRecvDelegate tcpRecv;
        public TcpRecvDelegate TcpRecv
        {
            get { return tcpRecv; }
            set
            {
                tcpRecv = value;
            }
        }

        public TcpComm() { }
        public TcpComm(string ip, int port)
        {
            AlarmFlags = new bool[5];

            this.ip = ip;
            this.port = port;
            try
            {
                simpleTcp = new SimpleTcpClient().Connect(ip, port);
                simpleTcp.DataReceived += (sender, msg) =>
                {
                    Decode(msg.Data);
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        ~TcpComm()
        {
            if (simpleTcp!=null)
            {
                simpleTcp.Disconnect();
                simpleTcp.Dispose();
            }

        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        public void SendData(byte[] data)
        {
            if (simpleTcp!=null)
            {
                simpleTcp.Write(data);
            }
            
        }


        private void Decode(byte[] bytes)
        {
            if (bytes.Length >= 30)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == 0x87)
                    {
                        if (bytes[i + 29] == 0x0a)
                        {
                            CurrentNetPort = bytes[6 + i];
                            AlarmInfos = new AlarmInfo[5];
                            int offset = 7 + i;
                            for (int k = 0; k < 5; k++)
                            {
                                AlarmFlags[k] = (bytes[k + 1] == 1);
                                AlarmInfos[k] = new AlarmInfo();
                                //AlarmInfos[k].cameraNo = bytes[k * 4 + offset];
                                AlarmInfos[k].cameraNo = bytes[k + 1] == 1 ? 6 - k - 1 : 0;
                                AlarmInfos[k].x = bytes[k * 4 + 1 + offset];
                                AlarmInfos[k].y = bytes[k * 4 + 2 + offset];
                                AlarmInfos[k].width = bytes[k * 4 + 3 + offset];
                                AlarmInfos[k].infoTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            bool alarmFlag = false;
                            for (int j = 0; j < 5; j++)
                            {
                                if (AlarmFlags[j])
                                {
                                    alarmFlag = true;
                                }
                            }

                            if (alarmFlag && (this.TcpRecv != null))
                            {
                                this.TcpRecv(AlarmInfos, AlarmFlags);
                            }
                            break;
                        }
                        
                    }
                }
            }
        }

        public bool[] AlarmFlags { get; set; }
        public int CurrentNetPort { get; set; }
        public AlarmInfo[] AlarmInfos { get; set; }
    }

    public class AlarmInfo : BindableBase
    {
        public int cameraNo { get; set; }
        public string cameraName { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public string info { get; set; } = "有报警，请连接至下位机查看";
        public string infoTime { get; set; }
    }

}