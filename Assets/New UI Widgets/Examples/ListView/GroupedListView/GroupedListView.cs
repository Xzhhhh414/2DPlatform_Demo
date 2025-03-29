namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// GroupedListView
	/// </summary>
	public class GroupedListView : ListViewCustom<GroupedListViewComponent, IGroupedListItem>
	{
		/// <summary>
		/// Grouped data.
		/// </summary>
		public GroupedList<IGroupedListItem> GroupedData = new GroupedList<IGroupedListItem>((groups, item) =>
		{
			var name = item.Name.Length > 0 ? item.Name[0].ToString() : string.Empty;

			foreach (var group in groups)
			{
				if (group.Name == name)
				{
					return group;
				}
			}

			return new GroupedListGroup() { Name = name };
		});

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			GroupedData.GroupComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);
			DataSource = GroupedData.Output;

			CanSelect = (index) => DataSource[index] is GroupedListItem;
		}
	}
}