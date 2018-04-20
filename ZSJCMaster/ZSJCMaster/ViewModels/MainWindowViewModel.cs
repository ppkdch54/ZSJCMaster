using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class MainWindowViewModel: BindableBase
    {
        private ControlPad controlPad;
        /// <summary>
        /// 控制板
        /// </summary>
        public ControlPad ControlPad
        {
            get { return controlPad; }
            set
            {
                controlPad = value;
                this.RaisePropertyChanged("ControlPad");
            }
        }

        public MainWindowViewModel()
        {
            this.ControlPad = new ControlPad();
            ControlPad.Name = "控制板";
            ControlPad.Cameras = new List<Camera>()
            {
                new Camera() { No=1, Name="左相机", IP="192.168.1.102", BeltNo=1, NetPortNum=1, AlarmPicDir="C:\\" },
                new Camera() { No=2, Name="右相机", IP="192.168.1.102", BeltNo=1, NetPortNum=2, AlarmPicDir="C:\\" },
                new Camera() { No=2, Name="右相机", IP="192.168.1.102", BeltNo=1, NetPortNum=2, AlarmPicDir="C:\\" },
                new Camera() { No=2, Name="右相机", IP="192.168.1.102", BeltNo=1, NetPortNum=2, AlarmPicDir="C:\\" },
            };

        }

    }
}
