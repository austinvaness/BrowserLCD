using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using HarmonyLib;
using VRage.Render11.Resources;
using VRageMath;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using System.Collections.Generic;
using VRage.Plugins;
using VRage.Audio;

namespace avaness.BrowserLCD
{
    public class SECEF : IPlugin
    {
        private TaskScheduler scheduler;
        private Thread thread;
        public static Logger log;
        public static Bitmap videoBitmap = null;
        public static Vector2 videoSize = new Vector2();
        public static CefSharp.OffScreen.ChromiumWebBrowser _browser;
        public static string textureName = null;
        public static Type mgtm;
        public static Type mgt;
        public static Type mgti;
        public static object userTexture = null;
        public static MethodInfo TexReset;
        public static Type MyCopyToRT;
        public static MethodInfo MyCopyToRT_Run;
        public static FieldInfo backBufferField;
        public Type MyManagers;
        public static Type tRender11;
        public static object MyBorrowedRwTextureManager;
        public static Type tMyTextureData;
        public static bool initialized = false;
        public static int m_audioSetting = 2;
        public static bool allowMultiplayer = false;
        public static bool clickLog = false;
        public static int AudioSetting
        {
            get
            {
                return m_audioSetting;
            }
            set
            {
                m_audioSetting = value;
                foreach (var browser in browsers)
                {
                    browser.Value.SetMute(m_audioSetting != 1);
                }
            }
        } // 0 = off, 1 = global, 2 = local, 3 = unsafe
        public static Dictionary<Vector2, Browser> browsers = new Dictionary<Vector2, Browser>();
        public void Init(object gameInstance)
        {
            Harmony harmony = new Harmony("avaness.BrowserLCD");
            Logger _log = new Logger();

            //harmony = HarmonyInstance.Create("Xo.SECEF");
            AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);

            log = _log;
            log.Log("Hello from plubgin");

            InitBrowser();
            /*Type other = typeof(MyTextureCacheItem);
            Type targetType = other.Assembly.GetType("VRage.Render11.Resources.MyFileTextureManager");
            MethodInfo CreateGeneratedTexture = AccessTools.Method(targetType, "CreateGeneratedTexture");*/
            harmony.PatchAll(Assembly.GetAssembly(typeof(SECEF)));

            Type other = typeof(MyTextureCache);
            mgtm = other.Assembly.GetType("VRage.Render11.Resources.MyGeneratedTextureManager");
            mgt = other.Assembly.GetType("VRage.Render11.Resources.MyGeneratedTexture");
            mgti = other.Assembly.GetType("VRage.Render11.Resources.Internal.MyGeneratedTexture");
            MethodInfo rut = AccessTools.Method(mgtm, "ResetUserTexture");

            var mgit = other.Assembly.GetType("VRage.Render11.Resources.Internal.MyGeneratedTexture");
            MethodInfo m = AccessTools.Method(SECEF.mgtm, "Reset", new Type[] { mgit, typeof(byte[]), typeof(int) });
            TexReset = m;
            harmony.Patch(rut, new HarmonyMethod(AccessTools.Method(typeof(Patch_ResetUserTexture), "Prefix")));

            var tMySourceVoice = typeof(IMyPlatformAudio).Assembly.GetType("VRage.Audio.MySourceVoice");
            //harmony.Patch(AccessTools.Method(tMySourceVoice, "OnStopPlayingBuffered"), new HarmonyMethod(AccessTools.Method(typeof(Patch_OnStopPlayingBuffered), "Prefix")));

            log.Log("Init");
            initialized = true;
        }
        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, "CefSharp", new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
                return null;
            log.Log("loading from " + assemblyPath);
            Assembly assembly = Assembly.UnsafeLoadFrom(assemblyPath);
            return assembly;
        }
        private bool scriptRegistered = false;
        public void Update()
        {
            if (!initialized)
            {
                return;
            }

            foreach (var b in browsers)
            {
                var _browser = b.Value;
                if (_browser.browser.IsBrowserInitialized)
                {
                    _browser.browser.GetBrowserHost().Invalidate(PaintElementType.View);
                }
            }
        }
        public MyTextSurfaceScriptFactory tssfInstance;
        public void Dispose()
        {
            log.Log("Dispose");
        }
        public void InitBrowser()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            thread = Thread.CurrentThread;

            if (!Cef.IsInitialized)
            {
                var isDefault = AppDomain.CurrentDomain.IsDefaultAppDomain();
                if (!isDefault)
                {
                    throw new Exception(@"Add <add key=""xunit.appDomain"" value=""denied""/> to your app.config to disable appdomains");
                }
                CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
                var settings = new CefSettings();
                settings.EnableAudio();
                settings.DisableGpuAcceleration();
                //The location where cache data will be stored on disk. If empty an in-memory cache will be used for some features and a temporary disk cache for others.
                //HTML5 databases such as localStorage will only persist across sessions if a cache path is specified. 
                settings.CachePath = null; // Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Tests\\Cache");

                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }
        }
        public static void AddBrowser(Vector2 id, Browser browser)
        {
            if (!browsers.ContainsKey(id))
            {
                browsers.Add(id, browser);
            }
        }
    }
}