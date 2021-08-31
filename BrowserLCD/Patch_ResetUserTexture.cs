using System;
using System.Reflection;
using HarmonyLib;

namespace avaness.BrowserLCD
{
    public static class Patch_ResetUserTexture
    {
        /* Fixes a crash due to missing format */
        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(object texture, byte[] data)
        {
            SECEF.userTexture = texture;
            var mgt = SECEF.mgti;
            //SECEF.log.Log("RUT: " + mgt);
            MethodInfo m = AccessTools.Method(SECEF.mgtm, "Reset", new Type[] { mgt, typeof(byte[]), typeof(int) });
            m.Invoke(null, new object[] { texture, data, 4 });

            return false;
        }
    }
}
