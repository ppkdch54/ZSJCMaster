using FirstFloor.ModernUI.Windows.Controls;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class AlarmContentViewModel:SettingPageViewModel
    {
        public UDPHost CurrentUDPClient { get; set; }
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

        private UDPServer udpServer;

        /// <summary>
        /// UDP服务器
        /// </summary>
        public UDPServer UDPServer
        {
            get { return udpServer; }
            set
            {
                udpServer = value;
                this.RaisePropertyChanged("UDPServer");
            }
        }

        private ObservableCollection<UDPHost> clients;

        /// <summary>
        /// UDP客户端集合
        /// </summary>
        public ObservableCollection<UDPHost> Clients
        {
            get { return clients; }
            set
            {
                clients = value;
                this.RaisePropertyChanged("Clients");
            }
        }



        #endregion
        #region command
        public DelegateCommand<ExCommandParameter> ChangeSerialPortParamsCommand { get; set; }
        public DelegateCommand<ExCommandParameter> ChangeUDPServerParamsCommand { get; set; }
        public DelegateCommand<ExCommandParameter> BeginningEditCommand { get; set; }
        public DelegateCommand<ExCommandParameter> RowEditEndingCommand { get; set; }
        public DelegateCommand<ExCommandParameter> DeleteClientCommand { get; set; }
        public DelegateCommand AddNewClientCommand { get; set; }
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
        private void ChangeUDPServerParams(ExCommandParameter param)
        {
            var txt = param.Sender as TextBox;
            string tag = txt.Tag.ToString();
            string value = txt.Text;
            UDPServer.UpdateServerToConfig(tag, value);
        }
        private void BeginningEdit(ExCommandParameter param)
        {
            var sender = param.Sender as DataGrid;
            var args = param.EventArgs as DataGridBeginningEditEventArgs;
            int index = args.Column.DisplayIndex;
            int id = int.Parse((sender.Columns[0].GetCellContent(args.Row) as TextBlock).Text);
            var client = this.Clients.SingleOrDefault(c => c.Id == id);
            if (client != null) { this.CurrentUDPClient = client; }
        }
        private void RowEditEnding(ExCommandParameter param)
        {
            if (this.CurrentUDPClient == null) { return; }
            var sender = param.Sender as DataGrid;
            var args = param.EventArgs as DataGridRowEditEndingEventArgs;
            int id = int.Parse((sender.Columns[0].GetCellContent(args.Row) as TextBlock).Text);
            var client = this.Clients.SingleOrDefault(c => c.Id == id);
            //保存
            UDPHost.UpdateClient(client);
        }
        private void DeleteClient(ExCommandParameter param)
        {
            var sender = param.Sender as Button;
            int id = int.Parse(sender.Tag.ToString());
            //删除
            if(ModernDialog.ShowMessage("确实要删除该上位机吗?","提示",System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                UDPHost.DeleteClient(id);
                this.Clients.Remove(this.Clients.Single(c => c.Id == id));
            }
            
        }
        private void AddNewClient()
        {
            int id = 0;
            if(this.Clients.Count > 0)
            {
                id = this.Clients.Max(c => c.Id);
            }
            UDPHost client = new UDPHost()
            {
                Id = id + 1,
                IP = "0.0.0.0",
                Port = 3333
            };
            this.Clients.Add(client);
            //保存到配置文件
            UDPHost.AddClient(client);
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
            //串口
            SerialComm comm = new SerialComm();
            this.PortName = PortList.SingleOrDefault(p => p.DisplayName == comm.PortName);
            this.BaudRate = BaudRateList.SingleOrDefault(b => (int)b.Value == comm.BaudRate);
            this.Parity = ParityList.SingleOrDefault(p => (Parity)p.Value == comm.Parity);
            this.DataBits = DataBitList.SingleOrDefault(d => (int)d.Value == comm.DataBits);
            this.StopBits = StopBitList.SingleOrDefault(s => (StopBits)s.Value == comm.StopBits);
            //UDP
            this.UDPServer = UDPServer.GetServerFromConfig();
            //Clients
            this.Clients = UDPHost.GetClients();
            this.ChangeSerialPortParamsCommand = new DelegateCommand<ExCommandParameter>(ChangeSerialPortsParam);
            this.ChangeUDPServerParamsCommand = new DelegateCommand<ExCommandParameter>(ChangeUDPServerParams);
            this.BeginningEditCommand = new DelegateCommand<ExCommandParameter>(BeginningEdit);
            this.RowEditEndingCommand = new DelegateCommand<ExCommandParameter>(RowEditEnding);
            this.DeleteClientCommand = new DelegateCommand<ExCommandParameter>(DeleteClient);
            this.AddNewClientCommand = new DelegateCommand(AddNewClient);
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
