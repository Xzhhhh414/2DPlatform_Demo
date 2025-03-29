namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UIWidgets.Extensions;
	using UIWidgets.Internal;
	using UIWidgets.Pool;
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
		/// ListView renderer with staggered layout.
		/// </summary>
		protected class TileViewStaggered : ListViewTypeSize
		{
			class ItemPositionComparer : IComparer<TileViewStaggeredItemPosition>
			{
				public int Compare(TileViewStaggeredItemPosition x, TileViewStaggeredItemPosition y)
				{
					var pos_compare = x.Position.CompareTo(y.Position);
					if (pos_compare != 0)
					{
						return pos_compare;
					}

					return x.Block.CompareTo(y.Block);
				}
			}

			/// <summary>
			/// Block.
			/// Contains list of indices in the block.
			/// </summary>
			class Block : List<int>
			{
			}

			/// <summary>
			/// List of blocks.
			/// </summary>
			readonly List<Block> BlocksIndices = new List<Block>();

			readonly List<float> BlocksFullSizes = new List<float>();

			readonly List<float> LayoutPaddingStart;

			readonly List<float> LayoutPaddingEnd;

			readonly SortedList<TileViewStaggeredItemPosition, int> OrderedIndices = new SortedList<TileViewStaggeredItemPosition, int>(new ItemPositionComparer());

			/// <summary>
			/// Blocks count.
			/// </summary>
			int Blocks;

			/// <summary>
			/// Initializes a new instance of the <see cref="TileViewStaggered"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="instanceSizes">Container for the instances sizes.</param>
			public TileViewStaggered(ListViewCustom<TItemView, TItem> owner, IInstanceSizes<TItem> instanceSizes = null)
				: base(owner, instanceSizes)
			{
				if (Owner.Layout == null)
				{
					Debug.LogWarning("TileViewStaggered requires Container.EasyLayout component.", Owner);
					return;
				}

				if (Owner.ChangeLayoutType)
				{
					Owner.Layout.LayoutType = LayoutTypes.Staggered;
				}
				else if (Owner.Layout.LayoutType != LayoutTypes.Staggered)
				{
					Debug.LogWarning("EasyLayout type should be set to Staggered when used TileViewStaggered.", Owner);
				}

				LayoutPaddingStart = Owner.Layout.StaggeredSettings.PaddingInnerStart;
				LayoutPaddingEnd = Owner.Layout.StaggeredSettings.PaddingInnerEnd;
			}

			/// <inheritdoc/>
			public override void DirectionChanged()
			{
				Owner.Layout.MainAxis = Owner.IsHorizontal() ? Axis.Horizontal : Axis.Vertical;
			}

			/// <inheritdoc/>
			public override bool IsTileView => true;

			/// <inheritdoc/>
			public override bool IsVirtualizationSupported()
			{
				var scroll_rect_specified = Owner.ScrollRect != null;
				var container_specified = Owner.Container != null;
				var valid_layout = container_specified && ((Owner.layout != null) || Owner.Container.TryGetComponent<EasyLayout>(out var _));

				return scroll_rect_specified && valid_layout;
			}

			/// <inheritdoc/>
			public override void CalculateMaxVisibleItems()
			{
				CalculateInstancesSizes(Owner.DataSource, false);

				MaxVisibleItems = CalculateMaxVisibleItems(Owner.DataSource);
			}

			/// <inheritdoc/>
			protected override int CalculateMaxVisibleItems(ObservableList<TItem> items)
			{
				if (!Owner.Virtualization)
				{
					return Owner.DataSource.Count;
				}

				var spacing_x = Owner.GetItemSpacingX();
				var spacing_y = Owner.GetItemSpacingY();

				var height = ViewportHeight();
				var width = ViewportWidth();

				Blocks = Owner.IsHorizontal()
					? Mathf.FloorToInt(height / (Owner.DefaultInstanceSize.y + spacing_y))
					: Mathf.FloorToInt(width / (Owner.DefaultInstanceSize.x + spacing_x));

				Blocks = Mathf.Max(1, Blocks);
				Blocks = Owner.LayoutBridge.BlocksConstraint(Blocks);

				CalculateBlocksIndices();

				var max_items_per_block = Owner.IsHorizontal()
					? MaxVisible(true, width, spacing_x) + 1
					: MaxVisible(false, height, spacing_y) + 1;
				max_items_per_block = Mathf.Max(max_items_per_block, MinVisibleItems);

				return Blocks * max_items_per_block;
			}

			static void EnsureListSize<T>(List<T> list, int size)
			{
				for (int i = list.Count; i < size; i++)
				{
					list.Add(default);
				}
			}

			int NextBlockIndex()
			{
				var index = 0;
				var min_size = BlocksFullSizes[0];

				for (int i = 1; i < BlocksFullSizes.Count; i++)
				{
					if (BlocksFullSizes[i] < min_size)
					{
						index = i;
						min_size = BlocksFullSizes[i];
					}
				}

				return index;
			}

			void InsertToBlock(int block_index, int index)
			{
				var spacing = Owner.LayoutBridge.GetSpacing();

				for (int i = BlocksIndices.Count; i <= block_index; i++)
				{
					BlocksIndices.Add(new Block());
				}

				var row = BlocksIndices[block_index];
				row.Add(index);

				if (row.Count > 1)
				{
					BlocksFullSizes[block_index] += spacing;
				}

				OrderedIndices.Add(new TileViewStaggeredItemPosition(block_index, BlocksFullSizes[block_index]), index);

				BlocksFullSizes[block_index] += GetInstanceSize(index);
			}

			/// <summary>
			/// Calculate block indices.
			/// </summary>
			protected void CalculateBlocksIndices()
			{
				BlocksFullSizes.Clear();

				EnsureListSize(BlocksFullSizes, Blocks);

				BlocksIndices.Clear();
				OrderedIndices.Clear();

				for (int i = 0; i < Owner.DataSource.Count; i++)
				{
					InsertToBlock(NextBlockIndex(), i);
				}
			}

			/// <inheritdoc/>
			public override float TopFillerSize()
			{
				Owner.Layout.StaggeredSettings.PaddingInnerStart = LayoutPaddingStart;

				return 0f;
			}

			/// <inheritdoc/>
			public override float BottomFillerSize()
			{
				Owner.Layout.StaggeredSettings.PaddingInnerEnd = LayoutPaddingEnd;

				return 0f;
			}

			/// <inheritdoc/>
			protected override int GetBlockIndex(int index)
			{
				for (int i = 0; i < BlocksIndices.Count; i++)
				{
					if (BlocksIndices[i].Contains(index))
					{
						return i;
					}
				}

				return 0;
			}

			/// <inheritdoc/>
			public override float GetItemPosition(int index)
			{
				if (Owner.DataSource.Count == 0)
				{
					return 0;
				}

				var block = BlocksIndices[GetBlockIndex(index)];

				var size = IsRequiredCenterTheItems() ? CenteredFillerSize() : 0f;
				var spacing = Owner.LayoutBridge.GetSpacing();
				for (int i = 0; i < block.Count; i++)
				{
					if (block[i] == index)
					{
						break;
					}

					size += GetInstanceSize(block[i]) + spacing;
				}

				if (size > 0)
				{
					size -= spacing;
				}

				return size + Owner.LayoutBridge.GetMargin();
			}

			/// <inheritdoc/>
			public override float GetItemPositionMiddle(int index)
			{
				var start = GetItemPosition(index);
				var end = GetItemPositionBottom(index);
				return start + ((end - start) / 2);
			}

			/// <inheritdoc/>
			public override float GetItemPositionBottom(int index)
			{
				if (Owner.DataSource.Count == 0)
				{
					return 0;
				}

				var block = BlocksIndices[GetBlockIndex(index)];

				var size = 0f;
				var spacing = Owner.LayoutBridge.GetSpacing();
				for (int i = 0; i < block.Count; i++)
				{
					size += GetInstanceSize(block[i]) + spacing;

					if (block[i] == index)
					{
						break;
					}
				}

				if (size > 0)
				{
					size -= spacing;
				}

				return size + Owner.LayoutBridge.GetMargin() - Owner.Viewport.ScaledAxisSize;
			}

			int GetIndexAtBlock(float position, int blockIndex)
			{
				return GetIndexAtBlock(position, blockIndex, NearestType.Before);
			}

			int GetIndexAtBlock(float position, int blockIndex, NearestType type)
			{
				if (Owner.DataSource.Count == 0)
				{
					return 0;
				}

				position -= IsRequiredCenterTheItems() ? CenteredFillerSize() : 0f;
				var spacing = Owner.LayoutBridge.GetSpacing();
				var block = BlocksIndices[blockIndex];
				var index = block[0];

				var item_size = GetInstanceSize(index);

				if (position > item_size)
				{
					position -= item_size;

					for (var i = 1; i < block.Count; i++)
					{
						index = block[i];

						item_size = GetInstanceSize(block[i]) + spacing;

						if (position < item_size)
						{
							break;
						}

						position -= item_size;
					}
				}

				switch (type)
				{
					case NearestType.Auto:
						if (position >= (GetInstanceSize(index) / 2f))
						{
							index += 1;
						}

						break;
					case NearestType.Before:
						break;
					case NearestType.After:
						index += 1;
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported NearestType: {0}", EnumHelper<NearestType>.ToString(type)));
				}

				return index;
			}

			/// <inheritdoc/>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var pos = GetPosition() - Owner.LayoutBridge.GetMargin();
				if (!LoopedListAvailable)
				{
					pos = Mathf.Max(pos, 0f);
				}

				return GetIndexAtBlock(pos, 0);
			}

			/// <inheritdoc/>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				var pos = Mathf.Max(0f, GetPosition() - Owner.LayoutBridge.GetMargin());
				var last_visible_index = GetIndexAtBlock(pos + Owner.Viewport.ScaledAxisSize, BlocksIndices.Count - 1);

				return last_visible_index;
			}

			/// <inheritdoc/>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Required")]
			public override bool UpdateDisplayedIndices()
			{
				using var _ = ListPool<int>.Get(out var indices);

				LayoutPaddingStart.Clear();
				LayoutPaddingEnd.Clear();

				EnsureListSize(LayoutPaddingStart, Blocks);
				EnsureListSize(LayoutPaddingEnd, Blocks);

				var start = Mathf.Max(0f, GetPosition() - Owner.LayoutBridge.GetMargin());
				var end = start + Owner.Viewport.ScaledAxisSize;
				var spacing = Owner.LayoutBridge.GetSpacing();

				foreach (var kv in OrderedIndices)
				{
					var info = kv.Key;
					var index = kv.Value;
					var size = GetInstanceSize(index);
					var item_end = info.Position + size;

					if (item_end < start)
					{
						LayoutPaddingStart[info.Block] = info.Position + size + spacing;
					}
					else if (info.Position > end)
					{
						LayoutPaddingEnd[info.Block] += size + spacing;
					}
					else
					{
						indices.Add(index);
					}
				}

				if (Owner.DisplayedIndices == indices)
				{
					return false;
				}

				Owner.DisplayedIndices.Clear();
				Owner.DisplayedIndices.AddRange(indices);
				Owner.Layout.NeedUpdateLayout();

				return true;
			}

			/// <inheritdoc/>
			public override int GetNearestIndex(Vector2 point, NearestType type)
			{
				var spacing = Owner.IsHorizontal() ? Owner.LayoutBridge.GetSpacingY() : Owner.LayoutBridge.GetSpacingX();
				var size = Owner.IsHorizontal() ? Owner.DefaultInstanceSize.y : Owner.DefaultInstanceSize.x;

				var block_pos = Owner.IsHorizontal() ? Mathf.Abs(point.y) - Owner.LayoutBridge.GetFullMarginY() : point.x - Owner.LayoutBridge.GetFullMarginX();
				var item_pos = (Owner.IsHorizontal() ? point.x : Mathf.Abs(point.y)) - Owner.LayoutBridge.GetMargin();
				var block_index = (block_pos < size) ? 0 : Mathf.FloorToInt((block_pos - size) / (size + spacing)) + 1;

				return GetIndexAtBlock(item_pos, block_index, type);
			}

			/// <inheritdoc/>
			public override int GetItemsPerBlock()
			{
				return Blocks;
			}

			/// <inheritdoc/>
			public override float ListSize()
			{
				if (Owner.DataSource.Count == 0)
				{
					return 0;
				}

				var max = BlocksFullSizes[0];
				for (int i = 1; i < BlocksFullSizes.Count; i++)
				{
					max = Mathf.Max(max, BlocksFullSizes[i]);
				}

				return max + Owner.LayoutBridge.GetFullMargin();
			}

			float GetPositionAtBlock(int index)
			{
				var block_index = GetBlockIndex(index);
				var block = BlocksIndices[block_index];
				var spacing = Owner.LayoutBridge.GetSpacing();

				var position = 0f;
				for (var i = 1; i < block.Count; i++)
				{
					var item_index = block[i];
					if (item_index == index)
					{
						break;
					}

					position += GetInstanceSize(item_index) + spacing;
				}

				return position;
			}

			bool BlockExists(int blockIndex)
			{
				if (blockIndex < 0)
				{
					return false;
				}

				if (blockIndex >= BlocksIndices.Count)
				{
					return false;
				}

				return true;
			}

			/// <inheritdoc/>
			public override bool OnItemMove(AxisEventData eventData, ListViewItem item)
			{
				var block_index = GetBlockIndex(item.Index);
				var position_at_block = GetPositionAtBlock(item.Index);
				var step = 0;
				var next_index = -1;
				switch (eventData.moveDir)
				{
					case MoveDirection.Left:
						if (Owner.IsHorizontal())
						{
							step = -1;
						}
						else
						{
							if (BlockExists(block_index - 1))
							{
								next_index = GetIndexAtBlock(position_at_block, block_index - 1, NearestType.Before);
							}
						}

						break;
					case MoveDirection.Right:
						if (Owner.IsHorizontal())
						{
							step = 1;
						}
						else
						{
							if (BlockExists(block_index + 1))
							{
								next_index = GetIndexAtBlock(position_at_block, block_index + 1, NearestType.Before);
							}
						}

						break;
					case MoveDirection.Up:
						if (Owner.IsHorizontal())
						{
							if (BlockExists(block_index - 1))
							{
								next_index = GetIndexAtBlock(position_at_block, block_index - 1, NearestType.Before);
							}
						}
						else
						{
							step = -1;
						}

						break;
					case MoveDirection.Down:
						if (Owner.IsHorizontal())
						{
							if (BlockExists(block_index + 1))
							{
								next_index = GetIndexAtBlock(position_at_block, block_index + 1, NearestType.Before);
							}
						}
						else
						{
							step = 1;
						}

						break;
				}

				if (step != 0)
				{
					next_index = GetSelectableComponentIndex(item.Index, step);
				}

				return Owner.Navigate(eventData, next_index, next_index > item.Index);
			}

			/// <inheritdoc/>
			public override void GetDebugInfo(System.Text.StringBuilder builder)
			{
				base.GetDebugInfo(builder);

				builder.AppendValue("Blocks: ", Blocks);

				builder.AppendLine("Blocks Indices");
				for (int i = 0; i < BlocksIndices.Count; i++)
				{
					var block = BlocksIndices[i];
					builder.AppendValue("\t", i, ". ", UtilitiesCollections.List2String(block));
				}

				builder.AppendLine("Blocks Sizes");
				for (int i = 0; i < BlocksFullSizes.Count; i++)
				{
					builder.AppendValue("\t", i, ". ", BlocksFullSizes[i]);
				}

				builder.AppendValue("LayoutPaddingStart: ", UtilitiesCollections.List2String(LayoutPaddingStart));
				builder.AppendValue("LayoutPaddingEnd: ", UtilitiesCollections.List2String(LayoutPaddingEnd));
			}
		}
	}
}