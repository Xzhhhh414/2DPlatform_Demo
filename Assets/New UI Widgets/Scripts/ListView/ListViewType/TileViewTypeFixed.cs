namespace UIWidgets
{
	using System;
	using EasyLayoutNS;
	using UIWidgets.Extensions;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <content>
	/// Base class for the custom ListViews.
	/// </content>
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustom<TItem>, IUpdatable, ILateUpdatable, IListViewCallbacks<TItemView>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// TileView renderer with items of fixed size.
		/// </summary>
		protected class TileViewTypeFixed : ListViewTypeFixed
		{
			/// <summary>
			/// Columns.
			/// </summary>
			protected int Columns;

			/// <summary>
			/// Rows.
			/// </summary>
			protected int Rows;

			/// <summary>
			/// Initializes a new instance of the <see cref="TileViewTypeFixed"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public TileViewTypeFixed(ListViewCustom<TItemView, TItem> owner)
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
			}

			/// <inheritdoc/>
			protected override void InitOnce()
			{
				base.InitOnce();

				if (Owner.Layout != null)
				{
					var children_size = Owner.IsHorizontal()
						? Owner.Layout.ChildrenWidth
						: Owner.Layout.ChildrenHeight;

					if (children_size != ChildrenSize.DoNothing)
					{
						var field = Owner.IsHorizontal() ? "ChildrenWidth" : "ChildrenHeight";
						var template = "ListType does not match with Container.EasyLayout settings and this can cause scroll problems. Please change ListType to TileViewWithVariableSize or EasyLayout.{0} to DoNothing.";
						Debug.LogWarning(string.Format(template, field), Owner);
					}
				}
			}

			/// <inheritdoc/>
			public override bool IsTileView => true;

			/// <summary>
			/// Gets the blocks count.
			/// </summary>
			/// <returns>The blocks count.</returns>
			/// <param name="items">Items.</param>
			protected int GetBlocksCount(int items)
			{
				return items < 0
					? Mathf.FloorToInt((float)items / (float)GetItemsPerBlock())
					: Mathf.CeilToInt((float)items / (float)GetItemsPerBlock());
			}

			/// <inheritdoc/>
			public override void CalculateMaxVisibleItems()
			{
				var spacing_x = Owner.GetItemSpacingX();
				var spacing_y = Owner.GetItemSpacingY();

				var width = ViewportWidth();
				var height = ViewportHeight();

				if (Owner.IsHorizontal())
				{
					Columns = Mathf.CeilToInt(width / (Owner.DefaultInstanceSize.x + spacing_x)) + 1;
					Columns = Mathf.Max(MinVisibleItems, Columns);

					Rows = Mathf.FloorToInt(height / (Owner.DefaultInstanceSize.y + spacing_y));
					Rows = Mathf.Max(1, Rows);
					Rows = Owner.LayoutBridge.RowsConstraint(Rows);
				}
				else
				{
					Columns = Mathf.FloorToInt(width / (Owner.DefaultInstanceSize.x + spacing_x));
					Columns = Mathf.Max(1, Columns);
					Columns = Owner.LayoutBridge.ColumnsConstraint(Columns);

					Rows = Mathf.CeilToInt(height / (Owner.DefaultInstanceSize.y + spacing_y)) + 1;
					Rows = Mathf.Max(MinVisibleItems, Rows);
				}

				MaxVisibleItems = Owner.Virtualization ? (Columns * Rows) : Owner.DataSource.Count;
			}

			/// <inheritdoc/>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var first = base.GetFirstVisibleIndex(strict) * GetItemsPerBlock();

				if (first > (Owner.DataSource.Count - 1))
				{
					first = Owner.DataSource.Count - 2;
				}

				return Mathf.Max(0, first);
			}

			/// <inheritdoc/>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				return ((base.GetLastVisibleIndex(strict) + 1) * GetItemsPerBlock()) - 1;
			}

			/// <inheritdoc/>
			public override float BottomFillerSize()
			{
				var last = Owner.DisplayedIndexLast;
				var blocks = last < 0 ? 0 : GetBlocksCount(Owner.DataSource.Count - last - 1);

				return (blocks == 0) ? 0f : blocks * GetItemSize();
			}

			/// <inheritdoc/>
			public override int GetNearestIndex(Vector2 point, NearestType type)
			{
				// block index
				var pos_block = Owner.IsHorizontal() ? point.x : Mathf.Abs(point.y);
				pos_block -= Owner.LayoutBridge.GetMargin();
				pos_block -= IsRequiredCenterTheItems() ? CenteredFillerSize() : 0f;

				var block = type switch
				{
					NearestType.Auto => (int)Math.Round(pos_block / GetItemSize(), MidpointRounding.AwayFromZero),
					NearestType.Before => Mathf.FloorToInt(pos_block / GetItemSize()),
					NearestType.After => Mathf.CeilToInt(pos_block / GetItemSize()),
					_ => throw new NotSupportedException(string.Format("Unsupported NearestType: {0}", EnumHelper<NearestType>.ToString(type))),
				};

				// item index in block
				var pos_elem = Owner.IsHorizontal() ? Mathf.Abs(point.y) : point.x;
				var size = Owner.IsHorizontal() ? Owner.DefaultInstanceSize.y + Owner.GetItemSpacingY() : Owner.DefaultInstanceSize.x + Owner.GetItemSpacingX();

				var k = type switch
				{
					NearestType.Auto => Mathf.RoundToInt(pos_elem / size),
					NearestType.Before => Mathf.FloorToInt(pos_elem / size),
					NearestType.After => Mathf.CeilToInt(pos_elem / size),
					_ => throw new NotSupportedException(string.Format("Unsupported NearestType: {0}", EnumHelper<NearestType>.ToString(type))),
				};
				var index = (block * GetItemsPerBlock()) + k;

				return index;
			}

			/// <inheritdoc/>
			public override int GetNearestItemIndex()
			{
				return base.GetNearestItemIndex() * GetItemsPerBlock();
			}

			/// <inheritdoc/>
			public override int GetItemsPerBlock()
			{
				return Owner.IsHorizontal() ? Rows : Columns;
			}

			/// <inheritdoc/>
			protected override int GetBlockIndex(int index)
			{
				return Mathf.FloorToInt((float)index / (float)GetItemsPerBlock());
			}

			/// <inheritdoc/>
			public override bool IsVirtualizationSupported()
			{
				var scroll_rect_specified = Owner.ScrollRect != null;
				var container_specified = Owner.Container != null;
				var valid_layout = container_specified && ((Owner.layout != null) || Owner.Container.TryGetComponent<EasyLayout>(out var _));

				return scroll_rect_specified && valid_layout;
			}

			/// <inheritdoc/>
			public override bool OnItemMove(AxisEventData eventData, ListViewItem item)
			{
				var step = 0;
				switch (eventData.moveDir)
				{
					case MoveDirection.Left:
						step = Owner.IsHorizontal() ? -1 : -GetItemsPerBlock();
						break;
					case MoveDirection.Right:
						step = Owner.IsHorizontal() ? 1 : GetItemsPerBlock();
						break;
					case MoveDirection.Up:
						step = Owner.IsHorizontal() ? -GetItemsPerBlock() : -1;
						break;
					case MoveDirection.Down:
						step = Owner.IsHorizontal() ? GetItemsPerBlock() : 1;
						break;
				}

				if (step == 0)
				{
					return false;
				}

				var target = GetSelectableComponentIndex(item.Index, step);

				return Owner.Navigate(eventData, target, target > item.Index);
			}

			/// <inheritdoc/>
			public override void ValidateContentSize()
			{
			}

			/// <inheritdoc/>
			public override void GetDebugInfo(System.Text.StringBuilder builder)
			{
				base.GetDebugInfo(builder);

				builder.AppendValue("Rows: ", Rows);
				builder.AppendValue("Columns: ", Columns);
			}
		}
	}
}