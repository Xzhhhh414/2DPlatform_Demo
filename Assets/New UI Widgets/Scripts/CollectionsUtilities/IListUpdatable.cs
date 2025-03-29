namespace UIWidgets
{
	/// <summary>
	/// Updatable list interface.
	/// </summary>
	public interface IListUpdatable
	{
		/// <summary>
		/// Maintains performance while items are added/removed/changed by preventing the widgets from drawing until the EndUpdate method is called.
		/// </summary>
		/// <returns>Updater.</returns>
		ListUpdater BeginUpdate();

		/// <summary>
		/// Ends the update and raise OnChange if something was changed.
		/// </summary>
		void EndUpdate();
	}
}