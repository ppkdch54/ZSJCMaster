﻿using FirstFloor.ModernUI.Windows.Controls;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZSJCMaster.DB;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class RecordPageViewModel : MainWindowViewModel
    {
        private bool hasChangedPageSize = false;
        private int currentPageSize;
        #region 属性
        private ControlPad currentControlPad;
        /// <summary>
        /// 当前控制板
        /// </summary>
        public ControlPad CurrentControlPad
        {
            get { return currentControlPad; }
            set
            {
                currentControlPad = value;
                this.RaisePropertyChanged("CurrentControlPad");
            }
        }

        private Camera currentCamera;

        /// <summary>
        /// 当前相机
        /// </summary>
        public Camera CurrentCamera
        {
            get { return currentCamera; }
            set
            {
                currentCamera = value;
                this.RaisePropertyChanged("CurrentCamera");
            }
        }

        private DateTime startTime = DateTime.Now;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                this.RaisePropertyChanged("StartTime");
            }
        }

        private DateTime endTime = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                this.RaisePropertyChanged("EndTime");
            }
        }

        private int totalPageCount = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount
        {
            get { return totalPageCount; }
            set
            {
                totalPageCount = value;
                this.RaisePropertyChanged("TotalPageCount");
            }
        }


        private int pageSize = 40;

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                pageSize = value;
                this.RaisePropertyChanged("PageSize");
            }
        }

        private int currentPageNum = 1;

        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPageNum
        {
            get { return currentPageNum; }
            set
            {
                currentPageNum = value;
                this.RaisePropertyChanged("CurrentPageNum");
            }
        }

        private List<AlarmInfoForDB> alarmInfoList;

        /// <summary>
        /// 报警信息记录
        /// </summary>
        public List<AlarmInfoForDB> AlarmInfoList
        {
            get { return alarmInfoList; }
            set
            {
                alarmInfoList = value;
                this.RaisePropertyChanged("AlarmInfoList");
            }
        }

        private bool enableFirstPage;

        /// <summary>
        /// 启用"第一页"
        /// </summary>
        public bool EnableFirstPage
        {
            get { return enableFirstPage; }
            set
            {
                enableFirstPage = value;
                this.RaisePropertyChanged("EnableFirstPage");
            }
        }

        private bool enableLastPage;

        /// <summary>
        /// 启用"最后一页"
        /// </summary>
        public bool EnableLastPage
        {
            get { return enableLastPage; }
            set
            {
                enableLastPage = value;
                this.RaisePropertyChanged("EnableLastPage");
            }
        }

        private bool enablePrevPage;

        /// <summary>
        /// 启用"上一页"
        /// </summary>
        public bool EnablePrevPage
        {
            get { return enablePrevPage; }
            set
            {
                enablePrevPage = value;
                this.RaisePropertyChanged("EnablePrevPage");
            }
        }

        private bool enableNextPage;

        /// <summary>
        /// 启用"下一页"
        /// </summary>
        public bool EnableNextPage
        {
            get { return enableNextPage; }
            set
            {
                enableNextPage = value;
                this.RaisePropertyChanged("EnableNextPage");
            }
        }

        private Visibility progressBarVisibility = Visibility.Collapsed;

        /// <summary>
        /// 显示等待进度条
        /// </summary>
        public Visibility ProgressBarVisibility
        {
            get { return progressBarVisibility; }
            set
            {
                progressBarVisibility = value;
                this.RaisePropertyChanged("ProgressBarVisibility");
            }
        }


        #endregion

        #region command
        public DelegateCommand<ExCommandParameter> SelectControlPadCommand { get; set; }
        public DelegateCommand QueryRecordCommand { get; set; }
        public DelegateCommand FirstPageCommand { get; set; }
        public DelegateCommand LastPageCommand { get; set; }
        public DelegateCommand PrevPageCommand { get; set; }
        public DelegateCommand NextPageCommand { get; set; }
        public DelegateCommand<ExCommandParameter> ChangePageSizeCommand {get;set;}
        #endregion

        #region command function
        private void SelectControlPad(ExCommandParameter param)
        {
            var sender = param.Sender as ComboBox;
            var controlpad = sender.SelectedItem as ControlPad;
            if (controlpad == null) { return; }
            this.CurrentControlPad = controlpad;
            var cameras = controlpad.GetCameras();
            if (cameras != null)
            {
                cameras.Insert(0, new Camera() { Id = 0, Name = "==全部==" });
                CurrentCamera = cameras[0];
            }
            this.Cameras = cameras;
            
        }

        AlarmInfoOperator ope = new AlarmInfoOperator();
        private void QueryRecord()
        {
            if (hasChangedPageSize)
            {
                currentPageSize = this.PageSize;
                hasChangedPageSize = false;
            }
            CurrentPageNum = 1;
            Query();
        }

        private void Query()
        {
            this.ProgressBarVisibility = Visibility.Visible;
            this.AlarmInfoList = null;
            Task.Run(()=> 
            {
                int controlpadId = -1;
                int cameraId = -1;
                if (CurrentControlPad != null)
                {
                    controlpadId = CurrentControlPad.Id;
                }
                if (CurrentCamera != null)
                {
                    cameraId = CurrentCamera.Id;
                }
                if (controlpadId == 0)
                {
                    controlpadId = -1;
                    cameraId = -1;
                }
                else
                {
                    if (cameraId == 0)
                    {
                        cameraId = -1;
                    }
                }
                int totalRecord = 0;
                try
                {
                    //System.Threading.Thread.Sleep(10000);
                    this.AlarmInfoList = ope.Query(StartTime, EndTime, currentPageSize, CurrentPageNum, out totalRecord, controlpadId, cameraId);
                    this.TotalPageCount = totalRecord % currentPageSize == 0 ? totalRecord / currentPageSize : totalRecord / currentPageSize + 1;
                    this.CurrentPageNum = this.TotalPageCount > 0 ? this.CurrentPageNum : 0;
                    this.EnablePrevPage = this.CurrentPageNum > 1;
                    this.EnableNextPage = this.CurrentPageNum != this.TotalPageCount;
                    this.EnableFirstPage = this.CurrentPageNum > 1;
                    this.EnableLastPage = this.CurrentPageNum < this.TotalPageCount;
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "提示", System.Windows.MessageBoxButton.OK);
                }
                finally
                {
                    this.ProgressBarVisibility = Visibility.Collapsed;
                }
            });
            
            
        }

        private void FirstPage()
        {
            ChangeCurrentPage(1);
        }
        private void LastPage()
        {
            ChangeCurrentPage(this.TotalPageCount);
        }
        private void PrevPage()
        {
            ChangeCurrentPage(CurrentPageNum - 1);
        }
        private void NextPage()
        {
            ChangeCurrentPage(CurrentPageNum + 1);
        }
        private void ChangeCurrentPage(int pageNum)
        {
            CurrentPageNum = pageNum;
            Query();
        }
        private void ChangePageSize(ExCommandParameter param)
        {
            hasChangedPageSize = true;
        }
        #endregion
        public RecordPageViewModel()
        {
            var controlpads = ControlPad.GetAllControlPads();
            if(controlpads != null && controlpads.Count > 0)
            {
                controlpads.Insert(0, new ControlPad() { Id = 0, Name = "==全部==" });
                this.ControlPads = controlpads;
                CurrentControlPad = this.ControlPads[0];
            }
            currentPageSize = this.PageSize;
            this.SelectControlPadCommand = new DelegateCommand<ExCommandParameter>(SelectControlPad);
            this.QueryRecordCommand = new DelegateCommand(QueryRecord);
            this.FirstPageCommand = new DelegateCommand(FirstPage);
            this.LastPageCommand = new DelegateCommand(LastPage);
            this.PrevPageCommand = new DelegateCommand(PrevPage);
            this.NextPageCommand = new DelegateCommand(NextPage);
            this.ChangePageSizeCommand = new DelegateCommand<ExCommandParameter>(ChangePageSize);
        }
    }
}
