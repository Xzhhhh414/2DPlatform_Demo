namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviourInteractable,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable, UIThemes.ITargetOwner
	{
		/// <summary>
		/// Scroll data for the ListViewBase.
		/// </summary>
		protected abstract class ListViewScrollData<TItem> : ListViewBaseScrollData
		{
			/// <summary>
			/// Displayed items.
			/// </summary>
			protected readonly List<TItem> DisplayedItems = new List<TItem>();

			void UpdateItems(List<int> indices)
			{
				DisplayedIndices.Clear();
				DisplayedIndices.AddRange(indices);

				DisplayedItems.Clear();

				foreach (var index in indices)
				{
					DisplayedItems.Add(GetItem(index));
				}
			}

			/// <summary>
			/// Get item.
			/// </summary>
			/// <param name="index">Index/</param>
			/// <returns>Item.</returns>
			protected abstract TItem GetItem(int index);

			/// <summary>
			/// Get index of item.
			/// </summary>
			/// <param name="item">Item.</param>
			/// <returns>Index.</returns>
			protected abstract int IndexOf(TItem item);

			void UpdateScroll()
			{
				ScrollPosition = ListView.GetScrollPosition();
				ItemsPositions.Clear();

				for (int i = 0; i < DisplayedIndices.Count; i++)
				{
					var index = DisplayedIndices[i];
					var item = DisplayedItems[i];
					if ((index < ListView.GetItemsCount()) && !EqualityComparer<TItem>.Default.Equals(GetItem(index), item))
					{
						index = IndexOf(item);
					}

					if (index < 0)
					{
						ItemsPositions.Add(null);
						continue;
					}

					ItemsPositions.Add(ListView.GetItemPosition(index, false));
				}
			}

			void FixScroll()
			{
				if (DisplayedIndices.Count == 0)
				{
					return;
				}

				var scroll_position = ListView.GetScrollPosition();
				for (int i = 0; i < ItemsPositions.Count; i++)
				{
					var item_position = ItemsPositions[i];
					var index = DisplayedIndices[i];
					var item = DisplayedItems[i];
					if ((index < ListView.GetItemsCount()) && !EqualityComparer<TItem>.Default.Equals(GetItem(index), item))
					{
						index = IndexOf(item);
					}

					if (index < 0)
					{
						continue;
					}

					if (!item_position.HasValue)
					{
						continue;
					}

					var pos = scroll_position + ListView.GetItemPosition(index, false) - item_position.Value;
					ListView.ScrollToPosition(pos);
					break;
				}

				ScrollPosition = ListView.GetScrollPosition();
			}

			/// <inheritdoc/>
			public override void Update(List<int> indices)
			{
				if (Restore && RetainScrollPosition)
				{
					Restore = false;
					FixScroll();
				}

				UpdateItems(indices);
				UpdateScroll();
			}
		}
	}
}