using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class AlarmContentViewModel:SettingPageViewModel
    {
        #region 属性
        /// <summary>
        /// 串口号列表
        /// </summary>
        private List<MyPort> portList;
        public List<MyPort> PortList
        {
            get { return portList; }
            private set
            {
                portList = value;
                this.RaisePropertyChanged("PortList");
            }
        }

        /// <summary>
        /// 波特率列表
        /// </summary>
        private List<MyBaudRate> baudRateList;
        public List<MyBaudRate> BaudRateList
        {
            get { return baudRateList; }
            private set
            {
                baudRateList = value;
                this.RaisePropertyChanged("BaudRateList");
            }
        }

        /// <summary>
        /// 数据位列表
        /// </summary>
        private List<MyDataBits> dataBitList;
        public List<MyDataBits> DataBitList
        {
            get { return dataBitList; }
            private set
            {
                dataBitList = value;
                this.RaisePropertyChanged("DataBitList");
            }
        }

        /// <summary>
        /// 校验位列表
        /// </summary>
        private List<MyParity> parityList;
        public List<MyParity> ParityList
        {
            get { return parityList; }
            private set
            {
                parityList = value;
                this.RaisePropertyChanged("ParityList");
            }
        }

        /// <summary>
        /// 停止位列表
        /// </summary>
        private List<MyStopBits> stopBitList;
        public List<MyStopBits> StopBitList
        {
            get { return stopBitList; }
            private set
            {
                stopBitList = value;
                this.RaisePropertyChanged("StopBitList");
            }
        }

        /// <summary>
        /// 端口号
        /// </summary>
        private MyPort portName;
        public MyPort PortName
        {
            get { return portName; }
            set
            {
                portName = value;
                this.RaisePropertyChanged("Port");
            }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        private MyBaudRate baudRate;
        public MyBaudRate BaudRate
        {
            get { return baudRate; }
            set
            {
                baudRate = value;
                this.RaisePropertyChanged("BaudRate");
            }
        }

        /// <summary>
        /// 数据位
        /// </summary>
        private MyDataBits dataBits;
        public MyDataBits DataBits
        {
            get { return dataBits; }
            set
            {
                dataBits = value;
                this.RaisePropertyChanged("DataBits");
            }
        }

        /// <summary>
        /// 校验位
        /// </summary>
        private MyParity parity;
        public MyParity Parity
        {
            get { return parity; }
            set
            {
                parity = value;
                this.RaisePropertyChanged("Parity");
            }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        private MyStopBits stopBits;
        public MyStopBits StopBits
        {
            get { return stopBits; }
            set
            {
                stopBits = value;
                this.RaisePropertyChanged("StopBits");
            }
        }
        #endregion
        #region command
        public DelegateCommand<ExCommandParameter> ChangeSerialPortParamsCommand { get; set; }
        #endregion
        #region command function
        private void ChangeSerialPortsParam(ExCommandParameter param)
        {
            var sender = param.Sender as System.Windows.Controls.ComboBox;
            int selectedIndex = sender.SelectedIndex;
            if (selectedIndex < 0) { return; }
            var selectedItem = sender.SelectedItem as SettingWindowParamHelper;
            XmlConfigHelper config = new XmlConfigHelper();
            config.Load("Application.config");
            config.UpdateNodeValue(selectedItem.ParamTypeName, selectedItem.Value.ToString());
            config.Save("Application.config");
        }
        #endregion
        #region method
        private void InitPortNames()
        {
            //填充端口号
            this.PortList = new List<MyPort>();
            for (int i = 1; i <= 30; i++)
            {
                var port = new MyPort() { DisplayName = "COM" + i, Value = "COM" + i };
                this.PortList.Add(port);
            }
        }

        private void InitBaudRates()
        {
            //填充波特率
            this.BaudRateList = new List<MyBaudRate>();
            int[] bautRates = { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 43000, 56000, 57600, 115200 };
            for (int i = 0; i < bautRates.Length; i++)
            {
                var baudRate = new MyBaudRate()
                {
                    DisplayName = bautRates[i].ToString(),
                    Value = bautRates[i]
                };
                this.BaudRateList.Add(baudRate);
            }
        }

        private void InitDataBits()
        {
            //填充数据位
            this.DataBitList = new List<MyDataBits>();
            int[] dataBits = { 5, 6, 7, 8 };
            for (int i = 0; i < dataBits.Length; i++)
            {
                var dataBit = new MyDataBits()
                {
                    DisplayName = dataBits[i].ToString(),
                    Value = dataBits[i]
                };
                this.DataBitList.Add(dataBit);
            }
        }

        private void InitStopBits()
        {
            //填充停止位
            string none ="无";
            string one = "1";
            string two = "2";
            string onePointFive = "1.5";
            this.StopBitList = new List<MyStopBits>()
            {
                //none,one,two,onePointFive
                new MyStopBits() { DisplayName=none,Value=(int)System.IO.Ports.StopBits.None },
                new MyStopBits() { DisplayName=one,Value=(int)System.IO.Ports.StopBits.One},
                new MyStopBits() { DisplayName=two,Value=(int)System.IO.Ports.StopBits.Two },
                new MyStopBits() { DisplayName=onePointFive,Value=(int)System.IO.Ports.StopBits.OnePointFive }
            };
        }

        private void InitParity()
        {
            //填充校验位
            string none = "无";
            string odd = "奇校验";
            string even = "偶校验";
            string mark = "标记";
            string space = "空";
            this.ParityList = new List<MyParity>()
            {
                //none,odd,even,mark,space
                new MyParity() { DisplayName=none,Value=(int)System.IO.Ports.Parity.None },
                new MyParity() { DisplayName=odd,Value=(int)System.IO.Ports.Parity.Odd },
                new MyParity() { DisplayName=even,Value=(int)System.IO.Ports.Parity.Even},
                new MyParity() { DisplayName=mark,Value=(int)System.IO.Ports.Parity.Mark },
                new MyParity() { DisplayName=space,Value=(int)System.IO.Ports.Parity.Space}
            };
        }
        #endregion 

        public AlarmContentViewModel()
        {
            //填充端口号
            InitPortNames();
            //填充波特率
            InitBaudRates();
            //填充数据位
            InitDataBits();
            //填充校验位
            InitParity();
            //填充停止位
            InitStopBits();

            //读取配置文件
            SerialComm comm = new SerialComm();
            this.PortName = PortList.SingleOrDefault(p => p.DisplayName == comm.PortName);
            this.BaudRate = BaudRateList.SingleOrDefault(b => (int)b.Value == comm.BaudRate);
            this.Parity = ParityList.SingleOrDefault(p => (Parity)p.Value == comm.Parity);
            this.DataBits = DataBitList.SingleOrDefault(d => (int)d.Value == comm.DataBits);
            this.StopBits = StopBitList.SingleOrDefault(s => (StopBits)s.Value == comm.StopBits);

            this.ChangeSerialPortParamsCommand = new DelegateCommand<ExCommandParameter>(ChangeSerialPortsParam);
        }
    }

    public class SettingWindowParamHelper
    {
        public string DisplayName { get; set; }
        public object Value { get; set; }
        public string ParamTypeName { get; set; }
    }

    public class MyPort : SettingWindowParamHelper
    {
        public MyPort()
        {
            this.ParamTypeName = "portName";
        }
    }
    public class MyBaudRate : SettingWindowParamHelper
    {
        public MyBaudRate()
        {
            this.ParamTypeName = "baudRate";
        }
    }
    public class MyDataBits : SettingWindowParamHelper
    {
        public MyDataBits()
        {
            this.ParamTypeName = "dataBits";
        }
    }
    public class MyParity : SettingWindowParamHelper
    {
        public MyParity()
        {
            this.ParamTypeName = "parity";
        }
    }
    public class MyStopBits : SettingWindowParamHelper
    {
        public MyStopBits()
        {
            this.ParamTypeName = "stopBits";
        }
    }
}
