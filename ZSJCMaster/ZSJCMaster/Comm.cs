using Prism.Mvvm;
using System;
using System.IO.Ports;
using System.Net.Sockets;

namespace 纵撕检测.Models.BasicOperation
{
    public class TcpComm: BindableBase
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
    }

    public class SerialComm: BindableBase
    {
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

        public SerialComm() { }
        public SerialComm(string portName="COM1", int baudRate=9600, System.IO.Ports.Parity parity= System.IO.Ports.Parity.None,int dataBits=8, StopBits stopBits=StopBits.One)
        {
            serial = new SerialPort(portName,baudRate,parity,dataBits,stopBits);
            //serial.ReceivedBytesThreshold = 4;//设置 DataReceived 事件发生前内部输入缓冲区中的字节数
            //eventHandler = serial.DataReceived;
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
            //serial.Open();
        }
        
        public void SendData(Byte[] data)
        {
            //serial.Write(data, 0, data.Length);
        }
    }

}