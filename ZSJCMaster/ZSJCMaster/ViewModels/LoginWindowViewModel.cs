using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZSJCMaster.Models;

namespace ZSJCMaster.ViewModels
{
    class LoginWindowViewModel: BindableBase
    {
        private User user;

        public User User
        {
            get { return user; }
            set
            {
                user = value;
                this.RaisePropertyChanged("User");
            }
        }

        public DelegateCommand<Window> LoginCommand { get; set; }
        private void Login(Window window)
        {
            if (User.UserName.ToLower().Equals("admin") && User.Password.Equals("123456"))
            {
                user.State = true;
                window.Close();
            }
            else
            {

                //string errorTitle = Application.Current.FindResource("ErrorTitle").ToString();
                //string errorInfo = Application.Current.FindResource("UserNameOrPasswordError").ToString();
                MessageBox.Show("用户名或密码错误", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public LoginWindowViewModel()
        {
            this.LoginCommand = new DelegateCommand<Window>(Login);
            this.User = User.GetInstance();
        }
    }
}
