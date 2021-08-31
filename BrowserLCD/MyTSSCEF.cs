using VRageMath;
using Sandbox.Game.GameSystems.TextSurfaceScripts;

namespace avaness.BrowserLCD
{
    [MyTextSurfaceScript("TSS_CEF", "CEF Web Browser")]
    public class MyTSSCEF : MyTSSCommon
    {
        // Token: 0x170005DF RID: 1503
        // (get) Token: 0x06003F3C RID: 16188 RVA: 0x00028ED8 File Offset: 0x000270D8
        public override ScriptUpdate NeedsUpdate
        {
            get
            {
                return ScriptUpdate.Update10;
            }
        }

        // Token: 0x06003F3D RID: 16189 RVA: 0x001857D4 File Offset: 0x001839D4
        public MyTSSCEF(Sandbox.ModAPI.Ingame.IMyTextSurface surface, VRage.Game.ModAPI.IMyCubeBlock block, Vector2 size) : base(surface, block, size)
        {

        }

        // Token: 0x06003F3E RID: 16190 RVA: 0x00185898 File Offset: 0x00183A98
        public override void Run()
        {
            base.Run();
        }

        // Token: 0x06003F3F RID: 16191 RVA: 0x00185A1C File Offset: 0x00183C1C
        // Note: this type is marked as 'beforefieldinit'.
        static MyTSSCEF()
        {
        }
    }
}
