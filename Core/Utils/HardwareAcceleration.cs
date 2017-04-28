using System;
using System.IO;

namespace TweetDck.Core.Utils{
    static class HardwareAcceleration{
        private static readonly string LibEGL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libEGL.dll");
        private static readonly string LibGLES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libGLESv2.dll");

        private static readonly string DisabledLibEGL = LibEGL+".bak";
        private static readonly string DisabledLibGLES = LibGLES+".bak";

        public static bool IsEnabled => File.Exists(LibEGL) && File.Exists(LibGLES);
        public static bool CanEnable => File.Exists(DisabledLibEGL) && File.Exists(DisabledLibGLES);

        public static bool Enable(){
            if (IsEnabled)return false;

            try{
                File.Move(DisabledLibEGL, LibEGL);
                File.Move(DisabledLibGLES, LibGLES);
                return true;
            }catch{
                return false;
            }
        }

        public static bool Disable(){
            if (!IsEnabled)return false;

            try{
                if (File.Exists(DisabledLibEGL)){
                    File.Delete(DisabledLibEGL);
                }

                if (File.Exists(DisabledLibGLES)){
                    File.Delete(DisabledLibGLES);
                }
            }catch{
                // woops
            }

            try{
                File.Move(LibEGL, DisabledLibEGL);
                File.Move(LibGLES, DisabledLibGLES);
                return true;
            }catch{
                return false;
            }
        }
    }
}
