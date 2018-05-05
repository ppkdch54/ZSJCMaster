using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class CameraContentViewModel:SettingPageViewModel
    {
        private ControlPad currentControlPad;
        public ControlPad CurrentControlPad
        {
            get { return currentControlPad; }
            set
            {
                currentControlPad = value;
                this.RaisePropertyChanged("CurrentControlPad");
            }
        }

        #region command
        public DelegateCommand<ExCommandParameter> PageLoadedCommand { get; set; }
        public DelegateCommand<ExCommandParameter> RowEditEndingCommand { get; set; }
        public DelegateCommand AddNewCameraCommand { get; set; }
        public DelegateCommand<ExCommandParameter> SelectControlPadCommand { get; set; }
        public DelegateCommand<ExCommandParameter> SelectAlarmPicPathCommand { get; set; }
        public DelegateCommand<ExCommandParameter> DeleteCameraCommand { get; set; }
        #endregion

        #region command functions
        private void PageLoaded(ExCommandParameter param)
        {
            this.ControlPads = ControlPad.GetAllControlPads();
       
        }
        private void SelectControlPad(ExCommandParameter param)
        {
            var sender = param.Sender as ComboBox;
            var controlpad = sender.SelectedItem as ControlPad;
            if (controlpad == null) { return; }
            this.CurrentControlPad = controlpad;
            this.Cameras = controlpad.GetCameras();
        }
        private void SelectAlarmPicPath(ExCommandParameter param)
        {
            var sender = param.Sender as Button;
            int id = int.Parse(sender.Tag.ToString());
            var camera = this.Cameras.SingleOrDefault(c => c.Id == id);
            if (camera == null) { return; }
            //显示选择对话框
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = camera.AlarmPicDir;
            dialog.ShowDialog();
            camera.AlarmPicDir = dialog.SelectedPath;
        }

        private void RowEditing(ExCommandParameter param)
        {
            var sender = param.Sender as DataGrid;
            var args = param.EventArgs as DataGridRowEditEndingEventArgs;
            int id = int.Parse((sender.Columns[0].GetCellContent(args.Row) as TextBlock).Text);
            var camera = this.Cameras.SingleOrDefault(c => c.Id == id);
            if (camera == null) { return; }
            Camera.UpdateCamera(camera);
        }
        private void AddNewCamera()
        {
            int maxId = 0;
            int maxPort = 0;
            int beltNo = 1;
            string alarmPic = "C:\\Alarm_Pic";
            if (this.Cameras.Count > 0)
            {
                maxId = this.Cameras.Max(p => p.Id);
                maxPort = this.Cameras.Max(p => p.NetPortNum);
                beltNo = this.Cameras.Single(p => p.Id == maxId).BeltNo;
                alarmPic = this.Cameras.Single(p => p.Id == maxId).AlarmPicDir;
            }
            Camera camera = new Camera();
            camera.Id = maxId + 1;
            camera.ControlPadNo = this.CurrentControlPad.Id;
            camera.NetPortNum = maxPort + 1;
            camera.BeltNo = beltNo;
            camera.AlarmPicDir = alarmPic;
            this.Cameras.Add(camera);
            Camera.AddCamera(camera);
        }
        private void DeleteCamera(ExCommandParameter param)
        {
            var sender = param.Sender as Button;
            int cameraId = int.Parse(sender.Tag.ToString());
            int controlpadId = this.CurrentControlPad.Id;
            //删除
            bool b = Camera.DeleteCamera(cameraId, controlpadId);
            if (b)
            {
                this.Cameras.Remove(this.Cameras.Single(p => p.Id == cameraId));
            }
        }
        #endregion
        public CameraContentViewModel()
        {
            this.PageLoadedCommand = new DelegateCommand<ExCommandParameter>(PageLoaded);
            this.RowEditEndingCommand = new DelegateCommand<ExCommandParameter>(RowEditing);
            this.AddNewCameraCommand = new DelegateCommand(AddNewCamera);
            this.SelectControlPadCommand = new DelegateCommand<ExCommandParameter>(SelectControlPad);
            this.SelectAlarmPicPathCommand = new DelegateCommand<ExCommandParameter>(SelectAlarmPicPath);
            this.DeleteCameraCommand = new DelegateCommand<ExCommandParameter>(DeleteCamera);
        }

    }
}
