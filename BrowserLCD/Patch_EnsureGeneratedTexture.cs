using System;
using HarmonyLib;
using Sandbox.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRageMath;
using VRage.Game.Entity;
using Sandbox.Game.Entities.Blocks;

namespace avaness.BrowserLCD
{
    [HarmonyPatch(typeof(MyTextPanelComponent))]
    [HarmonyPatch("EnsureGeneratedTexture")]
    public static class Patch_EnsureGeneratedTexture
    {
        public static Guid guid = new Guid("74DE02B3-27F9-4960-B1C4-27351F2B06D1");

        [HarmonyPriority(Priority.Low)]
        [HarmonyPrefix]
        public static bool Prefix(object __instance)
        {
            var instance = (MyTextPanelComponent)__instance;

            MyEntity m_entity = (MyEntity)AccessTools.Field(typeof(MyTextPanelComponent), "m_block").GetValue(__instance);
            int m_area = (int)AccessTools.Field(typeof(MyTextPanelComponent), "m_area").GetValue(__instance);
            if (!SECEF.browsers.ContainsKey(new Vector2(m_entity.EntityId, m_area)) || (SECEF.browsers[new Vector2(m_entity.EntityId, m_area)].persist && SECEF.browsers[new Vector2(m_entity.EntityId, m_area)].textureName == null))/* && m_entity.DisplayNameText.Contains("[CEF]")*/
            {
                if (instance.ContentType == ContentType.SCRIPT && instance.Script == "TSS_CEF")
                {
                    SECEF.log.Log("EnsureGenTex reset " + m_entity.DisplayNameText);
                    instance.Reset();
                    //AccessTools.Field(typeof(MyTextPanelComponent), "m_textureGenerated").SetValue(__instance, false);
                    var textureSize = (Vector2I)AccessTools.Field(typeof(MyTextPanelComponent), "m_textureSize").GetValue(__instance);
                    SECEF_Patch_CreateTexture.Prefix((MyRenderComponentScreenAreas)AccessTools.Field(typeof(MyTextPanelComponent), "m_render").GetValue(__instance), m_area, textureSize);
                    AccessTools.Field(typeof(MyTextPanelComponent), "m_textureGenerated").SetValue(__instance, true);
                    if (m_entity.Storage != null)
                    {
                        //var text = m_entity.Storage.GetValue(guid);
                        //SECEF.log.Log("text: " + text);
                    }
                    /*if(m_area == 0 && text != null && text.Length>0)
                    {
                        Patch_SetCustomData.Prefix(m_entity, text, true);
                    }*/
                    return false;
                }
            }
            else if ((instance.ContentType != ContentType.SCRIPT || instance.Script != "TSS_CEF"))
            {
                //AccessTools.Field(typeof(MyTextPanelComponent), "m_textureGenerated").SetValue(__instance, false);
                //instance.Reset();
                AccessTools.Method(typeof(MyTextPanelComponent), "ReleaseTexture").Invoke(instance, new object[] { true });
                AccessTools.Field(typeof(MyTextPanelComponent), "m_textureGenerated").SetValue(__instance, false);
                //Patch_ReleaseTexture.Prefix((MyRenderComponentScreenAreas)AccessTools.Field(typeof(MyTextPanelComponent), "m_render").GetValue(__instance), m_area);
            }
            return true;
        }
    }
}
