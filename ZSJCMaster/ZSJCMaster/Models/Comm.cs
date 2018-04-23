using Prism.Mvvm;
using System;
using System.IO.Ports;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
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
            serial = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
            serial.ReceivedBytesThreshold = 4;//设置 DataReceived 事件发生前内部输入缓冲区中的字节数
        }

        ~SerialComm()
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

        public void Open()
        {
            if (!serial.IsOpen)
            {
                serial.Open();
            }

        }

        public void SendData(Byte[] data)
        {
            Open();
            serial.Write(data, 0, data.Length);
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
    public class TcpComm : BindableBase
    {
        TcpClient client;
        NetworkStream ns;
        TcpListener server;
        string ip;
        int port;
        //public delegate void TcpRecvMethod();

        public TcpComm() { }
        public TcpComm(string ip, int port)
        {
            client = new TcpClient(ip, port);
            ns = client.GetStream();
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            Thread t1 = new Thread(new ThreadStart(RecData));
            t1.IsBackground = true;
            t1.Start();
        }

        ~TcpComm()
        {
            if (ns!=null)
            {
                ns.Close();
            }
            if (client!=null)
            {
                client.Close();
            }        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        public void SendData(byte[] data)
        {
            ns.Write(data, 0, data.Length);
        }

        public void RecData()
        {
            try
            {
                server.Start();
                // Buffer for reading data  
                Byte[] bytes = new Byte[256];
                String data = null;
                // Enter the listening loop.  
                while (true)
                {
                    // Perform a blocking call to accept requests.  
                    // You could also user server.AcceptSocket() here.  
                    TcpClient client = server.AcceptTcpClient();
                    data = null;
                    // Get a stream object for reading and writing  
                    NetworkStream stream = client.GetStream();
                    int i;
                    // Loop to receive all the data sent by the client.  
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        //byte[] information analysis
                        Decode(bytes);
                    }
                    // Shutdown and end connection  
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.  
                server.Stop();
            }

        }

        private void Decode(byte[] bytes)
        {
            //包长度30
            if (bytes.Length==30)
            {
                if (bytes[0]==0x87 && bytes[29]==0x0a)
                {
                    //警报标志位
                    for (int i = 0; i < 5; i++)
                    {
                        AlarmFlags[i] = (bytes[i+1] == 1);
                    }
                    
                    CurrentNetPort = bytes[6];
                    AlarmInfos = new AlarmInfo[5];
                    int index = 0;
                    int offset = 7;
                    for (int i = 0; i < 5; i++)
                    {
                        AlarmInfos[i + index * 4].cameraNo = bytes[i + index * 4 + offset];
                        AlarmInfos[i + index * 4 + 1].x = bytes[i + index * 4 + 1 + offset];
                        AlarmInfos[i + index * 4 + 2].y = bytes[i + index * 4 + 2 + offset];
                        AlarmInfos[i + index * 4 + 3].width = bytes[i + index * 4 + 3 + offset];
                    }
                }
            }
        }

        public bool[] AlarmFlags { get; set; }
        public int CurrentNetPort { get; set; }
        public AlarmInfo[] AlarmInfos { get; set; }

        public class AlarmInfo:BindableBase
        {
            public int cameraNo { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public int width { get; set; }
        }
    }

}