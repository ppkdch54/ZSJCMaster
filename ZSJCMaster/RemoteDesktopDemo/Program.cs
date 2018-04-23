using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDesktopDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Process p = Process.Start("mstsc.exe");
            p.WaitForExit();//关键，等待外部程序退出后才能往下执行
        }
    }
}
