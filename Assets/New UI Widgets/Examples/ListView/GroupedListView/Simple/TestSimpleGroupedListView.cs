namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Test SimpleGroupedListView.
	/// </summary>
	public class TestSimpleGroupedListView : MonoBehaviour
	{
		/// <summary>
		/// File with test data.
		/// </summary>
		[SerializeField]
		protected TextAsset sourceFile;

		/// <summary>
		/// GroupedListView
		/// </summary>
		[SerializeField]
		protected SimpleGroupedListView GroupedListView;

		/// <summary>
		/// Load data.
		/// </summary>
		protected virtual void Start()
		{
			LoadData();
		}

		/// <summary>
		/// Load data.
		/// </summary>
		protected virtual void LoadData()
		{
			GroupedListView.Init();

			using var _ = GroupedListView.GroupedData.BeginUpdate();

			var lines = sourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			foreach (var line in lines)
			{
				var l = line.Trim();
				if (string.IsNullOrEmpty(l) || l[0] == '#')
				{
					continue;
				}

				GroupedListView.GroupedData.Add(new SimpleGroupedItem() { Name = l });
			}
		}

		/// <summary>
		/// Change the data.
		/// </summary>
		public void ChangeData()
		{
			// create new group
			var list = new GroupedList<SimpleGroupedItem>(GroupedListView.GroupedData.Item2Group)
			{
				// set sorting function (optional)
				GroupComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name),

				// create new data list
				Output = new ObservableList<SimpleGroupedItem>(),
			};

			// add items
			list.Add(new SimpleGroupedItem() { Name = "Item 1" });
			list.Add(new SimpleGroupedItem() { Name = "Item 2" });

			list.Add(new SimpleGroupedItem() { Name = "A 1" });
			list.Add(new SimpleGroupedItem() { Name = "A 2" });

			// attach new grouped items to ListView
			GroupedListView.GroupedData = list;
			GroupedListView.DataSource = list.Output;
		}

		/// <summary>
		/// Remove item with specified index if item is group will be deleted first item in group.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Remove(int index)
		{
			var item = GroupedListView.DataSource[index];

			// if is group
			if (item.IsGroup)
			{
				Remove(index + 1);
			}
			else
			{
				GroupedListView.GroupedData.Remove(item);
			}
		}
	}
}