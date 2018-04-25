using Prism.Commands;
using System.Linq;
using System.Windows.Controls;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class ControlPadContentViewModel: SettingPageViewModel
    {
        private ControlPad currentControlPad;
        #region commands
        public DelegateCommand<ExCommandParameter> BeginningEditCommand { get; set; }
        public DelegateCommand<ExCommandParameter> RowEditEndingCommand { get; set; }
        public DelegateCommand<ExCommandParameter> DeleteControlPadCommand { get; set; }
        public DelegateCommand AddNewControlPadCommand { get; set; }
        #endregion commands

        #region command functions
        private void BeginningEdit(ExCommandParameter param)
        {
            var sender = param.Sender as DataGrid;
            var args = param.EventArgs as DataGridBeginningEditEventArgs;
            int index = args.Column.DisplayIndex;
            int id = int.Parse((sender.Columns[0].GetCellContent(args.Row) as TextBlock).Text);
            var pad = this.ControlPads.SingleOrDefault(p => p.Id == id);
            if (pad != null) { this.currentControlPad = pad; }
        }
        private void AddNewControlPad()
        {
            int id = this.ControlPads.Max(p => p.Id);
            ControlPad pad = new ControlPad()
            {
                Id = id + 1,
                Name = "控制板" + (id + 1),
                IP = "0.0.0.0",
                PortNum = 3333
            };
            this.ControlPads.Add(pad);
            //保存到配置文件
            ControlPad.AddControlPad(pad);
        }

        private void RowEditEnding(ExCommandParameter param)
        {
            if (this.currentControlPad == null) { return; }
            var sender = param.Sender as DataGrid;
            var args = param.EventArgs as DataGridRowEditEndingEventArgs;
            int id = int.Parse((sender.Columns[0].GetCellContent(args.Row) as TextBlock).Text);
            var pad = this.ControlPads.SingleOrDefault(p => p.Id == id);
            //保存
            ControlPad.UpdateControlPad(pad);
        }

        private void DeleteControlPad(ExCommandParameter param)
        {
            var sender = param.Sender as Button;
            int id = int.Parse(sender.Tag.ToString());
            //删除
            ControlPad.DeleteControlPad(id);
        }
        #endregion command functions

        public ControlPadContentViewModel()
        {
            this.ControlPads = ControlPad.GetAllControlPads();
            this.BeginningEditCommand = new DelegateCommand<ExCommandParameter>(BeginningEdit);
            this.RowEditEndingCommand = new DelegateCommand<ExCommandParameter>(RowEditEnding);
            this.AddNewControlPadCommand = new DelegateCommand(AddNewControlPad);
            this.DeleteControlPadCommand = new DelegateCommand<ExCommandParameter>(DeleteControlPad);
        }
    }
}
