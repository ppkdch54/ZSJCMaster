using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ZSJCMaster.Validations
{
    class PathRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(!Directory.Exists(value.ToString()))
            {
                return new ValidationResult(false, "非法路径!");
            }else
            {
                return new ValidationResult(true,null);
            }
        }
    }
}
