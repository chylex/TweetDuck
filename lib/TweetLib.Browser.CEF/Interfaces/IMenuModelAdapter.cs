namespace TweetLib.Browser.CEF.Interfaces {
	public interface IMenuModelAdapter<T> {
		int GetItemCount(T model);

		void AddCommand(T model, int command, string name);
		int GetCommandAt(T model, int index);

		void AddCheckCommand(T model, int command, string name);
		void SetChecked(T model, int command, bool isChecked);

		void AddSeparator(T model);
		bool IsSeparatorAt(T model, int index);

		void RemoveAt(T model, int index);
	}
}
