namespace UIWidgets
{
	using EasyLayoutNS;
	using UIWidgets.Extensions;
	using UnityEngine;
	using UnityEngine.UI;

	/// <content>
	/// Base class for the custom ListViews.
	/// </content>
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustom<TItem>, IUpdatable, ILateUpdatable, IListViewCallbacks<TItemView>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// GridView renderer with items of fixed size.
		/// </summary>
		protected class GridViewFixed : TileViewTypeFixed
		{
			int itemsPerBlock;

			/// <summary>
			/// Items per block (row or column).
			/// </summary>
			public int ItemsPerBlock
			{
				get => itemsPerBlock;

				set
				{
					itemsPerBlock = value;
					Owner.Layout.GridConstraintCount = value;

					CalculateMaxVisibleItems();
				}
			}

			/// <inheritdoc/>
			public override bool AllowSetContentSizeFitter => false;

			/// <summary>
			/// Initializes a new instance of the <see cref="GridViewFixed"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public GridViewFixed(ListViewCustom<TItemView, TItem> owner)
				: base(owner)
			{
				if (Owner.ChangeLayoutType && (Owner.Layout != null))
				{
					Owner.Layout.LayoutType = LayoutTypes.Grid;

					if (Owner.StyleTable)
					{
						Owner.Layout.GridConstraint = Owner.IsHorizontal() ? GridConstraints.FixedRowCount : GridConstraints.FixedColumnCount;
						Owner.Layout.GridConstraintCount = 1;
					}
				}

				if (Owner.ScrollRect != null)
				{
					Owner.ScrollRect.horizontal = true;
					Owner.ScrollRect.vertical = true;
				}

				if (Owner.Container.TryGetComponent<ContentSizeFitter>(out var fitter))
				{
					fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
					fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				}
			}

			/// <inheritdoc/>
			public override void ResetPosition()
			{
				Owner.ContainerAnchoredPosition = Vector2.zero;

				if (Owner.ScrollRect != null)
				{
					Owner.ScrollRect.horizontal = true;
					Owner.ScrollRect.vertical = true;
					Owner.ScrollRect.StopMovement();
				}
			}

			/// <inheritdoc/>
			public override void CalculateMaxVisibleItems()
			{
				if (Owner.IsHorizontal())
				{
					Rows = ItemsPerBlock;
					Columns = Mathf.CeilToInt(Owner.DataSource.Count / (float)ItemsPerBlock);
				}
				else
				{
					Columns = ItemsPerBlock;
					Rows = Mathf.CeilToInt(Owner.DataSource.Count / (float)ItemsPerBlock);
				}

				MaxVisibleItems = Owner.Virtualization ? (Columns * Rows) : Owner.DataSource.Count;
			}

			/// <inheritdoc/>
			public override bool UpdateDisplayedIndices()
			{
				// TODO calculate actually visible indices in block
				return base.UpdateDisplayedIndices();
			}

			/// <inheritdoc/>
			public override void UpdateLayout(bool recalculate = true)
			{
				// TODO calculate and set Owner.Layout.InnerPadding to match visible indices
				base.UpdateLayout(recalculate);
			}

			/// <inheritdoc/>
			public override int GetItemsPerBlock() => ItemsPerBlock;

			/// <inheritdoc/>
			public override void ValidateContentSize()
			{
			}

			/// <inheritdoc/>
			public override void GetDebugInfo(System.Text.StringBuilder builder)
			{
				base.GetDebugInfo(builder);

				builder.AppendValue("ItemsPerBlock: ", ItemsPerBlock);
				builder.AppendValue("Rows: ", Rows);
				builder.AppendValue("Columns: ", Columns);
			}
		}
	}
}