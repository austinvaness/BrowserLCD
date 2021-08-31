using System;
using HarmonyLib;

namespace avaness.BrowserLCD
{
    public static class Patch_OnStopPlayingBuffered
    {
        public static bool Prefix(object __instance, IntPtr context)
        {
            if (__instance == null)
                return false;

            var dataStreams = AccessTools.Field(__instance.GetType(), "m_dataStreams").GetValue(__instance);
            if (dataStreams == null)
            {
                var f = AccessTools.Method(__instance.GetType(), "OnAllBuffersFinished");
                if (f != null)
                    f.Invoke(__instance, new object[] { });
                return false;
            }
            return true;
        }
    }
}
