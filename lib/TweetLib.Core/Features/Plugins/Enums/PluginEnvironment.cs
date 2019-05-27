using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TweetLib.Core.Features.Plugins.Enums{
    [Flags]
    public enum PluginEnvironment{
        None = 0,
        Browser = 1,
        Notification = 2
    }

    public static class PluginEnvironmentExtensions{
        public static IEnumerable<PluginEnvironment> Values{
            get{
                yield return PluginEnvironment.Browser;
                yield return PluginEnvironment.Notification;
            }
        }

        public static bool IncludesDisabledPlugins(this PluginEnvironment environment){
            return environment == PluginEnvironment.Browser;
        }

        public static string? GetPluginScriptFile(this PluginEnvironment environment){
            return environment switch{
                PluginEnvironment.Browser      => "browser.js",
                PluginEnvironment.Notification => "notification.js",
                _                              => null
            };
        }

        public static string GetPluginScriptVariables(this PluginEnvironment environment){
            return environment switch{
                PluginEnvironment.Browser      => "$,$TD,$TDP,TD",
                PluginEnvironment.Notification => "$TD,$TDP",
                _                              => string.Empty
            };
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
                    value = default;
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
