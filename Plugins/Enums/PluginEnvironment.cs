using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TweetDuck.Plugins.Enums{
    [Flags]
    enum PluginEnvironment{
        None = 0,
        Browser = 1,
        Notification = 2
    }

    static class PluginEnvironmentExtensions{
        public static IEnumerable<PluginEnvironment> Values{
            get{
                yield return PluginEnvironment.Browser;
                yield return PluginEnvironment.Notification;
            }
        }

        public static bool IncludesDisabledPlugins(this PluginEnvironment environment){
            return environment == PluginEnvironment.Browser;
        }

        public static string GetScriptIdentifier(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.Browser: return "root:plugins:browser";
                case PluginEnvironment.Notification: return "root:plugins:notification";
                default: return null;
            }
        }

        public static string GetPluginScriptFile(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.Browser: return "browser.js";
                case PluginEnvironment.Notification: return "notification.js";
                default: return null;
            }
        }

        public static string GetPluginScriptVariables(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.Browser: return "$,$TD,$TDP,TD";
                case PluginEnvironment.Notification: return "$TD,$TDP";
                default: return string.Empty;
            }
        }

        public static IReadOnlyDictionary<PluginEnvironment, T> Map<T>(T forNone, T forBrowser, T forNotification){
            return new PluginEnvironmentDictionary<T>(forNone, forBrowser, forNotification);
        }

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        private sealed class PluginEnvironmentDictionary<T> : IReadOnlyDictionary<PluginEnvironment, T>{
            private const int TotalKeys = 3;

            public IEnumerable<PluginEnvironment> Keys => Enum.GetValues(typeof(PluginEnvironment)).Cast<PluginEnvironment>();
            public IEnumerable<T> Values => data;
            public int Count => TotalKeys;

            public T this[PluginEnvironment key] => data[(int)key];

            private readonly T[] data;

            public PluginEnvironmentDictionary(T forNone, T forBrowser, T forNotification){
                this.data = new T[TotalKeys];
                this.data[(int)PluginEnvironment.None] = forNone;
                this.data[(int)PluginEnvironment.Browser] = forBrowser;
                this.data[(int)PluginEnvironment.Notification] = forNotification;
            }

            public bool ContainsKey(PluginEnvironment key){
                return key >= 0 && (int)key < TotalKeys;
            }

            public bool TryGetValue(PluginEnvironment key, out T value){
                if (ContainsKey(key)){
                    value = this[key];
                    return true;
                }
                else{
                    value = default(T);
                    return false;
                }
            }

            public IEnumerator<KeyValuePair<PluginEnvironment, T>> GetEnumerator(){
                return Keys.Select(key => new KeyValuePair<PluginEnvironment, T>(key, this[key])).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
