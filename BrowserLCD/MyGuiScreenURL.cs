using System;
using VRage;
using VRageMath;
using VRage.Utils;
using VRage.Audio;
using Sandbox.Graphics.GUI;
using VRage.Game;
using Sandbox;
using System.Text;

namespace avaness.BrowserLCD
{
    public class MyGuiScreenURL : MyGuiScreenBase
    {
        public MyGuiControlTextbox m_nameTextbox;
        public MyGuiControlButton m_okButton;
        public MyGuiControlButton m_cancelButton;
        Vector2 browserId;
        public override string GetFriendlyName()
        {
            return "MyGuiScreenURL";
        }
        public MyGuiScreenURL(Vector2 browserId) : base(new Vector2?(new Vector2(0.5f, 0.9f)), new Vector4?(MyGuiConstants.SCREEN_BACKGROUND_COLOR), new Vector2?(new Vector2(0.497142851f, 0.280534357f)), false, null, MySandboxGame.Config.UIBkOpacity, MySandboxGame.Config.UIOpacity, null)
        {
            this.browserId = browserId;
            base.EnabledBackgroundFade = false;
            base.AddCaption("Enter a URL", null, new Vector2?(new Vector2(0f, 0.003f)), 0.8f);
            MyGuiControlSeparatorList myGuiControlSeparatorList = new MyGuiControlSeparatorList();
            myGuiControlSeparatorList.AddHorizontal(new Vector2(0f, 0f) - new Vector2(this.m_size.Value.X * 0.78f / 2f, this.m_size.Value.Y / 2f - 0.075f), this.m_size.Value.X * 0.78f, 0f, null);
            this.Controls.Add(myGuiControlSeparatorList);
            MyGuiControlSeparatorList myGuiControlSeparatorList2 = new MyGuiControlSeparatorList();
            myGuiControlSeparatorList2.AddHorizontal(new Vector2(0f, 0f) - new Vector2(this.m_size.Value.X * 0.78f / 2f, -this.m_size.Value.Y / 2f + 0.122f), this.m_size.Value.X * 0.78f, 0f, null);
            this.Controls.Add(myGuiControlSeparatorList2);
            float y = -0.027f;
            this.m_nameTextbox = new MyGuiControlTextbox(new Vector2?(new Vector2(0f, y)), "", 75, null, 0.8f, MyGuiControlTextboxType.Normal, MyGuiControlTextboxStyleEnum.Default);
            this.m_nameTextbox.Size = new Vector2(0.385f, 1f);
            this.m_okButton = new MyGuiControlButton(null, MyGuiControlButtonStyleEnum.Default, null, null, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM, null, MyTexts.Get(MyCommonTexts.Ok), 0.8f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiControlHighlightType.WHEN_ACTIVE, new Action<MyGuiControlButton>(this.OnOkButtonClick), GuiSounds.MouseClick, 1f, null, false);
            this.m_cancelButton = new MyGuiControlButton(null, MyGuiControlButtonStyleEnum.Default, null, null, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM, null, MyTexts.Get(MyCommonTexts.Close), 0.8f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiControlHighlightType.WHEN_ACTIVE, new Action<MyGuiControlButton>(this.OnCancelButtonClick), GuiSounds.MouseClick, 1f, null, false);
            Vector2 value = new Vector2(0, this.m_size.Value.Y / 2f - 0.045f);
            Vector2 value2 = new Vector2(0.025f, 0f);
            this.m_okButton.Position = value - value2;
            this.m_cancelButton.Position = value + value2;
            //this.m_cancelButton.Position = new Vector2(-(m_cancelButton.Size.X / 2), this.m_size.Value.Y / 2f - 0.045f); //value;// + value2;
            this.Controls.Add(this.m_nameTextbox);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.m_cancelButton);
            this.m_nameTextbox.MoveCarriageToEnd();
            base.CloseButtonEnabled = true;
            this.OnEnterCallback = new Action(this.OnEnterPressed);

            //this.m_nameTextbox.browserId = browserId;
            m_nameTextbox.UserData = browserId;
            //AccessTools.Field(typeof(MyGuiControlTextbox), "m_carriagePositionIndex").SetValue(m_nameTextbox, 1);
        }
        StringBuilder tmp = new StringBuilder();
        private void OnEnterPressed()
        {
            m_nameTextbox.GetText(tmp);
            if (tmp.Length > 0)
            {
                if (SECEF.browsers.ContainsKey(browserId))
                {
                    var _browser = SECEF.browsers[browserId];
                    _browser.LoadUrl(tmp.ToString());
                }
            }
            CloseScreen();
        }
        private void OnCancelButtonClick(MyGuiControlButton sender)
        {
            CloseScreen();

        }

        private void OnOkButtonClick(MyGuiControlButton sender)
        {
            OnEnterPressed();
        }
    }
}
