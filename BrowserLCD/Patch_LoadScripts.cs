using System.Reflection;
using HarmonyLib;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using Sandbox.Game.World;
using VRage.Game;
using Sandbox.ModAPI;

namespace avaness.BrowserLCD
{
    [HarmonyPatch(typeof(MyTextSurfaceScriptFactory), "LoadScripts")]
    public static class Patch_LoadScripts
    {
        [HarmonyPriority(Priority.Low)]
        public static void Postfix()
        {
            SECEF.log.Log("Added TSS");
            if (MySession.Static.OnlineMode == MyOnlineModeEnum.OFFLINE)
            {
                MyTextSurfaceScriptFactory.Instance.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            }
            SECEF.allowMultiplayer = false;
            MyAPIUtilities.Static.MessageEntered -= MessageHandler;
            MyAPIUtilities.Static.MessageEntered += MessageHandler;
        }
        private static void MessageHandler(string message, ref bool sendToOthers)
        {
            if (message == "/secef ihaveabadfeelingaboutthis")
            {
                if (!MyTextSurfaceScriptFactory.Instance.Scripts.ContainsKey("TSS_CEF"))
                {
                    MyTextSurfaceScriptFactory.Instance.RegisterFromAssembly(Assembly.GetExecutingAssembly());
                    MyAPIGateway.Utilities.ShowMessage("[SECEF]", "SECEF enabled. Look for the script on an LCD control panel");
                }
                SECEF.allowMultiplayer = true;
            }
            else if (message == "/secef audio off")
            {
                SECEF.AudioSetting = 0;
                MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Audio disabled");
            }
            else if (message == "/secef audio global")
            {
                SECEF.AudioSetting = 1;
                MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Audio set to global");
            }
            else if (message == "/secef audio local")
            {
                SECEF.AudioSetting = 2;
                MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Audio set to local. This may crash your game.");
            }
            else if (message == "/secef audio unsafe")
            {
                SECEF.AudioSetting = 3;
                MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Audio set to unsafe. local with less jitters but will almost definitely crash your game.");
            }
            else if (message == "/secef clicklog on")
            {
                SECEF.clickLog = true;
                MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Click log on.");
            }
            else if (message == "/secef clicklog off")
            {
                SECEF.clickLog = false;
                MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Click log off.");
            }
            if (message.StartsWith("/secef"))
            {
                sendToOthers = false;
            }
        }
    }
}
