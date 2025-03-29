namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustom<TItem>, IUpdatable, ILateUpdatable, IListViewCallbacks<TItemView>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// Scroll data for the ListView.
		/// </summary>
		protected sealed class ListViewScrollData : ListViewScrollData<TItem>
		{
			readonly ListViewCustom<TItemView, TItem> owner;

			/// <inheritdoc/>
			protected sealed override bool RetainScrollPosition => owner.RetainScrollPosition;

			/// <inheritdoc/>
			protected sealed override ListViewBase ListView => owner;

			/// <inheritdoc/>
			protected sealed override float Margin => owner.LayoutBridge.GetMargin();

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewScrollData"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public ListViewScrollData(ListViewCustom<TItemView, TItem> owner)
			{
				this.owner = owner;
			}

			/// <inheritdoc/>
			protected sealed override TItem GetItem(int index) => owner.DataSource[index];

			/// <inheritdoc/>
			protected sealed override int IndexOf(TItem item) => owner.DataSource.IndexOf(item);
		}
	}
}