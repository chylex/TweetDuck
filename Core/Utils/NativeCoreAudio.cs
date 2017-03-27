using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace TweetDck.Core.Utils{
    internal static class NativeCoreAudio{
        private const int EDATAFLOW_RENDER = 0;
        private const int EROLE_MULTIMEDIA = 1;

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumerator{}

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator{
            int Unimpl_EnumAudioEndpoints();
            IMMDevice GetDefaultAudioEndpoint(int dataFlow, int role);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice{
            [return:MarshalAs(UnmanagedType.IUnknown)]
            object Activate(ref Guid id, int clsCtx, IntPtr activationParams);
        }

        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionManager2{
            int Unimpl_FindWillToLive();
            int Unimpl_HelloDarknessMyOldFriend();
            IAudioSessionEnumerator GetSessionEnumerator();
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionEnumerator{
            int GetCount();
            IAudioSessionControl GetSession(int sessionIndex);
        }

        [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionControl{}

        [Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionControl2{
            int Unimpl_GetState();
            int Unimpl_GetDisplayName();
            int Unimpl_SetDisplayName();
            int Unimpl_GetIconPath();
            int Unimpl_SetIconPath();
            int Unimpl_GetGroupingParam();
            int Unimpl_SetGroupingParam();
            int Unimpl_RegisterAudioSessionNotification();
            int Unimpl_UnregisterAudioSessionNotification();

            [return:MarshalAs(UnmanagedType.LPWStr)]
            string GetSessionIdentifier();
        }

        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISimpleAudioVolume{
            void SetMasterVolume(float level, ref Guid eventContext);
            float GetMasterVolume();
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        private static ISimpleAudioVolume GetVolumeObject(string name){
            IMMDeviceEnumerator devices = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            IMMDevice device = devices.GetDefaultAudioEndpoint(EDATAFLOW_RENDER, EROLE_MULTIMEDIA);

            Guid sessionManagerGUID = typeof(IAudioSessionManager2).GUID;
            IAudioSessionManager2 manager = (IAudioSessionManager2)device.Activate(ref sessionManagerGUID, 0, IntPtr.Zero);
            IAudioSessionEnumerator sessions = manager.GetSessionEnumerator();

            ISimpleAudioVolume volumeObj = null;

            for(int index = sessions.GetCount()-1; index >= 0; index--){
                IAudioSessionControl2 ctl = sessions.GetSession(index) as IAudioSessionControl2;

                if (ctl != null){
                    string identifier = ctl.GetSessionIdentifier();

                    if (identifier != null && identifier.Contains(name)){
                        volumeObj = ctl as ISimpleAudioVolume;
                        break;
                    }

                    Marshal.ReleaseComObject(ctl);
                }
            }

            Marshal.ReleaseComObject(devices);
            Marshal.ReleaseComObject(device);
            Marshal.ReleaseComObject(manager);
            Marshal.ReleaseComObject(sessions);
            return volumeObj;
        }

        public static double GetMixerVolume(string appPath){
            ISimpleAudioVolume obj = GetVolumeObject(appPath);
            float level = 1F;

            if (obj != null){
                level = obj.GetMasterVolume();
                Marshal.ReleaseComObject(obj);
            }

            return Math.Round(level, 2);
        }

        public static double GetMixerVolumeForCurrentProcess(){
            string path;

            using(Process process = Process.GetCurrentProcess()){
                path = process.MainModule.FileName;
                path = path.Substring(Path.GetPathRoot(path).Length);
            }

            return GetMixerVolume(path);
        }
    }
}
