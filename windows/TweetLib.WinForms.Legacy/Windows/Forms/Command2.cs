using System.Diagnostics;
using System.Reflection;

namespace System.Windows.Forms {
	internal sealed class Command2 {
		private static readonly Type Type = typeof(Form).Assembly.GetType("System.Windows.Forms.Command");
		private static readonly ConstructorInfo Constructor = Type.GetConstructor(new Type[] { typeof(ICommandExecutor) });
		private static readonly MethodInfo DisposeMethod = Type.GetMethod("Dispose", BindingFlags.Instance | BindingFlags.Public);
		private static readonly PropertyInfo IDProperty = Type.GetProperty("ID");

		internal static void EnsureValid() {
			if (Constructor == null || DisposeMethod == null || IDProperty == null) {
				throw new InvalidOperationException();
			}
		}
		
		public int ID { get; }

		private readonly object cmd;

		public Command2(ICommandExecutor executor) {
			this.cmd = Constructor.Invoke(new object[] { executor });
			this.ID = (int) IDProperty.GetValue(cmd)!;
		}

		public void Dispose() {
			try {
				DisposeMethod.Invoke(cmd, null);
			} catch (Exception e) {
				Debug.WriteLine(e);
				Debugger.Break();
			}
		}
	}
}
