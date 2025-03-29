namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Pool;

	/// <summary>
	/// Base class for the track layouts.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	public class TrackLayout<TData, TPoint>
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Set order for the specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="temp">Temp list.</param>
		/// <param name="used">Temp list for the used items.</param>
		protected virtual void Layout(List<TData> items, List<TData> temp, List<TData> used)
		{
		}

		/// <summary>
		/// Set order for the specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		public void Set(List<TData> items)
		{
			if (items.Count == 0)
			{
				return;
			}

			using var _ = ListPool<TData>.Get(out var temp_items);
			using var __ = ListPool<TData>.Get(out var temp_used_items);

			Layout(items, temp_items, temp_used_items);
		}

		/// <summary>
		/// Make sure that only one item with same order has enabled FixedOrder.
		/// </summary>
		/// <param name="items">Items.</param>
		protected void EnsureSingleFixedOrder(List<TData> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				if (!item.FixedOrder)
				{
					continue;
				}

				for (int j = i + 1; j < items.Count; j++)
				{
					var next_item = items[j];
					if (next_item.Order > item.Order)
					{
						break;
					}

					if (next_item.FixedOrder)
					{
						next_item.FixedOrder = false;
					}
				}
			}
		}

		/// <summary>
		/// Can be item placed along with the specified items.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="items">Items.</param>
		/// <returns>true if items can be places together; otherwise false.</returns>
		protected virtual bool CanBeWithItems(TData item, List<TData> items)
		{
			return !IsIntersect(items, item.StartPoint, item.EndPoint);
		}

		/// <summary>
		/// Is any item in the specified list intersect points?
		/// </summary>
		/// <param name="list">Items.</param>
		/// <param name="start">Start point.</param>
		/// <param name="end">End point.</param>
		/// <returns>true if data located between specified points; otherwise false.</returns>
		protected bool IsIntersect(List<TData> list, TPoint start, TPoint end)
		{
			if (list.Count == 0)
			{
				return false;
			}

			for (int i = 0; i < list.Count; i++)
			{
				var item = list[i];

				// item.Start in [start..end)
				var start_intersect = (item.StartPoint.CompareTo(start) >= 0)
					&& (item.StartPoint.CompareTo(end) < 0);

				// item.End in [start..end)
				var end_intersect = (item.EndPoint.CompareTo(start) > 0)
					&& (item.EndPoint.CompareTo(end) <= 0);

				// item.Start <= start and Item.end >= end
				var full_intersect = (item.StartPoint.CompareTo(start) <= 0)
					&& (item.EndPoint.CompareTo(end) > 0);

				if (start_intersect || end_intersect || full_intersect)
				{
					return true;
				}
			}

			return false;
		}
	}
}