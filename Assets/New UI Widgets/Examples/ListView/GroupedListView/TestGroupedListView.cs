namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Test GroupedListView.
	/// </summary>
	public class TestGroupedListView : MonoBehaviour
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
		protected GroupedListView GroupedListView;

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
			GroupedListView.GroupedData.Output = GroupedListView.DataSource;

			using var _ = GroupedListView.GroupedData.BeginUpdate();

			var lines = sourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			foreach (var line in lines)
			{
				var l = line.Trim();
				if (string.IsNullOrEmpty(l) || l[0] == '#')
				{
					continue;
				}

				GroupedListView.GroupedData.Add(new GroupedListItem() { Name = l });
			}
		}

		/// <summary>
		/// Change the data.
		/// </summary>
		public void ChangeData()
		{
			// create new group
			var list = new GroupedList<IGroupedListItem>(GroupedListView.GroupedData.Item2Group)
			{
				// set sorting function (optional)
				GroupComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name),

				// create new data list
				Output = new ObservableList<IGroupedListItem>(),
			};

			// add items
			list.Add(new GroupedListItem() { Name = "Item 1" });
			list.Add(new GroupedListItem() { Name = "Item 2" });

			list.Add(new GroupedListItem() { Name = "A 1" });
			list.Add(new GroupedListItem() { Name = "A 2" });

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
			if (item.Name.Length == 1)
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