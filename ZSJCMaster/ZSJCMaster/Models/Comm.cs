using Prism.Mvvm;
using SimpleTCP;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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

        #region 串口参数(仅供设置参数使用)
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
        #endregion
        SerialPort serial;

        public SerialComm()
        {
            LoadPara();//载入串口配置参数
        }

        public void InitialSerial()
        {
            if(portName != "None")
            {
                serial = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
                serial.ReceivedBytesThreshold = 4;//设置 DataReceived 事件发生前内部输入缓冲区中的字节数
                Open();
            } 
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
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            
        }

        public void Open()
        {
            try
            {
                serial.Open();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void SendData(Byte[] data)
        {
            if (serial != null && serial.IsOpen)
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
        TcpRecvDelegate tcpRecv;
        string ip;
        int port;

        private bool isConnection;
        /// <summary>
        /// 获取或者设置网络连接状态
        /// </summary>
        public bool IsConnection
        {
            get { return isConnection; }
            set { isConnection = value; }
        }

        public TcpComm() { }
        public TcpComm(string ip, int port,TcpRecvDelegate tcpRecv)
        {
            this.ip = ip;
            this.port = port;
            this.tcpRecv = tcpRecv;
            simpleTcp = new SimpleTcpClient();
            AlarmFlags = new bool[5];
            try
            {
                simpleTcp.Connect(ip, port);
                simpleTcp.DataReceived += (sender, msg) =>
                {
                    Decode(msg.Data);
                };
            }
            catch (Exception)
            {
                IsConnection = false;
            }
            Task.Run(() =>
            {
                checkState();
            });
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
                try
                {
                    simpleTcp.Write(data);
                }
                catch (Exception)
                {
                    IsConnection = false;
                }
                
            }
            
        }

        private void checkState()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (simpleTcp.TcpClient.Connected == false)
                {
                    try
                    {
                        simpleTcp.TcpClient.Close();
                        simpleTcp.Connect(ip, port);
                        simpleTcp.DataReceived += (sender, msg) =>
                        {
                            Decode(msg.Data);
                        };
                        IsConnection = true;
                    }
                    catch
                    {
                        IsConnection = false;
                    }
                }

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
                                AlarmInfos[k].X = bytes[k * 4 + offset];
                                AlarmInfos[k].Y = bytes[k * 4 + 1 + offset];
                                AlarmInfos[k].CameraNo = bytes[k * 4 + 2 + offset];
                                AlarmInfos[k].Width = bytes[k * 4 + 3 + offset];
                                AlarmInfos[k].InfoTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            bool alarmFlag = false;
                            for (int j = 0; j < 5; j++)
                            {
                                if (AlarmFlags[j])
                                {
                                    alarmFlag = true;
                                }
                            }

                            if (alarmFlag && (this.tcpRecv != null))
                            {
                                this.tcpRecv(AlarmInfos, AlarmFlags);
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

    public class UdpComm
    {
        UdpClient client;
        public UdpComm(int port)
        {
            client = new UdpClient(port);
        }
        public UdpComm(string ip,int port)
        {
            client = new UdpClient(ip, port);
        }

        public byte[] Receive(string ip,int port)
        {
            if(client != null)
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(ip),port);
                byte[] datas = client.Receive(ref endPoint);
                return datas;
            }
            return null;
        }

        public void SendData(byte[] data)
        {
            if(client != null)
            {
                client.Send(data, data.Length);
            }
        }
    }
    public class AlarmInfo : BindableBase
    {
        public int CameraNo { get; set; }
        public string CameraName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public string InfoTime { get; set; }
    }

    /// <summary>
    /// 报警信息Model(数据库专用)
    /// </summary>
    public class AlarmInfoForDB:BindableBase
    {
        public long Id { get; set; }
        public int ControlPadId { get; set; }
        public int CameraId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public DateTime InfoTime { get; set; }

    }

}