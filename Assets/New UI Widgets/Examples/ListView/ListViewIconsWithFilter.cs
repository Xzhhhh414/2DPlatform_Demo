namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Sample ListViewIcons with filter.
	/// </summary>
	public class ListViewIconsWithFilter : ListViewIcons
	{
		[SerializeField]
		[FormerlySerializedAs("m_items")]
		List<ListViewIconsItemDescription> listItems = new List<ListViewIconsItemDescription>();

		ObservableList<ListViewIconsItemDescription> originalItems;

		/// <summary>
		/// Get or sets items.
		/// </summary>
		public ObservableList<ListViewIconsItemDescription> OriginalItems
		{
			get
			{
				if (originalItems == null)
				{
					originalItems = new ObservableList<ListViewIconsItemDescription>(listItems);
					originalItems.OnChangeMono.AddListener(Filter);
				}

				return originalItems;
			}

			set
			{
				originalItems?.OnChangeMono.RemoveListener(Filter);

				originalItems = value;

				originalItems?.OnChangeMono.AddListener(Filter);

				Filter();
			}
		}

		/// <summary>
		/// Search string.
		/// </summary>
		protected string Search = string.Empty;

		/// <summary>
		/// Filter data using specified search string.
		/// </summary>
		/// <param name="search">Search string.</param>
		public void Filter(string search)
		{
			Search = search;
			Filter();
		}

		/// <summary>
		/// Copy items from OriginalItems to DataSource if it's match specified string.
		/// </summary>
		protected void Filter()
		{
			using var _ = DataSource.BeginUpdate();
			DataSource.Clear();

			if (string.IsNullOrEmpty(Search))
			{
				// if search string not specified add all items
				DataSource.AddRange(OriginalItems);
			}
			else
			{
				// else add items with name starts with specified string
				foreach (var item in OriginalItems)
				{
					if (item.Name.StartsWith(Search, System.StringComparison.InvariantCulture))
					{
						DataSource.Add(item);
					}
				}
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			// call Filter() to set initial DataSource
			Filter();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			originalItems?.OnChangeMono.RemoveListener(Filter);

			base.OnDestroy();
		}
	}
}