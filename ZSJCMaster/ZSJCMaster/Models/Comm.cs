using Prism.Mvvm;
using System;
using System.IO.Ports;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
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
        public TcpComm() { }
        public TcpComm(string ip, int port)
        {
            client = new TcpClient(ip, port);
            ns = client.GetStream();

        }

        ~TcpComm()
        {
            ns.Close();
            client.Close();
        }
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
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.  
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                // Start listening for client requests.  
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
                        // Translate data bytes to a ASCII string.  
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);
                        // Process the data sent by the client.  
                        
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                        // Send back a response.  
                        stream.Write(msg, 0, msg.Length);
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
    }

}