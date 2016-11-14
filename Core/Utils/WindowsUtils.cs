using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TweetDck.Core.Utils{
    static class WindowsUtils{
        public static bool CheckFolderPermission(string path, FileSystemRights right){
            try{
                AuthorizationRuleCollection rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                if (identity.Groups == null){
                    return false;
                }

                bool accessAllow = false, accessDeny = false;

                foreach(FileSystemAccessRule rule in rules.Cast<FileSystemAccessRule>().Where(rule => identity.Groups.Contains(rule.IdentityReference) && (right & rule.FileSystemRights) == right)){
                    switch(rule.AccessControlType){
                        case AccessControlType.Allow: accessAllow = true; break;
                        case AccessControlType.Deny: accessDeny = true; break;
                    }
                }

                return accessAllow && !accessDeny;
            }
            catch{
                return false;
            }
        }
    }
}
