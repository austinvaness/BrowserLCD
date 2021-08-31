using HarmonyLib;
using VRageMath;
using Sandbox.Game.Entities.Cube;
using VRage.Game.ModAPI.Ingame;

namespace avaness.BrowserLCD
{
    /*[HarmonyPatch(typeof(MyEntity3DSoundEmitter), "PlaySound", new Type[] { typeof(byte[]), typeof(int), typeof(int), typeof(float), typeof(float), typeof(MySoundDimensions) })]
    public static class Patch_PlaySound
    {
        public static bool Prefix(byte[] buffer, int size, int sampleRate, float volume = 1f, float maxDistance = 0f, MySoundDimensions dimension = MySoundDimensions.D3)
        {
            //SECEF.log.Log("play sound patch " + (buffer == null) + " " + size + " " + sampleRate + " " + (MyAudio.Static == null));
            if(buffer == null || size == 0 || MyAudio.Static == null)
            {
                return false;
            }
            return true;
        }

    }*/

    [HarmonyPatch(typeof(MyTerminalBlock), "SetCustomData_Internal")]
    public static class Patch_SetCustomData
    {
        [HarmonyPriority(Priority.Low)]
        public static void Postfix(MyTerminalBlock __instance, string value, bool sync)
        {
            var entity = (IMyEntity)__instance;
            var id = 0;
            if (value == null || value.Length == 0)
            {
                return;
            }

            var myId = new Vector2(entity.EntityId, id);
            if (!sync)
            {
                return;
            }
            foreach (var br in SECEF.browsers)
            {
                if (br.Value.entity.EntityId == entity.EntityId)
                {
                    //br.Value.ProcessCommands(value);
                    SECEF_Patch_CreateTexture.UpdateCustomData(__instance);
                }
            }
        }
    }
}
