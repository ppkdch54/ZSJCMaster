using FirstFloor.ModernUI.Windows.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ZSJCMaster.Helpers;

namespace ZSJCMaster.ViewModels
{
    class SetupWizardWindowViewModel: BindableBase
    {
        private int currentIndex;
        private bool enableBackwardButton;

        /// <summary>
        /// 是否启用“后退”按钮
        /// </summary>
        public bool EnableBackwardButton
        {
            get { return enableBackwardButton; }
            set
            {
                enableBackwardButton = value;
                this.RaisePropertyChanged("EnableBackwardButton");
            }
        }

        private string forwardButtonText = "前进 ->";

        /// <summary>
        /// “前进”按钮文字
        /// </summary>
        public string ForwardButtonText
        {
            get { return forwardButtonText; }
            set
            {
                forwardButtonText = value;
                this.RaisePropertyChanged("ForwardButtonText");
            }
        }

        public DelegateCommand<ExCommandParameter> ForwardSetupCommand { get; set; }
        public DelegateCommand<ExCommandParameter> BackwardSetupCommand { get; set; }
        public DelegateCommand<ExCommandParameter> ExitSetupCommand { get; set; }

        private void ForwardSetup(ExCommandParameter param)
        {
            var tabs = param.Parameter as ModernTab;
            currentIndex++;
            if (currentIndex == tabs.Links.Count)
            {
                #if !DEBUG
                //修改配置不再显示设置向导
                XDocument doc = XDocument.Load("Application.config");
                doc.Descendants("showSetupWizard").Single().SetAttributeValue("show", "false");
                doc.Save("Application.config");
                #endif
                //重启软件
                MessageBox.Show("将立即重启以使设置生效，稍后您可以通过系统设置页面更改设置!", "提示", MessageBoxButton.OK,MessageBoxImage.Information);
                Application.Current.Shutdown();
                System.Windows.Forms.Application.Restart();
                return;
            }
            tabs.SelectedSource = tabs.Links[currentIndex].Source;
            SetButtonState(tabs.Links.Count);
        }

        private void BackwardSetup(ExCommandParameter param)
        {
            var tabs = param.Parameter as ModernTab;
            currentIndex--;
            tabs.SelectedSource = tabs.Links[currentIndex].Source;
            SetButtonState(tabs.Links.Count);
        }

        private void SetButtonState(int totalCount)
        {
            currentIndex = currentIndex == totalCount - 1 ? totalCount - 1 : currentIndex;
            ForwardButtonText = currentIndex == totalCount - 1 ? "完成" : "前进 ->";
            EnableBackwardButton = currentIndex > 0;
        }
        private void ExitSetup(ExCommandParameter param)
        {
            var window = param.Parameter as Window;
            window.Close();

        }

        public SetupWizardWindowViewModel()
        {
            this.ForwardSetupCommand = new DelegateCommand<ExCommandParameter>(ForwardSetup);
            this.BackwardSetupCommand = new DelegateCommand<ExCommandParameter>(BackwardSetup);
            this.ExitSetupCommand = new DelegateCommand<ExCommandParameter>(ExitSetup);
        }
    }
}
