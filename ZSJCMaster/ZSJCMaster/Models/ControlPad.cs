using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using ZSJCMaster.Helpers;

namespace ZSJCMaster.Models
{
    public class ControlPad: BindableBase
    {
        private List<Camera> cameras;

        public List<Camera> Cameras
        {
            get { return cameras; }
            set { cameras = value; }
        }

        //构造函数,打开串口
        public ControlPad()
        {

        }


    }
}
