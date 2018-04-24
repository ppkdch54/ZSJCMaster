using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ZSJCMaster.Validations
{
    class PortRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int i = 0;
            if(!int.TryParse(value.ToString(),out i))
            {
                return new ValidationResult(false, "端口必须是数字!");
            }
            if(i<0 || i > 65535)
            {
                return new ValidationResult(false,"端口必须介于0-65535之间!");
            }else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
