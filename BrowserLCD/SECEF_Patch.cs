using HarmonyLib;
using Sandbox.Game.Components;
using VRage.Collections;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace avaness.BrowserLCD
{
    /*[HarmonyPatch(typeof(MyRenderComponentScreenAreas))]
    [HarmonyPatch("RenderSpritesToTexture")]
    public static class SECEF_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(int area, ListReader<MySprite> sprites, Vector2I textureSize, Vector2 aspectRatio, VRageMath.Color backgroundColor, byte backgroundAlpha)
        {
            SECEF.log.Log("RenderSrpitestoTextture");
            return true;
        }
    }*/
}
