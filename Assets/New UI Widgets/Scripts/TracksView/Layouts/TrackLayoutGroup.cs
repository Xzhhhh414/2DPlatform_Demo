namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	/// <summary>
	/// Layout with compact order and items at any line grouped by condition.
	/// </summary>
	/// <typeparam name="TData">Type of data.</typeparam>
	/// <typeparam name="TPoint">Type of point.</typeparam>
	public abstract class TrackLayoutGroup<TData, TPoint> : TrackLayoutAnyLineCompact<TData, TPoint>
		where TData : class, ITrackData<TPoint>, IObservable, INotifyPropertyChanged
		where TPoint : IComparable<TPoint>
	{
		readonly List<TData> resetFixedOrder = new List<TData>();

		/// <summary>
		/// Set order for the specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="temp">Temp list.</param>
		/// <param name="used">Temp list for the used items.</param>
		protected override void Layout(List<TData> items, List<TData> temp, List<TData> used)
		{
			EnsureSingleFixedOrder(items);

			// find items with FixedOrder
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				if (item.FixedOrder)
				{
					temp.Add(item);
				}
			}

			// set same order for the items with same name
			for (int temp_index = 0; temp_index < temp.Count; temp_index++)
			{
				var fixed_item = temp[temp_index];
				for (int item_index = 0; item_index < items.Count; item_index++)
				{
					var item = items[item_index];
					if ((!ReferenceEquals(item, fixed_item)) && SameGroup(item, fixed_item))
					{
						item.Order = fixed_item.Order;
						item.FixedOrder = true;
						resetFixedOrder.Add(item);
					}
				}
			}

			temp.Clear();

			var order = 0;
			temp.AddRange(items);
			while (temp.Count > 0)
			{
				var index = SetOrder(temp, used, order);
				if (index > -1)
				{
					used.Add(temp[index]);
					temp.RemoveAt(index);
				}
				else
				{
					used.Clear();
					order += 1;
				}
			}

			// reset fixedOrder
			for (int i = 0; i < resetFixedOrder.Count; i++)
			{
				resetFixedOrder[i].FixedOrder = false;
			}
		}

		/// <summary>
		/// Can be item placed along with the specified items.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="items">Items.</param>
		/// <returns>true if items can be places together; otherwise false.</returns>
		protected override bool CanBeWithItems(TData item, List<TData> items)
		{
			var valid_name = (items.Count == 0) || SameGroup(items[0], item);

			return valid_name && !IsIntersect(items, item.StartPoint, item.EndPoint);
		}

		/// <summary>
		/// Check if items belongs to the same group.
		/// </summary>
		/// <param name="x">First item.</param>
		/// <param name="y">Second item.</param>
		/// <returns>true if items belongs to the same group; otherwise false.</returns>
		protected abstract bool SameGroup(TData x, TData y);
	}
}