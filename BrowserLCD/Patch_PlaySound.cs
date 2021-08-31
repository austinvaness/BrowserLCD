using HarmonyLib;
using Sandbox.Game.Entities;
using System;
using VRage.Data.Audio;

namespace avaness.BrowserLCD
{
    /*[HarmonyPatch(typeof(MyEntity3DSoundEmitter), "PlaySound", new Type[] { typeof(byte[]), typeof(int), typeof(int), typeof(float), typeof(float), typeof(MySoundDimensions) })]
    public static class Patch_PlaySound
    {
        public static bool Prefix(byte[] buffer, int size, int sampleRate, float volume = 1f, float maxDistance = 0f, MySoundDimensions dimension = MySoundDimensions.D3)
        {
            //SECEF.log.Log("play sound patch " + (buffer == null) + " " + size + " " + sampleRate + " " + (MyAudio.Static == null));
            if(buffer == null || size == 0 || MyAudio.Static == null)
            {
                return false;
            }
            return true;
        }

    }*/
}
