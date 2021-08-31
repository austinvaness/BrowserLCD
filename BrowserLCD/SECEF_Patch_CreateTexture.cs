using System;
using HarmonyLib;
using Sandbox.Game.Components;
using VRageMath;
using VRage.Game.Entity;
using System.Runtime.InteropServices;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using VRage.Game;
using System.Drawing;
using VRageRender;
using CefSharp;
using VRageRender.Messages;

namespace avaness.BrowserLCD
{
    /*[HarmonyPatch(typeof(MyRenderComponentScreenAreas))]
    [HarmonyPatch("CreateTexture")]*/
    public static class SECEF_Patch_CreateTexture
    {
        //[HarmonyPrefix]
        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(MyRenderComponentScreenAreas __instance, int area, Vector2I textureSize)
        {
            //SECEF.log.Log("CreateTexture. bm: " + SECEF.videoBitmap + " Size: " + SECEF.videoBitmap.Width + "data: ");
            if (MySession.Static.OnlineMode != MyOnlineModeEnum.OFFLINE && !SECEF.allowMultiplayer)
                return true;

            MyEntity m_entity = (MyEntity)AccessTools.Field(typeof(MyRenderComponentScreenAreas), "m_entity").GetValue(__instance);
            //m_entity = m_entity.GetTopMostParent(typeof(MyEntity));
            SECEF.log.Log("got Entity " + m_entity + "  " + m_entity.DisplayNameText);
            var id = m_entity.EntityId;

            var textureName = __instance.GenerateOffscreenTextureName(m_entity.EntityId, area);

            SECEF.log.Log(DateTime.Now + " Set texturename " + textureName + " x/y: " + textureSize + "id: " + new Vector2(id, area));
            if (SECEF.browsers.ContainsKey(new Vector2(id, area)))
            {
                SECEF.log.Log("Found existing browser " + new Vector2(id, area));
                var b = SECEF.browsers[new Vector2(id, area)];
                //MyRenderProxy.UnloadTexture(textureName);
                b.textureName = textureName;
                b.entity = m_entity;
                //b.area = area;
                MyRenderProxy.CreateGeneratedTexture(b.textureName, (int)textureSize.X, (int)textureSize.Y, MyGeneratedTextureType.RGBA_Linear, 1, b.videoData, true);
                b.browser.GetBrowserHost().Invalidate(PaintElementType.View);
                return false;
            }
            var browser = new Browser(textureName, textureSize.X, textureSize.Y, m_entity.Storage != null ? m_entity.Storage.GetValue(Patch_EnsureGeneratedTexture.guid) : "");
            MyRenderProxy.CreateGeneratedTexture(browser.textureName, (int)textureSize.X, (int)textureSize.Y, MyGeneratedTextureType.RGBA_Linear, 1, browser.videoData, true);
            browser.entity = m_entity;
            browser.area = area;
            SECEF.AddBrowser(new Vector2(id, area), browser);
            var myTerminal = (MyTerminalBlock)m_entity;
            myTerminal.CustomDataChanged -= UpdateCustomData;
            myTerminal.CustomDataChanged += UpdateCustomData;
            if (myTerminal.CustomData.Length > 0)
                UpdateCustomData(myTerminal);
            //MyRenderProxy.CreateGeneratedTexture(__instance.GenerateOffscreenTextureName(m_entity.EntityId, area), (int)SECEF.videoSize.X, (int)SECEF.videoSize.Y, MyGeneratedTextureType.RGBA, 1, data, true);
            return false;
        }

        public static void GetBGRValues(Bitmap bmp, ref byte[] rgbValues)
        {
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            var rowBytes = bmpData.Width * Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            var imgBytes = bmp.Height * rowBytes;
            //byte[] rgbValues = new byte[imgBytes];

            var ptr = bmpData.Scan0;
            Marshal.Copy(ptr, rgbValues, 0, imgBytes);
            bmp.UnlockBits(bmpData);
            //return rgbValues;
        }

        public static void UpdateCustomData(MyTerminalBlock __instance)
        {
            var value = __instance.CustomData;
            var entity = __instance;
            var id = 0;
            if (value == null || value.Length == 0)
                return;

            var myId = new Vector2(entity.EntityId, id);
            foreach (var br in SECEF.browsers)
            {
                if (br.Value.entity.EntityId == entity.EntityId)
                    br.Value.ProcessCommands(value);
            }
        }
    }
}
