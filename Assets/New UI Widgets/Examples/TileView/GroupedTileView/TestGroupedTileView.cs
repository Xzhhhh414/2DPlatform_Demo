namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Test GroupedTileView.
	/// </summary>
	public class TestGroupedTileView : MonoBehaviour
	{
		/// <summary>
		/// File with test data.
		/// </summary>
		[SerializeField]
		protected Sprite TestPhoto;

		/// <summary>
		/// GroupedListView
		/// </summary>
		[SerializeField]
		protected GroupedTileViewBase GroupedTileView;

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
			GroupedTileView.GroupedData.Output = GroupedTileView.DataSource;

			using var _ = GroupedTileView.GroupedData.BeginUpdate();

			AddItems(DateTime.Now, 5, TestPhoto);
			AddItems(new DateTime(2019, 01, 05, 12, 14, 55), 7, TestPhoto);
			AddItems(new DateTime(2018, 01, 05, 12, 14, 55), 1, TestPhoto);
			AddItems(new DateTime(2019, 04, 05, 12, 14, 55), 4, TestPhoto);
		}

		void AddItems(DateTime dt, int count, Sprite image)
		{
			for (int i = 0; i < count; i++)
			{
				GroupedTileView.GroupedData.Add(new Photo() { Created = dt, Image = image });
			}
		}

		/// <summary>
		/// Change the data.
		/// </summary>
		public void ChangeData()
		{
			// create new group
			var list = new GroupedList<Photo>(GroupedTileView.GroupedData.Item2Group)
			{
				// set sorting function (optional)
				GroupComparison = (x, y) => x.Created.CompareTo(y.Created),

				// create new data list
				Output = new ObservableList<Photo>(),
			};

			// add items
			AddItems(DateTime.Now, 5, TestPhoto);
			AddItems(new DateTime(2019, 01, 05, 12, 14, 55), 7, TestPhoto);

			// attach new grouped items to ListView
			GroupedTileView.GroupedData = list;
			GroupedTileView.DataSource = list.Output;
		}

		/// <summary>
		/// Remove item with specified index if item is group will be deleted first item in group.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Remove(int index)
		{
			var item = GroupedTileView.DataSource[index];

			// if is group
			if (item.IsGroup)
			{
				Remove(index + 1);
			}
			else
			{
				GroupedTileView.GroupedData.Remove(item);
			}
		}
	}
}