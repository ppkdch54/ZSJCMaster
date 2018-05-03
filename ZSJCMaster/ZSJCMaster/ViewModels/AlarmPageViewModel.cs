﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class AlarmPageViewModel : MainWindowViewModel
    {
        private AlarmInfo currentItem;
        private AlarmLamp alarmLamp;

        /// <summary>
        /// 表格当前项
        /// </summary>
        public AlarmInfo CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                this.RaisePropertyChanged("CurrentItem");
            }
        }

        private ObservableCollection<AlarmInfo> alarmInfos;

        /// <summary>
        /// 报警信息集合
        /// </summary>
        public ObservableCollection<AlarmInfo> AlarmInfos
        {
            get { return alarmInfos; }
            set
            {
                alarmInfos = value;
                this.RaisePropertyChanged("AlarmInfos");
            }
        }

        public AlarmPageViewModel()
        {
            alarmLamp = new AlarmLamp();
            this.AlarmInfos = new ObservableCollection<AlarmInfo>();
            if (App.Current == null) { return; }
            if(this.ControlPads == null)
            {
                this.ControlPads = ControlPad.GetAllControlPads();
            }
            ControlPad pad = new ControlPad((AlarmInfo[] info, bool[] flags) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < flags.Length; i++)
                    {
                        if (!IsEmpty(info[i]))
                        {

                            var controlpad = this.ControlPads.SingleOrDefault(p => p.Id == 1);
                            if (controlpad != null)
                            {
                                var camera = controlpad.GetCameras(controlpad.Id).SingleOrDefault(c => c.No == info[i].cameraNo);
                                info[i].cameraName = camera.Name;
                            }

                            AlarmInfos.Add(info[i]);
                            //向报警器串口发送命令
                            Task.Run(() => {
                                alarmLamp.AlarmMusicAndFlash();
                                Thread.Sleep(2000);
                                alarmLamp.StopAllAlarm();
                            });

                            CurrentItem = info[i];
                        }
                    }
                });
            });

        }

        private bool IsEmpty(AlarmInfo alarmInfo)
        {
            if (alarmInfo.cameraNo != 0 || alarmInfo.x != 0 || alarmInfo.y != 0 || alarmInfo.width != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
