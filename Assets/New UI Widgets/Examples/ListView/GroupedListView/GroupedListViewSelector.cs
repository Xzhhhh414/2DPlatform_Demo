namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// GroupedListView template selector.
	/// </summary>
	[SerializeField]
	public class GroupedListViewSelector : MonoBehaviour, IListViewTemplateSelector<GroupedListViewComponent, IGroupedListItem>
	{
		/// <summary>
		/// Group template.
		/// </summary>
		[SerializeField]
		public GroupedListViewComponent GroupTemplate;

		/// <summary>
		/// Item template.
		/// </summary>
		[SerializeField]
		public GroupedListViewComponent ItemTemplate;

		GroupedListViewComponent[] templates;

		/// <inheritdoc/>
		public GroupedListViewComponent[] AllTemplates()
		{
			templates ??= new[] { GroupTemplate, ItemTemplate };

			return templates;
		}

		/// <inheritdoc/>
		public GroupedListViewComponent Select(int index, IGroupedListItem item)
		{
			return item is GroupedListGroup ? GroupTemplate : ItemTemplate;
		}
	}
}