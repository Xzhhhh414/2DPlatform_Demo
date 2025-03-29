namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Pool;

	/// <summary>
	/// Base class for GroupedList.
	/// Group items (group by GetGroup() method) then convert to flat list.
	/// After each group item inserted EmptyGroupItem (total ItemsPerBlock - 1).
	/// After last item in each group inserted EmptyItem to fill last block to ItemsPerBlock count.
	/// </summary>
	/// <typeparam name="T">Items type.</typeparam>
	public class GroupedList<T> : IEnumerable<T>, IEnumerable, IListUpdatable
	{
		/// <summary>
		/// Get group for the specified item.
		/// </summary>
		/// <param name="groups">Existing group.</param>
		/// <param name="item">Item.</param>
		/// <returns>Group.</returns>
		public delegate T GetGroupByItem(IReadOnlyCollection<T> groups, T item);

		/// <summary>
		/// Contains groups and items for each group.
		/// Group as key. Items for group as value.
		/// </summary>
		protected Dictionary<T, List<T>> GroupsWithItems = new Dictionary<T, List<T>>();

		/// <summary>
		/// Group comparison.
		/// </summary>
		public Comparison<T> GroupComparison;

		/// <summary>
		/// Items comparison.
		/// </summary>
		public Comparison<T> ItemsComparison;

		/// <summary>
		/// Output list.
		/// </summary>
		[Obsolete("Renamed to Output.")]
		public ObservableList<T> Data => Output;

		/// <summary>
		/// Output list.
		/// </summary>
		public ObservableList<T> Output;

		/// <summary>
		/// Groups.
		/// </summary>
		public Dictionary<T, List<T>>.KeyCollection Groups => GroupsWithItems.Keys;

		/// <summary>
		/// Items.
		/// </summary>
		public Dictionary<T, List<T>>.ValueCollection Items => GroupsWithItems.Values;

		/// <summary>
		/// Count.
		/// </summary>
		public int Count => GroupsWithItems.Count;

		/// <summary>
		/// Get group for the specified item.
		/// </summary>
		public readonly GetGroupByItem Item2Group;

		int itemsPerBlock = 1;

		/// <summary>
		/// Items per block (row or column).
		/// </summary>
		public int ItemsPerBlock
		{
			get => itemsPerBlock;

			set
			{
				if (itemsPerBlock != value)
				{
					itemsPerBlock = value;
					Update();
				}
			}
		}

		T emptyGroupItem;

		/// <summary>
		/// Empty item to fill group row.
		/// </summary>
		public T EmptyGroupItem
		{
			get => emptyGroupItem;

			set
			{
				if (!IsItemsEquals(emptyGroupItem, value))
				{
					emptyGroupItem = value;
					Update();
				}
			}
		}

		T emptyItem;

		/// <summary>
		/// Empty item to fill the last items block.
		/// </summary>
		public T EmptyItem
		{
			get => emptyItem;

			set
			{
				if (!IsItemsEquals(emptyItem, value))
				{
					emptyItem = value;
					Update();
				}
			}
		}

		static bool IsItemsEquals(T a, T b) => EqualityComparer<T>.Default.Equals(a, b);

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupedList{T}"/> class.
		/// </summary>
		public GroupedList()
		{
			Item2Group = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupedList{T}"/> class.
		/// </summary>
		/// <param name="item2group">Delegate to get group for the specified item.</param>
		/// <exception cref="ArgumentNullException">item2group is null.</exception>
		public GroupedList(GetGroupByItem item2group)
		{
			Item2Group = item2group ?? throw new ArgumentNullException(nameof(item2group));
		}

		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected virtual T GetGroup(T item) => Item2Group(GroupsWithItems.Keys, item);

		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		public virtual T GetGroupOf(T item) => GetGroup(item);

		/// <summary>
		/// Add item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="update">Update output.</param>
		/// <param name="sort">Sort items.</param>
		protected virtual void Add(T item, bool update, bool sort = true)
		{
			var group = GetGroup(item);
			if (!GroupsWithItems.TryGetValue(group, out var items))
			{
				items = new List<T>();
				GroupsWithItems.Add(group, items);
			}

			items.Add(item);

			if (sort && (ItemsComparison != null))
			{
				items.Sort(ItemsComparison);
			}

			if (update)
			{
				Update();
			}
		}

		/// <summary>
		/// Add item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void Add(T item) => Add(item, true);

		/// <summary>
		/// Add items.
		/// </summary>
		/// <param name="items">Items.</param>
		public virtual void AddRange(IList<T> items)
		{
			for (var i = 0; i < items.Count; i++)
			{
				Add(items[i], false);
			}

			Update();
		}

		/// <summary>
		/// Remove item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item exists and deleted; otherwise, false.</returns>
		public virtual bool Remove(T item)
		{
			var group = GetGroup(item);
			if (GroupsWithItems.TryGetValue(group, out var group_items))
			{
				if (group_items.Remove(item))
				{
					if (group_items.Count == 0)
					{
						GroupsWithItems.Remove(group);
					}

					Update();

					return true;
				}

				return false;
			}

			foreach (var kv in GroupsWithItems)
			{
				if (kv.Value.Remove(item))
				{
					Update();

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Remove all items that match predicate.
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		/// <returns>Count of removed items.</returns>
		public virtual int Remove(Predicate<T> predicate)
		{
			var removed = 0;

			foreach (var items in GroupsWithItems.Values)
			{
				removed += items.RemoveAll(predicate);
			}

			if (removed > 0)
			{
				Update();
			}

			return removed;
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public virtual void Clear()
		{
			GroupsWithItems.Clear();
			Update();
		}

		/// <summary>
		/// Get items of group.
		/// </summary>
		/// <param name="group">Group.</param>
		/// <returns>Items.</returns>
		public virtual IReadOnlyList<T> ItemsOfGroup(T group)
		{
			if (!GroupsWithItems.TryGetValue(group, out var items))
			{
				items = null;
			}

			return items;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the groups.
		/// </summary>
		/// <returns>A enumerator for the groups.</returns>
		public Dictionary<T, List<T>>.KeyCollection.Enumerator GetEnumerator() => GroupsWithItems.Keys.GetEnumerator();

		/// <summary>
		/// Returns an enumerator that iterates through the groups.
		/// </summary>
		/// <returns>A enumerator for the groups.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GroupsWithItems.Keys.GetEnumerator();

		/// <summary>
		/// Returns an enumerator that iterates through the groups.
		/// </summary>
		/// <returns>A enumerator for the groups.</returns>
		IEnumerator IEnumerable.GetEnumerator() => GroupsWithItems.Keys.GetEnumerator();

		/// <summary>
		/// Contains group.
		/// </summary>
		/// <param name="group">Group.</param>
		/// <returns>true if contains group; otherwise false.</returns>
		public virtual bool ContainsGroup(T group) => GroupsWithItems.ContainsKey(group);

		/// <summary>
		/// Remove group with all items.
		/// </summary>
		/// <param name="group">Group.</param>
		/// <returns>true if group was removed; otherwise false.</returns>
		public virtual bool RemoveGroup(T group)
		{
			var result = GroupsWithItems.Remove(group);

			Update();

			return result;
		}

		/// <summary>
		/// Update data list.
		/// </summary>
		public virtual void Update()
		{
			if (inUpdate)
			{
				isChanged = true;
				return;
			}

			if (Output == null)
			{
				return;
			}

			using var _ = Output.BeginUpdate();

			Output.Clear();

			InsertGroups();
		}

		/// <summary>
		/// Insert groups with items to the Data list.
		/// </summary>
		protected virtual void InsertGroups()
		{
			using var _ = ListPool<T>.Get(out var groups);
			groups.AddRange(GroupsWithItems.Keys);

			if (GroupComparison != null)
			{
				groups.Sort(GroupComparison);
			}

			foreach (var group in groups)
			{
				var items = GroupsWithItems[group];
				if (items.Count == 0)
				{
					continue;
				}

				Output.Add(group);

				for (var j = 1; j < ItemsPerBlock; j++)
				{
					Output.Add(EmptyGroupItem);
				}

				Output.AddRange(items);

				if (ItemsPerBlock > 0)
				{
					var n = items.Count % ItemsPerBlock;
					if (n > 0)
					{
						for (var j = n; j < ItemsPerBlock; j++)
						{
							Output.Add(EmptyItem);
						}
					}
				}
			}
		}

		bool inUpdate;
		bool isChanged;

		/// <summary>
		/// Pause data list update.
		/// </summary>
		/// <returns>Updater.</returns>
		public virtual ListUpdater BeginUpdate()
		{
			inUpdate = true;
			return new ListUpdater(this);
		}

		/// <summary>
		/// Ends data list update.
		/// </summary>
		public virtual void EndUpdate()
		{
			inUpdate = false;
			if (isChanged)
			{
				isChanged = false;
				Update();
			}
		}
	}
}