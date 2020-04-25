using System.Reflection;
using Version = TweetDuck.Version;

[assembly: AssemblyVersion(Version.Tag)]
[assembly: AssemblyFileVersion(Version.Tag)]

namespace TweetDuck{
    internal static class Version{
        public const string Tag = "1.18.3";
    }
}
