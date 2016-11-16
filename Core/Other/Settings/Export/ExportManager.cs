using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TweetDck.Plugins;
using TweetDck.Plugins.Enums;

namespace TweetDck.Core.Other.Settings.Export{
    sealed class ExportManager{
        private static readonly string CookiesPath = Path.Combine(Program.StoragePath, "Cookies");
        private static readonly string TempCookiesPath = Path.Combine(Program.StoragePath, "CookiesTmp");

        public bool IsRestarting { get; private set; }
        public Exception LastException { get; private set; }

        private readonly string file;
        private readonly PluginManager plugins;

        public ExportManager(string file, PluginManager plugins){
            this.file = file;
            this.plugins = plugins;
        }

        public bool Export(bool includeSession){
            try{
                using(CombinedFileStream stream = new CombinedFileStream(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))){
                    stream.WriteFile("config", Program.ConfigFilePath);

                    foreach(Plugin plugin in plugins.Plugins){
                        foreach(PathInfo path in EnumerateFilesRelative(plugin.GetPluginFolder(PluginFolder.Data))){
                            try{
                                stream.WriteFile(new string[]{ "plugin.data", plugin.Identifier, path.Relative }, path.Full);
                            }catch(ArgumentOutOfRangeException e){
                                MessageBox.Show("Could not include a plugin file in the export. "+e.Message, "Export Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }

                    if (includeSession){
                        stream.WriteFile("cookies", CookiesPath);
                    }

                    stream.Flush();
                }

                return true;
            }catch(Exception e){
                LastException = e;
                return false;
            }
        }

        public bool Import(){
            try{
                HashSet<string> missingPlugins = new HashSet<string>();

                using(CombinedFileStream stream = new CombinedFileStream(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))){
                    CombinedFileStream.Entry entry;

                    while((entry = stream.ReadFile()) != null){
                        switch(entry.KeyName){
                            case "config":
                                entry.WriteToFile(Program.ConfigFilePath);
                                Program.ReloadConfig();
                                break;

                            case "plugin.data":
                                string[] value = entry.KeyValue;

                                entry.WriteToFile(Path.Combine(Program.PluginDataPath, value[0], value[1]), true);

                                if (!plugins.IsPluginInstalled(value[0])){
                                    missingPlugins.Add(value[0]);
                                }

                                break;

                            case "cookies":
                                if (MessageBox.Show("Do you want to import the login session? This will restart "+Program.BrandName+".", "Importing "+Program.BrandName+" Profile", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                                    entry.WriteToFile(Path.Combine(Program.StoragePath, TempCookiesPath));
                                    IsRestarting = true;
                                }

                                break;
                        }
                    }
                }

                if (missingPlugins.Count > 0){
                    MessageBox.Show("Detected missing plugins when importing plugin data:"+Environment.NewLine+string.Join(Environment.NewLine, missingPlugins), "Importing "+Program.BrandName+" Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (IsRestarting){
                    Program.Restart(new string[]{ "-importcookies" });
                }
                else{
                    plugins.Reload();
                }

                return true;
            }catch(Exception e){
                LastException = e;
                return false;
            }
        }

        public static void ImportCookies(){
            if (File.Exists(TempCookiesPath)){
                try{
                    if (File.Exists(CookiesPath)){
                        File.Delete(CookiesPath);
                    }

                    File.Move(TempCookiesPath, CookiesPath);
                }catch(Exception e){
                    Program.Reporter.HandleException("Profile Import Error", "Could not import the cookie file to restore login session.", true, e);
                }
            }
        }

        private static IEnumerable<PathInfo> EnumerateFilesRelative(string root){
            return Directory.Exists(root) ? Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories).Select(fullPath => new PathInfo{
                Full = fullPath,
                Relative = fullPath.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) // strip leading separator character
            }) : Enumerable.Empty<PathInfo>();
        }

        private class PathInfo{
            public string Full { get; set; }
            public string Relative { get; set; }
        }
    }
}
