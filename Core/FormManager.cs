using System.Linq;
using System.Windows.Forms;
using TweetDuck.Core.Other;

namespace TweetDuck.Core{
    static class FormManager{
        public static T TryFind<T>() where T : Form{
            return Application.OpenForms.OfType<T>().FirstOrDefault();
        }

        public static bool TryBringToFront<T>() where T : Form{
            T form = TryFind<T>();

            if (form != null){
                form.BringToFront();
                return true;
            }
            else return false;
        }

        public static void CloseAllDialogs(){
            foreach(Form form in Application.OpenForms.Cast<Form>().Reverse()){
                if (form is FormSettings || form is FormPlugins || form is FormAbout || form is FormGuide){
                    form.Close();
                }
            }
        }
    }
}
