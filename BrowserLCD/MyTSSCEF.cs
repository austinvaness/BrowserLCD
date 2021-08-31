using VRageMath;
using Sandbox.Game.GameSystems.TextSurfaceScripts;

namespace avaness.BrowserLCD
{
    [MyTextSurfaceScript("TSS_CEF", "CEF Web Browser")]
    public class MyTSSCEF : MyTSSCommon
    {
        public override ScriptUpdate NeedsUpdate
        {
            get
            {
                return ScriptUpdate.Update10;
            }
        }

        public MyTSSCEF(Sandbox.ModAPI.Ingame.IMyTextSurface surface, VRage.Game.ModAPI.IMyCubeBlock block, Vector2 size) : base(surface, block, size)
        {

        }
    }
}
