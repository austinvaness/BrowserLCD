using System;
using HarmonyLib;
using VRageMath;
using Sandbox.Game.Entities.Blocks;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;
using Sandbox.Game.Entities.Character;
using VRage.Game.Entity.UseObject;
using VRage.Game.ModAPI;
using VRage.Input;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using CefSharp;

namespace avaness.BrowserLCD
{
    [HarmonyPatch(typeof(MyTextPanel), "OnOwnerUse", new Type[] { typeof(UseActionEnum), typeof(MyCharacter) })]
    public static class Patch_OnOwnerUse
    {
        public static bool keyboardScreenOpen = false;
        public static bool Prefix(object __instance, UseActionEnum actionEnum, MyCharacter user)
        {
            if (actionEnum == UseActionEnum.Manipulate)
            {
                var entity = (IMyEntity)__instance;
                var myId = new Vector2(entity.EntityId, 0);
                if (SECEF.browsers.ContainsKey(myId))
                {
                    if (!keyboardScreenOpen && MyInput.Static.IsAnyCtrlKeyPressed())
                    {
                        keyboardScreenOpen = true;
                        if (MyInput.Static.IsAnyShiftKeyPressed())
                        {
                            MyGuiScreenURL myGuiScreenURL = new MyGuiScreenURL(myId);
                            myGuiScreenURL.Closed += OnWindowClosed;
                            MyGuiSandbox.AddScreen(myGuiScreenURL);
                        }
                        else
                        {
                            MyGuiScreenKeyboard myGuiScreenRenameBlocks = new MyGuiScreenKeyboard(myId);
                            myGuiScreenRenameBlocks.Closed += OnWindowClosed;
                            MyGuiSandbox.AddScreen(myGuiScreenRenameBlocks);
                        }
                        return false;
                    }

                    var b = SECEF.browsers[myId];
                    var _browser = SECEF.browsers[myId].browser;

                    var aimedHeadPos = GetAimedPointFromHead(user);
                    if (aimedHeadPos == Vector3D.Zero)
                    {
                        if (SECEF.clickLog)
                        {
                            MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Click missed.");
                        }
                        return false;
                    }
                    var e2 = b.entity;

                    aimedHeadPos = e2.PositionComp.GetPosition() - aimedHeadPos;

                    var pos1 = new Vector2D(aimedHeadPos.Dot(e2.WorldMatrix.GetOrientation().Left), aimedHeadPos.Dot(e2.WorldMatrix.GetOrientation().Up));

                    var pos2 = (pos1 / new Vector2D(1.25f * (_browser.Size.Width / _browser.Size.Height), 1.25f)) * new Vector2D(_browser.Size.Width / 2, _browser.Size.Height / 2);
                    pos2 += new Vector2D(_browser.Size.Width / 2, _browser.Size.Height / 2);
                    pos2 = new Vector2D(Math.Round(pos2.X), Math.Round(pos2.Y));
                    //SECEF.log.Log("aimed heid: " + aimedHeadPos + "p1 " + pos1 + " p2 " + pos2 + " bs: " + _browser.Size);
                    if (SECEF.clickLog)
                    {
                        MyAPIGateway.Utilities.ShowMessage("[SECEF]", "Click hit: " + pos2);
                    }
                    if (_browser.IsBrowserInitialized)
                    {
                        var x = (int)pos2.X;
                        var y = (int)pos2.Y;
                        _browser.GetBrowser().GetHost().SendMouseClickEvent(new MouseEvent(x, y, CefEventFlags.None), MouseButtonType.Left, false, 1);
                        //Thread.Sleep(5);
                        _browser.GetBrowser().GetHost().SendMouseClickEvent(new MouseEvent(x, y, CefEventFlags.None), MouseButtonType.Left, true, 1);
                        _browser.GetBrowserHost().Invalidate(PaintElementType.View);
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
        /*public static Vector2D GetClickLocation(MyCharacter me, int width, int height)
        {
            var aimedHeadPos = GetAimedPointFromHead(me);
            if (aimedHeadPos == Vector3D.Zero)
            {
                return Vector2D.Zero;
            }
            var e2 = b.entity;

            aimedHeadPos = e2.PositionComp.GetPosition() - aimedHeadPos;

            var pos1 = new Vector2D(aimedHeadPos.Dot(e2.WorldMatrix.GetOrientation().Left), aimedHeadPos.Dot(e2.WorldMatrix.GetOrientation().Up));

            var pos2 = (pos1 / new Vector2D(1.25f * (width / height), 1.25f)) * new Vector2D(width / 2, height / 2);
            pos2 += new Vector2D(width / 2, height / 2);
            pos2 = new Vector2D(Math.Round(pos2.X), Math.Round(pos2.Y));
            return pos2;
        }*/
        private static Vector3D GetAimedPointFromHead(MyCharacter c)
        {
            MatrixD viewMatrix = c.GetViewMatrix();
            MatrixD matrixD;
            MatrixD.Invert(ref viewMatrix, out matrixD);
            Vector3D forward = matrixD.Forward;
            forward.Normalize();
            Vector3D vector3D = matrixD.Translation;

            Vector3D translation = c.GetHeadMatrix(false, true, false, false, false).Translation;
            vector3D += forward * (translation - vector3D).Dot(forward);
            Vector3D result = Vector3D.Zero; //(vector3D + forward * 25000.0);
            List<IHitInfo> m_raycastList = new List<IHitInfo>();
            m_raycastList.Clear();
            MyAPIGateway.Physics.CastRay(vector3D, vector3D + forward * 100.0, m_raycastList, 0);
            foreach (IHitInfo hitInfo in m_raycastList)
            {
                //SECEF.log.Log("hit " + hitInfo + "hk " + hitInfo.HkHitInfo);
                VRage.ModAPI.IMyEntity hitEntity = hitInfo.HitEntity;
                if (hitEntity != c)
                {
                    result = hitInfo.Position;
                    break;
                }
            }
            return result;
        }
        public static void OnWindowClosed(MyGuiScreenBase source, bool isUnloading)
        {

            keyboardScreenOpen = false;
        }
    }
}
