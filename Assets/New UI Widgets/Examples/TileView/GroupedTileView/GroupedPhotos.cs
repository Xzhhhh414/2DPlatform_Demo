namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// GroupedPhotos example.
	/// Items grouped by keys.
	/// </summary>
	[Obsolete("Replaced with new GroupedList<Photo>(...)")]
	public class GroupedPhotos : GroupedList<Photo>
	{
		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected override Photo GetGroup(Photo item)
		{
			var date = item.Created.Date;

			foreach (var group in GroupsWithItems.Keys)
			{
				if (group.Created == date)
				{
					return group;
				}
			}

			return new Photo() { Created = item.Created.Date, IsGroup = true };
		}
	}
}