namespace UIWidgets.Examples
{
	/// <summary>
	/// GroupedTileView
	/// </summary>
	public class GroupedTileView : GroupedTileViewBase
	{
		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			GroupedData.GroupComparison = (x, y) => x.Created.CompareTo(y.Created);
			GroupedData.EmptyGroupItem = new Photo() { IsGroup = true, IsEmpty = true };
			GroupedData.EmptyItem = new Photo() { IsEmpty = true };
			GroupedData.ItemsPerBlock = ListRenderer.GetItemsPerBlock();

			DataSource = GroupedData.Output;
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