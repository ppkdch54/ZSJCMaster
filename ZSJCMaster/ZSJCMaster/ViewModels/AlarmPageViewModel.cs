using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class AlarmPageViewModel:MainWindowViewModel
    {
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
            this.AlarmInfos = new ObservableCollection<AlarmInfo>();
            ControlPad pad = new ControlPad((AlarmInfo[] info,bool[] flags)=> 
            {
                App.Current.Dispatcher.Invoke(()=> 
                {
                    for (int i = 0; i < flags.Length; i++)
                    {
                        if (flags[i])
                        {
                            AlarmInfos.Add(info[i]);
                            CurrentItem = info[i];
                        }
                    }
                });
                
            });
            
            
        }

    }
}
