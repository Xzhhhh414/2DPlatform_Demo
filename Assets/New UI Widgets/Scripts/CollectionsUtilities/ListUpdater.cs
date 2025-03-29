namespace UIWidgets
{
	/// <summary>
	/// Updater.
	/// </summary>
	public readonly ref struct ListUpdater
	{
		readonly IListUpdatable list;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListUpdater"/> struct.
		/// </summary>
		/// <param name="instance">Instance.</param>
		public ListUpdater(IListUpdatable instance) => list = instance;

		/// <summary>
		/// Dispose (end update).
		/// </summary>
		public void Dispose() => list.EndUpdate();
	}
}