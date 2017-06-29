using System.ComponentModel;
using System.Windows.Forms;

namespace TweetDuck.Core.Controls{
    sealed class NumericUpDownEx : NumericUpDown{
        public string TextSuffix { get; set ; }

        protected override void UpdateEditText(){
            base.UpdateEditText();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime){
                ChangingText = true;
                Text += TextSuffix;
                ChangingText = false;
            }
        }
    }
}
