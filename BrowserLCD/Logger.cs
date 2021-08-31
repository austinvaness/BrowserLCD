using System;
using VRage.Utils;

namespace avaness.BrowserLCD
{
    public class Logger
    {
        internal void Log(string v)
        {
            MyLog.Default.WriteLine("[BrowserLCD] " + v);
        }
    }
}