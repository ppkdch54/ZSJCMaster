using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ZSJCMaster.Validations
{
    class IPv4Rule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string pattern = @"^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$";
            if (!Regex.IsMatch(value.ToString(), pattern))
            {
                return new ValidationResult(false, "不是正确的IP地址!");

            }else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
