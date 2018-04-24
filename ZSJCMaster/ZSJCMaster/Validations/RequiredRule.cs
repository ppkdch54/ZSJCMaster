using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ZSJCMaster.Validations
{
    class RequiredRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(value == null)
            {
                return new ValidationResult(false,"该参数不能为空!");
            }
            if(string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false,"该参数不能为空!");
            }else
            {
                return new ValidationResult(true,null);
            }
        }
    }
}
