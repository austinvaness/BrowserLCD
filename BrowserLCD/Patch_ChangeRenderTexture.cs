using HarmonyLib;
using Sandbox.Game.Entities.Blocks;
using VRage.Game.Entity;
using VRageMath;

namespace avaness.BrowserLCD
{
    /*[HarmonyPatch(typeof(MyTextPanelComponent))]
    [HarmonyPatch("ChangeRenderTexture")]
    public static class Patch_ChangeRenderTexture
    {
        [HarmonyPrefix]
        public static bool Prefix(object __instance, int area, string path)
        {
            return true;
            var instance = (MyTextPanelComponent)__instance;

            MyEntity m_entity = (MyEntity)AccessTools.Field(typeof(MyTextPanelComponent), "m_block").GetValue(__instance);
            SECEF.log.Log("Change Render Texture " + path + "  " + m_entity.DisplayNameText);
            if (!SECEF.browsers.ContainsKey(new Vector2(m_entity.EntityId, area)) && m_entity.DisplayNameText.Contains("[CEF]"))
            {
                instance.Reset();
            }
            return true;
        }
    }*/
}
