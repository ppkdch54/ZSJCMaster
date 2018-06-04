 using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSJCMaster.Models
{
    public class User: BindableBase
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool State { get; set; }

        private static User user = new User();
        private User()
        {
            
        }

        public static User GetInstance()
        {
            return user;
        }
        public static User GetAdmin()
        {
            user.UserName = "admin";
            user.Password = "123456";
            return user;
        }

    }
}
