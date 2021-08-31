using System;
using System.Threading;
using System.Threading.Tasks;
using VRageMath;
using VRage.Game.Entity;
using System.Runtime.InteropServices;
using System.Linq;
using Sandbox.Game.Entities.Cube;
using VRage.Audio;
using CefSharp;
using CefSharp.Enums;
using CefSharp.OffScreen;
using System.Drawing;
using VRageRender;

namespace avaness.BrowserLCD
{
    public class Browser : IAudioHandler
    {
        public int sampleRate;
        public int channels;
        public bool audioStreamStarted = false;
        public ChannelLayout channelLayout;
        //IMySourceVoice Sound;
        public float audioDistance = 30;
        public bool persist = false;

        public void OnAudioStreamPacket(IWebBrowser chromiumWebBrowser, IBrowser browser, int audioStreamId, IntPtr data, int noOfFrames, long pts)
        {
            if (SECEF.AudioSetting < 2 || textureName == null || MyAudio.Static == null || !MyAudio.Static.CanPlay || entity == null || ((MyFunctionalBlock)entity).m_soundEmitter == null || noOfFrames < 2)
                return;

            try
            {
                //float[] fData = new float[noOfFrames];
                IntPtr[] fPtrs = new IntPtr[channels];

                if (audioStreamStarted && entity != null)
                {
                    if (data == null)
                        return;

                    float[][] output = new float[channels][];
                    for (int ch = 0; ch < channels; ch++)
                        output[ch] = new float[noOfFrames];

                    Marshal.Copy(data, fPtrs, 0, channels);
                    if (fPtrs[0] == null || fPtrs[1] == null)
                        return;

                    for (int ch = 0; ch < channels; ch++)
                        Marshal.Copy(fPtrs[ch], output[ch], 0, output[ch].Length);

                    //Marshal.Copy(new IntPtr((void*)foo[0]), fData, 0, fData.Length);
                    //SECEF.log.Log("pointers: " + foo[0] + " " + foo[1]);// new IntPtr((void*)foo[0]));


                    //SECEF.log.Log("Got audio packet" + " id: " + audioStreamId + " nof: " + noOfFrames);
                    //SECEF.log.Log("F: " + output[0][0] + " " + output[1][0]);
                    /*for (int i = 0; i < output[0].Length; i++)
                    {
                        SECEF.log.Log("F: " + output[0][i] + " " + output[1][i]);
                    }*/

                    var left = output[0];

                    /*float[] right = null;
                    if (channels == 2)
                    {
                        right = output[1];
                    }*/
                    var j = 0;
                    /*for (int i = 0; i < left.Length; i ++)
                    {
                        SECEF.log.Log("F " + left[i] +" " + right[i]);
                        j++;
                    }*/
                    byte[] newOutput = new byte[left.Length * 2];
                    j = 0;
                    for (int i = 0; i < left.Length; i++)
                    {
                        /*if (channels == 2)
                        {
                            byte[] tmp = BitConverter.GetBytes((short)((left[i] / 1) * 32000));
                            byte[] tmp2 = BitConverter.GetBytes((short)((right[i] / 1) * 32000));
                            newOutput[j++] = (byte)((tmp[0] + tmp2[0]) / 2);
                            newOutput[j++] = (byte)((tmp[1] + tmp2[1]) / 2);
                        } else*/
                        {
                            byte[] tmp = BitConverter.GetBytes((short)((left[i] / 1) * 32000));
                            newOutput[j++] = tmp[0];
                            newOutput[j++] = tmp[1];
                        }
                    }

                    if (SECEF.AudioSetting == 3)
                    {
                        //((MyFunctionalBlock)entity).m_soundEmitter.PlaySound(newOutput, newOutput.Length, sampleRate, 1, audioDistance);
                        ((MyFunctionalBlock)entity).m_soundEmitter.PlaySound(newOutput, 1, audioDistance);
                    }
                    else
                    {
                        /*var mydelegate = new Action<object>(delegate (object param)
                        {
                        //((MyFunctionalBlock)entity).m_soundEmitter.PlaySound(newOutput, newOutput.Length, sampleRate, 1, audioDistance);
                            if(param == null)
                            {
                                return;
                            }
                            var _params = (object[])param;
                            if(MyAudio.Static == null || _params[0] == null || ((MyFunctionalBlock)_params[0]).m_soundEmitter.Entity == null || _params[1] == null || _params[2] == null || _params[3] == null || _params[4] == null || _params[5] == null)
                            {
                                return;
                            }
                            lock (((MyFunctionalBlock)_params[0]).m_soundEmitter)
                            {
                                ((MyFunctionalBlock)_params[0]).m_soundEmitter.PlaySound((byte[])_params[1], (int)_params[2], (int)_params[3], (float)_params[4], (float)_params[5]);
                            }
                        });
                        mydelegate.Invoke(new object[] { (MyFunctionalBlock)entity, newOutput, newOutput.Length, sampleRate, 1f, audioDistance });*/
                        ExecuteInMainContext(() => ((MyFunctionalBlock)entity).m_soundEmitter.PlaySound(newOutput, 1, audioDistance));
                        //                        ExecuteInMainContext(() => ((MyFunctionalBlock)entity).m_soundEmitter.PlaySound(newOutput, newOutput.Length, sampleRate, 1, audioDistance));
                    }
                    return;
                }
            }
            catch (IndexOutOfRangeException)
            {
                //SECEF.log.Log(e.ToString());
            }
        }

