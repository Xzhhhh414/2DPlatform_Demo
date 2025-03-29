namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// Linear GroupedTileView.
	/// </summary>
	public class LinearGroupedTileView : ListViewCustom<GroupedTileViewComponentBase, Photo>
	{
		/// <summary>
		/// Real DataSource (use instead of DataSource).
		/// </summary>
		public ObservableList<Photo> RealDataSource = new ObservableList<Photo>();

		/// <summary>
		/// Grouped data.
		/// </summary>
		public LinearGroupedList<Photo> GroupedData = new LinearGroupedList<Photo>(x => x.IsGroup);

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			GroupedData.EmptyHeaderItem = new Photo() { IsGroup = true, IsEmpty = true };
			GroupedData.EmptyItem = new Photo() { IsEmpty = true };
			GroupedData.ItemsPerBlock = ListRenderer.GetItemsPerBlock();

			GroupedData.Input = RealDataSource;
			GroupedData.Output = DataSource;
		}

		/// <inheritdoc/>
		public override void UpdateItems()
		{
			base.UpdateItems();

			GroupedData.ItemsPerBlock = ListRenderer.GetItemsPerBlock();
		}

		/// <inheritdoc/>
		public override void Resize()
		{
			base.Resize();

			GroupedData.ItemsPerBlock = ListRenderer.GetItemsPerBlock();
		}
	}
}