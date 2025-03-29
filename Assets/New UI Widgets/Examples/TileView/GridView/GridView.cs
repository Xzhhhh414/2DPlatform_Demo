namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// GridView.
	/// </summary>
	public class GridView : TileViewCustom<ListViewIconsItemComponent, ListViewIconsItemDescription>
	{
		/// <summary>
		/// Items per block.
		/// </summary>
		[SerializeField]
		protected int ItemsPerBlock = 10;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			using var _ = DataSource.BeginUpdate();

			for (int i = 0; i < ItemsPerBlock * 20; i++)
			{
				DataSource.Add(new ListViewIconsItemDescription() { Name = string.Format("Item {0:D3}", i) });
			}
		}

		/// <inheritdoc/>
		protected override ListViewTypeBase GetRenderer(ListViewType type)
		{
			return new GridViewFixed(this)
			{
				ItemsPerBlock = ItemsPerBlock,
			};
		}
	}
}