        void ExecuteInMainContext(Action action)
        {
            var synchronization = SynchronizationContext.Current;
            if (synchronization != null)
            {
                //synchronization.Send(_ => action(), null);//sync
                //OR
                synchronization.Post(_ => action(), null);//async
            }
            else
            {
                Task.Factory.StartNew(action);
            }

            //OR
            /*var scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task task = new Task(action);
            if (scheduler != null)
                task.Start(scheduler);
            else
                task.Start();*/
        }

        public void OnAudioStreamStarted(IWebBrowser chromiumWebBrowser, IBrowser browser, int audioStreamId, int channels, ChannelLayout channelLayout, int sampleRate, int framesPerBuffer)
        {
            this.sampleRate = sampleRate;
            this.channels = channels;
            this.channelLayout = channelLayout;

            audioStreamStarted = true;
            SECEF.log.Log("Audio stream started " + sampleRate + " ch: " + channels + " " + channelLayout + " " + framesPerBuffer);
        }

        public void OnAudioStreamStopped(IWebBrowser chromiumWebBrowser, IBrowser browser, int audioStreamId)
        {
            audioStreamStarted = false;
            SECEF.log.Log("Audio stream stopped");
            if ((entity as MyFunctionalBlock) != null && ((MyFunctionalBlock)entity).m_soundEmitter != null)
            {
                if (SECEF.AudioSetting == 3)
                    ((MyFunctionalBlock)entity).m_soundEmitter.StopSound(false);
                else
                    ExecuteInMainContext(() => ((MyFunctionalBlock)entity).m_soundEmitter.StopSound(false));
            }
        }

        public ChromiumWebBrowser browser;
        public byte[] videoData = new byte[1024 * 1024 * 4];
        public string textureName = null;
        public string initText = "";
        public MyEntity entity;
        public int area;
        public bool muted = true;

        public Browser(string _textureName, int width = 512, int height = 512, string _initText = "")
        {
            textureName = _textureName;
            initText = _initText;
            videoData = new byte[1024 * 1024 * 4];
            browser = new ChromiumWebBrowser();
            browser.BrowserInitialized += OnInitialized;
            browser.Size = new Size(width, height);
            browser.Paint += OnPaint;
            browser.LifeSpanHandler = new LifespanHandler();
            browser.AudioHandler = this;
        }

        public void OnInitialized(object sender, EventArgs e)
        {
            browser.GetBrowserHost().SetAudioMuted(SECEF.AudioSetting != 1);
            if (initText.Length > 0)
            {
                ProcessCommands(initText);
            }
        }

        public void OnPaint(object sender, OnPaintEventArgs e)
        {
            if (textureName != null)
            {
                Marshal.Copy(e.BufferHandle, videoData, 0, e.Width * e.Height * 4);
                if (videoData.Length < 1024 * 1024 * 4)
                    Array.Resize(ref videoData, 1024 * 1024 * 4);
                MyRenderProxy.ResetGeneratedTexture(textureName, videoData);

                e.Handled = true;
                //_browser.GetBrowserHost().Invalidate(PaintElementType.View);
            }
        }

