namespace UIWidgets
{
	/// <summary>
	/// Event subscription interface.
	/// </summary>
	public interface ISubscription
	{
		/// <summary>
		/// Is listener enabled?
		/// </summary>
		bool IsOn
		{
			get;
		}

		/// <summary>
		/// Enable event listener.
		/// </summary>
		void On();

		/// <summary>
		/// Disable event listener.
		/// </summary>
		void Off();

		/// <summary>
		/// Permanently disable event listener.
		/// </summary>
		void Clear();
	}
}