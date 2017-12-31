using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TweetDuck.Core.Utils{
    static class LocaleUtils{
        public static string LocaleFolder => Path.Combine(Program.ProgramPath, "locales");

        public static IEnumerable<Item> ChromiumLocales => Directory
            .EnumerateFiles(LocaleFolder, "*.pak", SearchOption.TopDirectoryOnly)
            .Select(file => new Item(Path.GetFileNameWithoutExtension(file)))
            .OrderBy(code => code);
        
        public sealed class Item : IComparable<Item>{
            public string Code { get; }
            public CultureInfo Info { get; }

            public Item(string code){
                this.Code = code;
                this.Info = CultureInfo.GetCultureInfo(code);
            }

            public override bool Equals(object obj){
                return obj is Item other && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode(){
                return Code.GetHashCode();
            }

            public override string ToString(){
                string capitalizedName = Info.TextInfo.ToTitleCase(Info.NativeName);
                return Info.DisplayName == Info.NativeName ? capitalizedName : $"{capitalizedName}, {Info.DisplayName}";
            }

            public int CompareTo(Item other){
                return string.Compare(Info.NativeName, other.Info.NativeName, false, CultureInfo.InvariantCulture);
            }
        }
    }
}
