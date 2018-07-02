using System.Text;
using System.Text.RegularExpressions;

namespace TweetDuck.Core.Handling.Filters{
    sealed class ResponseFilterVendor : ResponseFilterBase{
        private static readonly Regex RegexRestoreJQuery = new Regex(@"(\w+)\.fn=\1\.prototype", RegexOptions.Compiled);

        public ResponseFilterVendor(int totalBytes) : base(totalBytes, Encoding.UTF8){}

        public override bool InitFilter(){
            return true;
        }
        
        protected override string ProcessResponse(string text){
            return RegexRestoreJQuery.Replace(text, "window.$$=$1;$&", 1);
        }

        public override void Dispose(){}
    }
}
