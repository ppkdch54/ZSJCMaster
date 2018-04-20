using System;
using System.Collections.Generic;
using System.IO.Ports;
using 纵撕检测.Helpers;
using 纵撕检测.Models.BasicOperation;

namespace 纵撕检测.Models
{
    interface IControlPad
    {
    }

    public class ControlPad : IControlPad
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
        public ControlPad()
        {
            LoadPara();//载入串口配置参数
            serialComm = new SerialComm(PortName,BaudRate,Parity,DataBits,StopBits);
            serialComm.Serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);//DataReceived事件委托
            //OpenSerial();
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
        /// 启动除尘装置
        /// </summary>
        public void StartMotor()
        {
            byte[] protocol = new byte[] { 0x88, 0x03, 0x0A };
            serialComm.SendData(protocol);
        }

        /// <summary>
        /// 停止除尘装置
        /// </summary>
        public void StopMotor()
        {
            byte[] protocol = new byte[] { 0x88, 0x04, 0x0A };
            serialComm.SendData(protocol);
        }

        /// <summary>
        /// 报警信息发送,需要进一步修改,缺少信息
        /// </summary>
        void SendAlarmInfo()
        {
            throw new Exception();
            //byte[] protocol = BitConverter.GetBytes(0x860000000A);
            //serial.SendData(protocol);
        }
        /// <summary>
        /// 复位皮带总长,按钮发送
        /// </summary>
        public void ResetTotalLength()
        {
            byte[] protocol = new byte[] { 0x88, 0xff, 0xff, 0x0A };
            serialComm.SendData(protocol);
        }
        /// <summary>
        /// 设置皮带总长度
        /// </summary>
        /// <param name="length">皮带总长度</param>
        public void SetTotalLength(int length)
        {
            byte[] protocol = new byte[] { 0x88, 0x00, 0x00, 0x0A };
            int hLengthHigh = (length / 256);
            int hLengthLow = (length % 256);
            protocol[1] = (byte)hLengthHigh;
            protocol[2] = (byte)hLengthLow;
            serialComm.SendData(protocol);
        }
        /// <summary>
        /// 发送补偿值
        /// </summary>
        /// <param name="value">补偿值</param>
        public void SendCompensation(int value)
        {
            byte[] protocol = new byte[] { 0x83, 0x00, 0x00, 0x0A };
            if (value>0)
            {
                protocol[1] = 0x01;
            }
            else
            {
                value = -value;
            }
            protocol[2] = (byte)value;
            serialComm.SendData(protocol);
        }

        public static void SaveConnectConfig(string fileName)
        {
            //用现有库做保存文件
            FileOp fileOp = new FileOp(fileName);
            fileOp.SaveConfig();//struct info
        }
        private void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                List<int> result = new List<int>();
                //循环接收数据
                while (serialComm.Serial.BytesToRead > 0)
                {
                    int current = serialComm.Serial.ReadByte();
                    result.Add(current);
                }
                if (result[3] == 0x0a)
                {
                    switch (result[0])
                    {
                        //返回值：0x84,0x00,0x00,0x0a  返回目前设置的总长
                        case 0x84:
                            ReceivedTotalLength = (result[1] << 8) + result[2];
                            break;
                        //返回值：0x87,0x00,0x00,0x0a  返回目前圈数计数
                        case 0x87:
                            ReceivedLoopCount = (result[1] << 8) + result[2];
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
