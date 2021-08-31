using HarmonyLib;
using VRageMath;
using VRage.Input;
using Sandbox.Graphics.GUI;
using System.Text;
using CefSharp;

namespace avaness.BrowserLCD
{
    public class MagicTextbox : MyGuiControlTextbox
    {
        public Vector2 browserId = new Vector2();
        public MagicTextbox() : this(null, null, 512, null, 0.8f, MyGuiControlTextboxType.Normal, MyGuiControlTextboxStyleEnum.Default)
        {
        }
        public MagicTextbox(Vector2? position = null, string defaultText = null, int maxLength = 512, Vector4? textColor = null, float textScale = 0.8f, MyGuiControlTextboxType type = MyGuiControlTextboxType.Normal, MyGuiControlTextboxStyleEnum visualStyle = MyGuiControlTextboxStyleEnum.Default) : base(position, defaultText, maxLength, textColor, textScale, type, visualStyle)
        {
        }
        StringBuilder emptyString = new StringBuilder();
        public override MyGuiControlBase HandleInput()
        {
            if (base.HasFocus && SECEF.browsers.ContainsKey(browserId))
            {
                var _browser = SECEF.browsers[browserId];
                foreach (char c in MyInput.Static.TextInput)
                {
                    if (c == (char)MyKeys.Escape)
                    {

                    }
                    else
                    {
                        CefEventFlags m = 0;
                        if (MyInput.Static.IsAnyShiftKeyPressed())
                            m |= CefEventFlags.ShiftDown;
                        if (MyInput.Static.IsAnyCtrlKeyPressed())
                            m |= CefEventFlags.ControlDown;
                        _browser.SendKey(c, m);
                    }
                }
                var m_keyThrottler = (MyKeyThrottler)AccessTools.Field(typeof(MyGuiControlTextbox), "m_keyThrottler").GetValue(null);
                MyKeys[] usefulKeys =
                {
                    MyKeys.Left,
                    MyKeys.Right,
                    MyKeys.Up,
                    MyKeys.Down,
                    MyKeys.Home,
                    MyKeys.End,
                    MyKeys.PageDown,
                    MyKeys.PageUp,
                    MyKeys.Delete
                };
                foreach (var key in usefulKeys)
                {
                    if (m_keyThrottler.GetKeyStatus(key) == ThrottledKeyStatus.PRESSED_AND_READY)
                    {
                        CefEventFlags m = 0;
                        if (MyInput.Static.IsAnyShiftKeyPressed())
                            m |= CefEventFlags.ShiftDown;
                        if (MyInput.Static.IsAnyCtrlKeyPressed())
                            m |= CefEventFlags.ControlDown;
                        _browser.SendKey((char)key, m);
                    }
                }
                SetText(emptyString);
            }
            MyGuiControlBase myGuiControlBase = base.HandleInput();
            return myGuiControlBase;
        }

    }
}
