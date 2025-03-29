namespace UIWidgets.Internal
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UnityEngine;

	/// <summary>
	/// Default container to save instance sizes of items for TreeView.
	/// </summary>
	/// <typeparam name="TItem">Item type.</typeparam>
	public class NodeInstanceSizes<TItem> : IInstanceSizes<ListNode<TItem>>
	{
		readonly Dictionary<TreeNode<TItem>, Vector2> sizes;

		readonly Dictionary<TreeNode<TItem>, Vector2> overriddenSizes;

		readonly Dictionary<TreeNode<TItem>, bool> keep;

		/// <summary>
		/// Initializes a new instance of the <see cref="NodeInstanceSizes{TItem}"/> class.
		/// </summary>
		/// <param name="comparer">Items comparer.</param>
		public NodeInstanceSizes(IEqualityComparer<TreeNode<TItem>> comparer = null)
		{
			if (comparer == null)
			{
				sizes = new Dictionary<TreeNode<TItem>, Vector2>();
				overriddenSizes = new Dictionary<TreeNode<TItem>, Vector2>();
				keep = new Dictionary<TreeNode<TItem>, bool>();
			}
			else
			{
				sizes = new Dictionary<TreeNode<TItem>, Vector2>(comparer);
				overriddenSizes = new Dictionary<TreeNode<TItem>, Vector2>(comparer);
				keep = new Dictionary<TreeNode<TItem>, bool>(comparer);
			}
		}

		/// <summary>
		/// Count.
		/// </summary>
		public int Count => sizes.Count;

		/// <summary>
		/// Get and set instance size.
		/// </summary>
		/// <param name="key">Item.</param>
		/// <returns>Size.</returns>
		public Vector2 this[ListNode<TItem> key]
		{
			get => sizes[key.Node];

			set => sizes[key.Node] = value;
		}

		/// <summary>
		/// Check if container has size of the specified item.
		/// </summary>
		/// <param name="key">Item.</param>
		/// <returns>true if container has size of the specified item; otherwise false.</returns>
		public bool Contains(ListNode<TItem> key) => sizes.ContainsKey(key.Node);

		/// <summary>
		/// Try to get size of the specified item.
		/// </summary>
		/// <param name="key">Item.</param>
		/// <param name="size">Instance size.</param>
		/// <returns>true if container has size of the specified item; otherwise false.</returns>
		public bool TryGet(ListNode<TItem> key, out Vector2 size) => sizes.TryGetValue(key.Node, out size);

		/// <summary>
		/// Remove size of the specified item.
		/// </summary>
		/// <param name="key">Item.</param>
		/// <returns>true if container has size; otherwise false.</returns>
		public bool Remove(ListNode<TItem> key) => sizes.Remove(key.Node);

		/// <summary>
		/// Remove sizes of items that are not in the specified list.
		/// </summary>
		/// <param name="items">Items.</param>
		public void RemoveNotExisting(ObservableList<ListNode<TItem>> items)
		{
			// should not delete because collapsed nodes are not present in list
			// RemoveNotExisting(sizes, items);
			// RemoveNotExisting(overriddenSizes, items);
		}

		/// <summary>
		/// Remove sizes of items that are not in the specified list.
		/// </summary>
		/// <param name="isExisting">Function to check is item exists.</param>
		public void RemoveNotExisting(Func<TreeNode<TItem>, bool> isExisting)
		{
			RemoveNotExisting(sizes, isExisting);
			RemoveNotExisting(overriddenSizes, isExisting);
		}

		void RemoveNotExisting(Dictionary<TreeNode<TItem>, Vector2> currentSizes, Func<TreeNode<TItem>, bool> isExisting)
		{
			using var _ = ListPool<TreeNode<TItem>>.Get(out var remove);
			foreach (var node in currentSizes)
			{
				if (!isExisting(node.Key))
				{
					remove.Add(node.Key);
				}
			}

			foreach (var item in remove)
			{
				currentSizes.Remove(item);
			}

			keep.Clear();
		}

		/// <summary>
		/// Remove unexisting nodes.
		/// </summary>
		/// <param name="currentSizes">Current sizes.</param>
		/// <param name="items">Items.</param>
		protected void RemoveUnexisting(Dictionary<TreeNode<TItem>, Vector2> currentSizes, ObservableList<ListNode<TItem>> items)
		{
			foreach (var item in items)
			{
				if (currentSizes.ContainsKey(item.Node))
				{
					keep[item.Node] = true;
				}
			}

			using var _ = ListPool<TreeNode<TItem>>.Get(out var remove);

			foreach (var item in currentSizes)
			{
				if (!keep.ContainsKey(item.Key))
				{
					remove.Add(item.Key);
				}
			}

			foreach (var item in remove)
			{
				currentSizes.Remove(item);
			}

			keep.Clear();
		}

		/// <summary>
		/// Get items count that fits into specified area.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="horizontal">If true do calculation using items width; otherwise do calculation using items height.</param>
		/// <param name="visibleArea">Size of the visible area.</param>
		/// <param name="spacing">Spacing between items.</param>
		/// <returns>Maximum items count.</returns>
		public int Visible(ObservableList<ListNode<TItem>> items, bool horizontal, float visibleArea, float spacing)
		{
			using var _ = ListPool<float>.Get(out var sorted_sizes);

			foreach (var item in items)
			{
				if (!overriddenSizes.TryGetValue(item.Node, out var size))
				{
					size = sizes[item.Node];
				}

				sorted_sizes.Add(horizontal ? size.x : size.y);
			}

			sorted_sizes.Sort();
			var max = 0;
			foreach (var s in sorted_sizes)
			{
				visibleArea -= s;

				if (visibleArea <= 0f)
				{
					break;
				}

				max += 1;
				visibleArea -= spacing;
			}

			return Mathf.Max(1, max);
		}

		/// <summary>
		/// Check if has overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if has size of the specified item; otherwise false.</returns>
		public bool HasOverridden(ListNode<TItem> item)
		{
			return overriddenSizes.ContainsKey(item.Node);
		}

		/// <summary>
		/// Set overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="size">Size.</param>
		public void SetOverridden(ListNode<TItem> item, Vector2 size)
		{
			SetOverridden(item.Node, size);
		}

		/// <summary>
		/// Set overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="size">Size.</param>
		public void SetOverridden(TreeNode<TItem> item, Vector2 size)
		{
			overriddenSizes[item] = size;
		}

		/// <summary>
		/// Remove size of the specified item.
		/// </summary>
		/// <param name="key">Item.</param>
		/// <returns>true if container has size; otherwise false.</returns>
		public bool RemoveOverridden(TreeNode<TItem> key)
		{
			return overriddenSizes.Remove(key);
		}

		/// <summary>
		/// Remove overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if container has overridden size; otherwise false.</returns>
		public bool RemoveOverridden(ListNode<TItem> item)
		{
			return overriddenSizes.Remove(item.Node);
		}

		/// <summary>
		/// Try to get overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="size">Instance size.</param>
		/// <returns>true if container has overridden size of the specified item; otherwise false.</returns>
		public bool TryGetOverridden(ListNode<TItem> item, out Vector2 size)
		{
			return TryGetOverridden(item.Node, out size);
		}

		/// <summary>
		/// Try to get overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="size">Instance size.</param>
		/// <returns>true if container has overridden size of the specified item; otherwise false.</returns>
		public bool TryGetOverridden(TreeNode<TItem> item, out Vector2 size)
		{
			return overriddenSizes.TryGetValue(item, out size);
		}

		/// <summary>
		/// Get actual item size.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="defaultSize">Default size if any other size not specified.</param>
		/// <returns>Size.</returns>
		public Vector2 Get(ListNode<TItem> item, Vector2 defaultSize)
		{
			if (overriddenSizes.TryGetValue(item.Node, out var result))
			{
				return result;
			}

			if (sizes.TryGetValue(item.Node, out result))
			{
				return result;
			}

			return defaultSize;
		}

		/// <summary>
		/// Maximum size for each dimenstion.
		/// </summary>
		/// <param name="defaultSize">Default size.</param>
		/// <returns>Size.</returns>
		public Vector2 MaxSize(Vector2 defaultSize)
		{
			var result = defaultSize;
			foreach (var info in sizes)
			{
				if (info.Key.Index == -1)
				{
					continue;
				}

				if (!overriddenSizes.TryGetValue(info.Key, out var size))
				{
					size = info.Value;
				}

				result.x = Mathf.Max(result.x, size.x);
				result.y = Mathf.Max(result.y, size.y);
			}

			return result;
		}
	}
}