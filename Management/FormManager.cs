using System.Linq;
using System.Windows.Forms;

namespace TweetDuck.Management {
	static class FormManager {
		private static FormCollection OpenForms => System.Windows.Forms.Application.OpenForms;

		public static T TryFind<T>() where T : Form {
			return OpenForms.OfType<T>().FirstOrDefault();
		}

		public static bool TryBringToFront<T>() where T : Form {
			T form = TryFind<T>();

			if (form != null) {
				form.BringToFront();
				return true;
			}
			else {
				return false;
			}
		}

		public static void CloseAllDialogs() {
			foreach (IAppDialog dialog in OpenForms.OfType<IAppDialog>().Reverse()) {
				((Form) dialog).Close();
			}
		}

		public interface IAppDialog {}
	}
}
