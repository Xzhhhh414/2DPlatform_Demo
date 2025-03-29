namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// GroupedItems sample.
	/// Items grouped by keys.
	/// </summary>
	[Obsolete("Replaced with new GroupedList<IGroupedListItem>(...)")]
	public class GroupedItems : GroupedList<IGroupedListItem>
	{
		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected override IGroupedListItem GetGroup(IGroupedListItem item)
		{
			var name = item.Name.Length > 0 ? item.Name[0].ToString() : string.Empty;

			foreach (var group in GroupsWithItems.Keys)
			{
				if (group.Name == name)
				{
					return group;
				}
			}

			return new GroupedListGroup() { Name = name };
		}
	}
}