        public void SendKey(char key, CefEventFlags modifiers = 0)
        {
            if (!browser.IsBrowserInitialized)
                return;

            browser.GetBrowser().GetHost().SendKeyEvent(new KeyEvent
            {
                WindowsKeyCode = key, // Space
                FocusOnEditableField = true,
                IsSystemKey = false,
                Type = KeyEventType.KeyDown,
                Modifiers = modifiers
            });
            browser.GetBrowser().GetHost().SendKeyEvent(new KeyEvent
            {
                WindowsKeyCode = key, // Space
                FocusOnEditableField = true,
                IsSystemKey = false,
                Type = KeyEventType.Char,
                Modifiers = modifiers
            });
            browser.GetBrowser().GetHost().SendKeyEvent(new KeyEvent
            {
                WindowsKeyCode = key, // Space
                FocusOnEditableField = true,
                IsSystemKey = false,
                Type = KeyEventType.KeyUp,
                Modifiers = modifiers
            });

        }

        public void LoadUrl(string url)
        {
            browser.Load(url);
        }

        public bool ProcessCommands(string value)
        {
            var entity = this.entity;
            var id = 0;
            if (entity == null || value == null || value.Length == 0)
                return true;

            var myId = new Vector2(entity.EntityId, id);
            foreach (var oline in value.Split('\n'))
            {
                var line = oline;
                if (oline.Trim().Length == 0)
                    continue;

                if (oline[0] >= '0' && oline[0] <= '9')
                {
                    id = int.Parse(oline[0].ToString());
                    line = oline.Substring(1);
                    myId = new Vector2(entity.EntityId, id);
                }
                if (area == id && SECEF.browsers.ContainsKey(myId))
                {
                    var _browser = SECEF.browsers[myId].browser;
                    if (!_browser.IsBrowserInitialized)
                        continue;

                    //SECEF.log.Log("Line: " + line);
                    var param = line.Split(' ');
                    if (param.Count() == 0)
                        continue;

                    if (param.Count() == 2 && param[0] == "load")
                    {
                        SECEF.log.Log("Loading: " + param[1]);
                        if (param[1] != _browser.Address)
                            _browser.Load(param[1]);
                    }
                    else if (param.Count() == 3 && param[0] == "click")
                    {
                        var x = int.Parse(param[1]);
                        var y = int.Parse(param[2]);
                        _browser.GetBrowser().GetHost().SendMouseClickEvent(new MouseEvent(x, y, CefEventFlags.None), MouseButtonType.Left, false, 1);
                        //Thread.Sleep(5);
                        _browser.GetBrowser().GetHost().SendMouseClickEvent(new MouseEvent(x, y, CefEventFlags.None), MouseButtonType.Left, true, 1);
                        _browser.GetBrowserHost().Invalidate(PaintElementType.View);
                    }
                    else if (param.Count() > 0 && param[0] == "type")
                    {
                        for (int i = 1; i < param.Count(); i++)
                        {
                            if (i != 1)
                            {
                                _browser.GetBrowser().GetHost().SendKeyEvent(new KeyEvent
                                {
                                    WindowsKeyCode = 0x20, // Space
                                    FocusOnEditableField = true,
                                    IsSystemKey = false,
                                    Type = KeyEventType.Char
                                });
                            }
                            foreach (char c in param[i])
                            {
                                _browser.GetBrowser().GetHost().SendKeyEvent(new KeyEvent
                                {
                                    WindowsKeyCode = c,
                                    FocusOnEditableField = true,
                                    IsSystemKey = false,
                                    Type = KeyEventType.Char
                                });
                            }
                        }
                    }
                    else if (param[0] == "mute")
                    {
                        _browser.GetBrowser().GetHost().SetAudioMuted(true);
                        muted = true;
                    }
                    else if (param[0] == "unmute")
                    {
                        _browser.GetBrowser().GetHost().SetAudioMuted(false);
                        muted = false;
                    }
                    else if (param[0] == "back")
                    {
                        _browser.GetBrowser().GoBack();
                    }
                    else if (param[0] == "forward")
                    {
                        _browser.GetBrowser().GoForward();
                    }
                    else if (param[0] == "reload")
                    {
                        _browser.GetBrowser().Reload();
                    }
                    else if (param[0] == "distance")
                    {
                        if (float.TryParse(param[1], out float num))
                        {
                            audioDistance = num;
                        }

                    }
                    else if (param[0] == "persist")
                    {
                        persist = true;
                    }
                    else if (param[0] == "nopersist")
                    {
                        persist = false;
                    }
                }
            }
            return true;
        }

        public void SetMute(bool mute)
        {
            if (browser.IsBrowserInitialized)
                browser.GetBrowserHost().SetAudioMuted(mute);
        }
    }
}
