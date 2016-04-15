using System;
using System.Linq;
using Microsoft.Win32;

namespace TweetDck.Migration.Helpers{
    static class ProgramRegistrySearch{
        public static string FindByDisplayName(string displayName){
            Predicate<RegistryKey> predicate = key => displayName.Equals(key.GetValue("DisplayName") as string,StringComparison.OrdinalIgnoreCase);
            string guid;

            return FindMatchingSubKey(Registry.LocalMachine,@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall",predicate,out guid) ||
                   FindMatchingSubKey(Registry.LocalMachine,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",predicate,out guid) ||
                   FindMatchingSubKey(Registry.CurrentUser,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",predicate,out guid)
                   ? guid : null;
        }

        private static bool FindMatchingSubKey(RegistryKey keyHandle, string path, Predicate<RegistryKey> predicate, out string guid){
            string outputId = null;

            try{
                RegistryKey parentKey = keyHandle.OpenSubKey(path,false);
                if (parentKey == null)throw new InvalidOperationException();

                foreach(RegistryKey subKey in parentKey.GetSubKeyNames().Select(subName => parentKey.OpenSubKey(subName,false)).Where(subKey => subKey != null)){
                    if (predicate(subKey)){
                        outputId = subKey.Name.Substring(subKey.Name.LastIndexOf('\\')+1);
                        subKey.Close();
                        break;
                    }

                    subKey.Close();
                }

                parentKey.Close();
            }catch(Exception){
                guid = null;
                return false;
            }

            return (guid = outputId) != null;
        }
    }
}
