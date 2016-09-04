using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TweetDck.Plugins;

namespace TweetDck.Core.Other.Settings.Export{
    sealed class ExportManager{
        public static readonly string CookiesPath = Path.Combine(Program.StoragePath, "Cookies");
        public static readonly string TempCookiesPath = Path.Combine(Program.StoragePath, "CookiesTmp");

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

                    foreach(PathInfo path in EnumerateFilesRelative(plugins.PathOfficialPlugins)){
                        string[] split = path.Relative.Split(CombinedFileStream.KeySeparator);

                        if (split.Length < 3){
                            continue;
                        }
                        else if (split.Length == 3){
                            if (split[2].Equals(".meta", StringComparison.OrdinalIgnoreCase) ||
                                split[2].Equals("browser.js", StringComparison.OrdinalIgnoreCase) ||
                                split[2].Equals("notification.js", StringComparison.OrdinalIgnoreCase)){
                                continue;
                            }
                        }

                        try{
                            stream.WriteFile("plugin.off"+path.Relative, path.Full);
                        }catch(ArgumentOutOfRangeException e){
                            MessageBox.Show("Could not include a file in the export. "+e.Message, "Export Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    foreach(PathInfo path in EnumerateFilesRelative(plugins.PathCustomPlugins)){
                        try{
                            stream.WriteFile("plugin.usr"+path.Relative, path.Full);
                        }catch(ArgumentOutOfRangeException e){
                            MessageBox.Show("Could not include a file in the export. "+e.Message, "Export Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                bool updatedPlugins = false;

                using(CombinedFileStream stream = new CombinedFileStream(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))){
                    CombinedFileStream.Entry entry;

                    while((entry = stream.ReadFile()) != null){
                        switch(entry.KeyName){
                            case "config":
                                entry.WriteToFile(Program.ConfigFilePath);
                                Program.ReloadConfig();
                                break;

                            case "plugin.off":
                                string root = Path.Combine(plugins.PathOfficialPlugins, entry.Identifier.Split(CombinedFileStream.KeySeparator)[1]);

                                if (Directory.Exists(root)){
                                    entry.WriteToFile(Path.Combine(plugins.PathOfficialPlugins, entry.Identifier.Substring(entry.KeyName.Length+1)), true);
                                    updatedPlugins = true;
                                }

                                break;

                            case "plugin.usr":
                                entry.WriteToFile(Path.Combine(plugins.PathCustomPlugins, entry.Identifier.Substring(entry.KeyName.Length+1)), true);
                                updatedPlugins = true;
                                break;

                            case "cookies":
                                if (MessageBox.Show("Do you want to import the login session? This will restart "+Program.BrandName+".", "Importing "+Program.BrandName+" Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                                    entry.WriteToFile(Path.Combine(Program.StoragePath, TempCookiesPath));

                                    // okay to and restart, 'cookies' is always the last entry
                                    Process.Start(Application.ExecutablePath, "-restart -importcookies");
                                    Application.Exit();
                                }

                                break;
                        }
                    }
                }

                if (updatedPlugins){
                    plugins.Reload();
                }

                return true;
            }catch(Exception e){
                LastException = e;
                return false;
            }
        }

        private static IEnumerable<PathInfo> EnumerateFilesRelative(string root){
            return Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories).Select(fullPath => new PathInfo{
                Full = fullPath,
                Relative = fullPath.Substring(root.Length).Replace(Path.DirectorySeparatorChar, CombinedFileStream.KeySeparator) // includes leading separator character
            });
        }

        private class PathInfo{
            public string Full { get; set; }
            public string Relative { get; set; }
        }
    }
}
