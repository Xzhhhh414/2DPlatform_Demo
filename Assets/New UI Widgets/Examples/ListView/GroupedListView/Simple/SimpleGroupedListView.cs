namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// SimpleGroupedListView
	/// </summary>
	public class SimpleGroupedListView : ListViewCustom<SimpleGroupedComponent, SimpleGroupedItem>
	{
		/// <summary>
		/// Grouped data.
		/// </summary>
		public GroupedList<SimpleGroupedItem> GroupedData = new GroupedList<SimpleGroupedItem>((groups, item) =>
		{
			// determine a unique group feature; here used the first letter of the name
			var name = item.Name.Length > 0 ? item.Name[0].ToString() : string.Empty;

			// first check is such group already exists
			foreach (var group in groups)
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
		});

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void InitOnce()
		{
			base.InitOnce();

			// set groups sort by name
			GroupedData.GroupComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

			// set data source
			DataSource = GroupedData.Output;

			// allow select only ordinary items, not the group items
			CanSelect = index => !DataSource[index].IsGroup;
		}
	}
}