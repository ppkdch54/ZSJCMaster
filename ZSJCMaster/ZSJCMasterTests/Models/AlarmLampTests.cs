using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZSJCMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZSJCMaster.Models.Tests
{
    [TestClass()]
    public class AlarmLampTests
    {
        [TestMethod()]
        public void AlarmMusicAndFlashTest()
        {
            AlarmLamp alarmLamp = new AlarmLamp();
            for (int i = 0; i < 5; i++)
            {
                alarmLamp.AlarmMusicAndFlash();
                Thread.Sleep(500);
                alarmLamp.StopAllAlarm();
            }
        }
    }
}