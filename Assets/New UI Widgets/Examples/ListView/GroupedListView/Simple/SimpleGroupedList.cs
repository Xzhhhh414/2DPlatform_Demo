namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// GroupedItems sample.
	/// Items grouped by keys.
	/// </summary>
	[Obsolete("Replaced with new GroupedList<SimpleGroupedItem>(...)")]
	public class SimpleGroupedList : GroupedList<SimpleGroupedItem>
	{
		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected override SimpleGroupedItem GetGroup(SimpleGroupedItem item)
		{
			// determine a unique group feature; here used the first letter of the name
			var name = item.Name.Length > 0 ? item.Name[0].ToString() : string.Empty;

			// first check is such group already exists
			foreach (var group in GroupsWithItems.Keys)
			{
				if (group.Name == name)
				{
					return group;
				}
			}

			// if the group does not exists create a new item and mark it as the group item
			return new SimpleGroupedItem()
			{
				Name = name,
				IsGroup = true,
			};
		}
	}
}