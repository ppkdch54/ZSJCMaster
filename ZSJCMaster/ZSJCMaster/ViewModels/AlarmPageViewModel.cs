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
            ControlPad pad = new ControlPad();
            this.AlarmInfos = pad.AlarmInfos;
        }

    }
}
