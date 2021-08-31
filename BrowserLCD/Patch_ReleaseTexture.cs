using HarmonyLib;
using Sandbox.Game.Components;
using VRageMath;
using VRage.Game.Entity;

namespace avaness.BrowserLCD
{
    [HarmonyPatch(typeof(MyRenderComponentScreenAreas))]
    [HarmonyPatch("ReleaseTexture")]
    public static class Patch_ReleaseTexture
    {
        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(MyRenderComponentScreenAreas __instance, int area)
        {
            SECEF.log.Log("Releaseing texture " + __instance);
            MyEntity m_entity = (MyEntity)AccessTools.Field(typeof(MyRenderComponentScreenAreas), "m_entity").GetValue(__instance);
            var id = m_entity.EntityId;
            var myId = new Vector2(id, area);
            if (SECEF.browsers.ContainsKey(myId))
            {
                if (SECEF.browsers[myId].persist)
                {
                    SECEF.browsers[myId].textureName = null;
                    SECEF.log.Log("Persisting browser " + myId);
                }
                else
                {
                    SECEF.browsers[myId].browser.Dispose();
                    SECEF.browsers[myId].browser = null;
                    SECEF.browsers.Remove(myId);
                    SECEF.log.Log("Removing Browser " + myId);
                }
            }
            return true;
        }
    }
}
