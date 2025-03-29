namespace UIWidgets
{
	/// <summary>
	/// Hideable interface.
	/// </summary>
	public interface IHideable
	{
		/// <summary>
		/// Is open as asynchronous task?
		/// </summary>
		bool IsAsync
		{
			get;
		}

		/// <summary>
		/// Hide.
		/// </summary>
		void Hide();

		/// <summary>
		/// Complete asynchronous task.
		/// </summary>
		/// <param name="buttonIndex">Button index.</param>
		void Complete(int buttonIndex);
	}
}