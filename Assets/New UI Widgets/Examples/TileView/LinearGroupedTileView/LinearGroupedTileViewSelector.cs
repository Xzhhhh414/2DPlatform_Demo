namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// LinearGroupedTileView template selector.
	/// </summary>
	public class LinearGroupedTileViewSelector : MonoBehaviour, IListViewTemplateSelector<GroupedTileViewComponentBase, Photo>
	{
		/// <summary>
		/// Header.
		/// </summary>
		[SerializeField]
		protected GroupedTileViewComponentBase Header;

		/// <summary>
		/// Empty header.
		/// </summary>
		[SerializeField]
		protected GroupedTileViewComponentBase EmptyHeader;

		/// <summary>
		/// Item.
		/// </summary>
		[SerializeField]
		protected GroupedTileViewComponentBase Item;

		/// <summary>
		/// Empty item.
		/// </summary>
		[SerializeField]
		protected GroupedTileViewComponentBase EmptyItem;

		GroupedTileViewComponentBase[] templates;

		/// <inheritdoc/>
		public GroupedTileViewComponentBase[] AllTemplates()
		{
			templates ??= new[] { Header, EmptyHeader, Item, EmptyItem, };

			return templates;
		}

		/// <inheritdoc/>
		public GroupedTileViewComponentBase Select(int index, Photo item)
		{
			if (item.IsGroup)
			{
				return item.IsEmpty ? EmptyHeader : Header;
			}

			return item.IsEmpty ? EmptyItem : Item;
		}
	}
}