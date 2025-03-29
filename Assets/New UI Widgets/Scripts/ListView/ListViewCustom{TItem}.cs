namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	/// <typeparam name="TItem">Type of item.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
	[DataBindSupport]
	public abstract class ListViewCustom<TItem> : ListViewCustomBase
	{
		/// <summary>
		/// Item event.
		/// </summary>
		[Serializable]
		public class ItemEvent : UnityEvent<int, TItem>
		{
		}

		/// <summary>
		/// The items.
		/// </summary>
		[SerializeField]
		protected List<TItem> customItems = new List<TItem>();

		/// <summary>
		/// Data source.
		/// </summary>
#if UNITY_2020_1_OR_NEWER
		[NonSerialized]
#endif
		protected ObservableList<TItem> dataSource;

		/// <summary>
		/// Data source.
		/// </summary>
		[DataBindField]
		public abstract ObservableList<TItem> DataSource
		{
			get;
			set;
		}

		/// <summary>
		/// Selected item.
		/// </summary>
		[DataBindField]
		public TItem SelectedItem
		{
			get
			{
				if (SelectedIndex == -1)
				{
					return default;
				}

				return DataSource[SelectedIndex];
			}
		}

		/// <summary>
		/// Selected items.
		/// </summary>
		[DataBindField]
		public List<TItem> SelectedItems
		{
			get
			{
				var result = new List<TItem>(selectedIndices.Count);
				GetSelectedItems(result);

				return result;
			}

			set
			{
				SetSelectedItems(value, true);
			}
		}

		/// <summary>
		/// Item selected event.
		/// </summary>
		public ItemEvent OnItemSelected = new ItemEvent();

		/// <summary>
		/// Item deselected event.
		/// </summary>
		public ItemEvent OnItemDeselected = new ItemEvent();

		/// <summary>
		/// Newly selected indices.
		/// </summary>
		protected List<int> NewSelectedIndices = new List<int>();

		/// <summary>
		/// Selected items cache (to keep valid selected indices with updates).
		/// </summary>
		protected List<TItem> SelectedItemsCache = new List<TItem>();

		/// <summary>
		/// Items comparer.
		/// </summary>
		protected virtual IEqualityComparer<TItem> ItemsComparer => null;

		/// <inheritdoc/>
		protected override void InvokeSelect(int index, bool raiseEvents)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning(string.Format("Incorrect index: {0}", index.ToString()), this);
				return;
			}

			var item = DataSource[index];
			SelectedItemsCache.Add(item);

			base.InvokeSelect(index, raiseEvents);

			if (raiseEvents)
			{
				OnItemSelected.Invoke(index, item);
			}
		}

		/// <inheritdoc/>
		protected override void InvokeDeselect(int index, bool raiseEvents)
		{
			var valid = IsValid(index);
			var item = valid ? DataSource[index] : default;
			if (valid)
			{
				SelectedItemsCache.Remove(item);
			}

			base.InvokeDeselect(index, raiseEvents);

			if (raiseEvents)
			{
				OnItemDeselected.Invoke(index, item);
			}
		}

		/// <inheritdoc/>
		public override void RemoveItemAt(int index)
		{
			DataSource.RemoveAt(index);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(TItem item)
		{
			DataSource.Add(item);

			return DataSource.IndexOf(item);
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed TItem.</returns>
		public virtual int Remove(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index == -1)
			{
				return index;
			}

			DataSource.RemoveAt(index);

			return index;
		}

		/// <summary>
		/// Remove item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual void Remove(int index)
		{
			DataSource.RemoveAt(index);
		}

		/// <summary>
		/// Scrolls to specified item immediately.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ScrollTo(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index > -1)
			{
				ScrollTo(index);
			}
		}

		/// <summary>
		/// Scroll to the specified item with animation.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ScrollToAnimated(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index > -1)
			{
				ScrollToAnimated(index);
			}
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public int Set(TItem item, bool allowDuplicate = true)
		{
			int index;

			if (!allowDuplicate)
			{
				index = DataSource.IndexOf(item);
				if (index == -1)
				{
					index = Add(item);
				}
			}
			else
			{
				index = Add(item);
			}

			Select(index);

			return index;
		}

		/// <summary>
		/// Recalculates the selected indices.
		/// </summary>
		/// <param name="newItems">New items.</param>
		protected virtual void RecalculateSelectedIndices(ObservableList<TItem> newItems)
		{
			NewSelectedIndices.Clear();

			foreach (var item in SelectedItemsCache)
			{
				var new_index = newItems.IndexOf(item);
				if (new_index != -1)
				{
					NewSelectedIndices.Add(new_index);
				}
			}
		}

		/// <summary>
		/// Gets selected items.
		/// </summary>
		/// <param name="output">Selected items.</param>
		public void GetSelectedItems(List<TItem> output)
		{
			foreach (var index in selectedIndices)
			{
				output.Add(DataSource[index]);
			}
		}

		/// <summary>
		/// Sets selected items.
		/// </summary>
		/// <param name="selectedItems">Selected items.</param>
		/// <param name="deselectCurrent">Deselect currently selected items.</param>
		public void SetSelectedItems(List<TItem> selectedItems, bool deselectCurrent = false)
		{
			if (deselectCurrent)
			{
				DeselectAll();
			}

			foreach (var item in selectedItems)
			{
				Select(DataSource.IndexOf(item));
			}
		}

		/// <summary>
		/// Select first item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item was found and selected; otherwise false.</returns>
		public bool SelectFirstItem(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (IsValid(index))
			{
				Select(index);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Select all items.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item was found and selected; otherwise false.</returns>
		public bool SelectAllItems(TItem item)
		{
			var selected = false;
			var start = 0;
			bool is_valid;
			do
			{
				var index = DataSource.IndexOf(item, start);
				is_valid = IsValid(index);
				if (is_valid)
				{
					start = index + 1;
					Select(index);
					selected = true;
				}
			}
			while (is_valid);

			return selected;
		}
	}
}