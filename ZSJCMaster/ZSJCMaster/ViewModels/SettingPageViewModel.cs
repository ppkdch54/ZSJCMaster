﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class SettingPageViewModel: MainWindowViewModel
    {
        private DataGrid dgvControlPads;
        private ControlPad currentControlPad;
        #region commands
        public DelegateCommand<ExCommandParameter> DataGridLoadedCommand { get; set; }
        public DelegateCommand<ExCommandParameter> BeginningEditCommand { get; set; }
        public DelegateCommand<ExCommandParameter> CellEditEndingCommand { get; set; }
        public DelegateCommand<ExCommandParameter> RowEditEndingCommand { get; set; }
        public DelegateCommand<ExCommandParameter> EditControlPadCommand { get; set; }
        public DelegateCommand AddNewControlPadCommand { get; set; }
        #endregion commands

        #region command functions
        private void DataGridLoaded(ExCommandParameter param)
        {
            this.dgvControlPads = param.Sender as DataGrid;
        }
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
            ControlPad pad = new ControlPad() {
                Id = id + 1,
                Name = "控制板" + (id + 1),
                IP = "0.0.0.0",
                PortNum = 3333
            };
            this.ControlPads.Add(pad);
            //保存到配置文件
            ControlPad.AddControlPad(pad);
        }

        public void CellEditEnding(ExCommandParameter param)
        {
            //该方法有问题，建议使用WPF验证机制
            if (this.currentControlPad == null) { return; }
            var sender = param.Sender as DataGrid;
            var args = param.EventArgs as DataGridCellEditEndingEventArgs;
            int index = args.Column.DisplayIndex;
            var content = (args.Column.GetCellContent(args.Row) as TextBox).Text;
            int id = int.Parse((sender.Columns[0].GetCellContent(args.Row) as TextBlock).Text);
            var pad = this.ControlPads.SingleOrDefault(p => p.Id == id);
            
            string header = sender.Columns[index].Header.ToString();
            if (string.IsNullOrEmpty(content))
            {      
                switch (header)
                {
                    case "Name":
                        args.Column.SetCurrentValue(TextBox.TextProperty,currentControlPad.Name);
                        break;
                    case "IP":
                        args.Column.SetCurrentValue(TextBox.TextProperty,currentControlPad.IP);
                        break;
                    case "PortNum":
                        args.Column.SetCurrentValue(TextBox.TextProperty, currentControlPad.PortNum);
                        break;
                    default:
                        args.Column.SetCurrentValue(TextBox.TextProperty,"");
                        break;
                }
                return;
            }
            if(header == "PortNum")
            {
                int port = 0;
                bool b = int.TryParse(content, out port);
                if (b)
                {
                    if(port > 0 && port <= 65535)
                    {
                        args.Column.SetCurrentValue(TextBox.TextProperty, port);
                    }else
                    {
                        args.Column.SetCurrentValue(TextBox.TextProperty, currentControlPad.PortNum);
                    }
                }else
                {
                    args.Column.SetCurrentValue(TextBox.TextProperty, currentControlPad.PortNum);
                }
                
                return;
            }
            if(header == "IP")
            {
                string pattern = @"^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$";
                if(Regex.IsMatch(content,pattern))
                {
                    args.Column.SetCurrentValue(TextBox.TextProperty, content);
                    
                }else
                {
                    pad.IP = currentControlPad.IP;
                    args.Column.SetCurrentValue(TextBox.TextProperty, currentControlPad.IP);
                }
                return;
            }

        }

        private void RowEditEnding(ExCommandParameter param)
        {
            //var sender = param.Sender as DataGrid;
            //var args = param.EventArgs as DataGridRowEditEndingEventArgs;
            //var pad = args.Row.Item as ControlPad;
            //ControlPad.AddControlPad(pad);      
        }

        private void EditControlPad(ExCommandParameter param)
        {
            var sender = param.Sender as Button;
            int id = int.Parse(sender.Tag.ToString());
            if (this.dgvControlPads == null) { return; }
            this.dgvControlPads.CurrentItem = this.ControlPads.SingleOrDefault(p => p.Id == id);
            this.dgvControlPads.SelectedItem = this.dgvControlPads.CurrentItem;
            for (int i = 1; i < dgvControlPads.Columns.Count-1; i++)
            {
                dgvControlPads.Columns[i].IsReadOnly = false;
            }
            dgvControlPads.SelectionUnit = DataGridSelectionUnit.Cell;
            dgvControlPads.SelectionMode = DataGridSelectionMode.Single;
       
            
        }
        #endregion command functions

        public SettingPageViewModel()
        {
            this.ControlPads = ControlPad.GetAllControlPads();
            this.DataGridLoadedCommand = new DelegateCommand<ExCommandParameter>(DataGridLoaded);
            this.BeginningEditCommand = new DelegateCommand<ExCommandParameter>(BeginningEdit);
            this.CellEditEndingCommand = new DelegateCommand<ExCommandParameter>(CellEditEnding);
            this.RowEditEndingCommand = new DelegateCommand<ExCommandParameter>(RowEditEnding);
            this.AddNewControlPadCommand = new DelegateCommand(AddNewControlPad);
            this.EditControlPadCommand = new DelegateCommand<ExCommandParameter>(EditControlPad);
        }


    }
}
