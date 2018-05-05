using FirstFloor.ModernUI.Windows.Controls;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ZSJCMaster.DB;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class AlarmPageViewModel : MainWindowViewModel
    {
        private AlarmLamp alarmLamp;
        private AlarmInfoOperator ope;
        private AlarmInfo currentItem;

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
            ope = new AlarmInfoOperator();
            this.AlarmInfos = new ObservableCollection<AlarmInfo>();
            if (App.Current == null) { return; }
            if(this.ControlPads == null)
            {
                this.ControlPads = ControlPad.GetAllControlPads();
            }
            foreach (var controlpad in this.ControlPads)
            {
                this.Cameras = controlpad.GetCameras();
                try
                {
                    ControlPad pad = new ControlPad(controlpad.Id, (AlarmInfo[] info, bool[] flags) =>
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            for (int i = 0; i < flags.Length; i++)
                            {
                                if (!IsEmpty(info[i]))
                                {

                                    if (controlpad != null)
                                    {
                                        var camera = this.Cameras.SingleOrDefault(c => c.Id == info[i].CameraNo);
                                        if (camera != null)
                                        {
                                            info[i].CameraName = camera.Name;
                                        }

                                    }

                                    AlarmInfos.Add(info[i]);
                                    //向报警器串口发送命令
                                    Task.Run(() => {
                                        alarmLamp.AlarmMusicAndFlash();
                                        Thread.Sleep(2000);
                                        alarmLamp.StopAllAlarm();
                                    });

                                    CurrentItem = info[i];
                                    //写入数据库

                                    ope.Add(ope.AlarmInfoToAlarmInfoForDB(controlpad.Id, info[i]));
                                }
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message,"提示",MessageBoxButton.OK);
                }
                
            }
            

        }

        private bool IsEmpty(AlarmInfo alarmInfo)
        {
            if (alarmInfo.CameraNo != 0 || alarmInfo.X != 0 || alarmInfo.Y != 0 || alarmInfo.Width != 0)
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
