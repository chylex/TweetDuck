using System.Linq;
using System.Windows.Forms;

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

        public static bool HasAnyDialogs => Application.OpenForms.OfType<IAppDialog>().Any();
        
        public static void CloseAllDialogs(){
            foreach(IAppDialog dialog in Application.OpenForms.OfType<IAppDialog>().Reverse()){
                ((Form)dialog).Close();
            }
        }

        public interface IAppDialog{}
    }
}
