namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// NestedListView item.
	/// </summary>
	[Serializable]
	public class NestedListViewItem
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		[SerializeField]
		List<ListViewIconsItemDescription> items = new List<ListViewIconsItemDescription>();

		ObservableList<ListViewIconsItemDescription> observableItems;

		/// <summary>
		/// Items.
		/// </summary>
		public ObservableList<ListViewIconsItemDescription> Items
		{
			get
			{
				observableItems ??= new ObservableList<ListViewIconsItemDescription>(items);

				return observableItems;
			}

			set => observableItems = value;
		}
	}
}