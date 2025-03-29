namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Base class for the Tabs widget.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/containers/tabs.html")]
	public abstract class TabsBase : MonoBehaviourInitiable
	{
		/// <summary>
		/// Index of the selected tab.
		/// </summary>
		public abstract int SelectedTabIndex
		{
			get;
		}

		/// <summary>
		/// Tabs count.
		/// </summary>
		protected abstract int TabsCount
		{
			get;
		}

		/// <summary>
		/// Selects the tab.
		/// </summary>
		/// <param name="index">Tab index.</param>
		public abstract void SelectTab(int index);

		/// <summary>
		/// Check is tab can be selected.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>true if tab can be selected; otherwise false.</returns>
		protected abstract bool CanSelectTabByIndex(int index);

		/// <summary>
		/// Select the next tab.
		/// If current tab is last will be selected the first tab.
		/// </summary>
		public virtual void NextTab() => NextTab(true);

		/// <summary>
		/// Select the next tab.
		/// </summary>
		/// <param name="loop">Select the first tab if current tab is last.</param>
		public virtual void NextTab(bool loop) => NextTab(SelectedTabIndex, +1, loop);

		/// <summary>
		/// Select previous tab.
		/// If current tab is first will be selected the last tab.
		/// </summary>
		public virtual void PreviousTab() => PreviousTab(true);

		/// <summary>
		/// Select the previous tab.
		/// </summary>
		/// <param name="loop">Select the last tab if current tab is first.</param>
		public virtual void PreviousTab(bool loop) => NextTab(SelectedTabIndex, -1, loop);

		/// <summary>
		/// Select the next tab.
		/// </summary>
		/// <param name="tab_index">Tab index.</param>
		/// <param name="direction">+1 to select next tab; -1 to select previous tab.</param>
		/// <param name="loop">Select the first/last tab if current tab is last/first.</param>
		protected virtual void NextTab(int tab_index, int direction, bool loop)
		{
			tab_index += direction;

			if (tab_index == TabsCount)
			{
				if (loop)
				{
					tab_index = 0;
				}
				else
				{
					return;
				}
			}
			else if (tab_index < 0)
			{
				if (loop)
				{
					tab_index = TabsCount - 1;
				}
				else
				{
					return;
				}
			}

			if (CanSelectTabByIndex(tab_index))
			{
				SelectTab(tab_index);
			}
			else
			{
				NextTab(tab_index, direction, loop);
			}
		}
	}
}