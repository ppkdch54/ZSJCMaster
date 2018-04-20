﻿using Prism.Mvvm;
using System;
using ZSJCMaster.Helpers;

namespace ZSJCMaster.Models
{

    public class Camera : BindableBase
    {
        private int no;

        /// <summary>
        /// 相机编号
        /// </summary>
        public int No
        {
            get { return no; }
            set
            {
                no = value;
                this.RaisePropertyChanged("No");
            }
        }


        private string name;

        /// <summary>
        /// 相机名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        private string ip;

        /// <summary>
        /// 相机IP
        /// </summary>
        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                this.RaisePropertyChanged("IP");
            }
        }

        private int beltNo;

        /// <summary>
        /// 皮带编号
        /// </summary>
        public int BeltNo
        {
            get { return beltNo; }
            set
            {
                beltNo = value;
                this.RaisePropertyChanged("BeltNo");
            }
        }

        private int netPortNum;

        /// <summary>
        /// 网口编号
        /// </summary>
        public int NetPortNum
        {
            get { return netPortNum; }
            set
            {
                netPortNum = value;
                this.RaisePropertyChanged("NetPortNum");
            }
        }

        private string alarmPicDir;

        /// <summary>
        /// 上位机报警图片存储路径
        /// </summary>
        public string AlarmPicDir
        {
            get { return alarmPicDir; }
            set
            {
                alarmPicDir = value;
                this.RaisePropertyChanged("AlarmPicDir");
            }
        }

        public Camera()
        {
            LoadPara();
        }

        private void LoadPara()
        {
            //XmlConfigHelper config = new XmlConfigHelper();
            //config.Load("Application.config");
            //No = int.Parse(config.ReadNodeValue("no"));
        }

        public void SavePara()
        {

        }
    }
}
