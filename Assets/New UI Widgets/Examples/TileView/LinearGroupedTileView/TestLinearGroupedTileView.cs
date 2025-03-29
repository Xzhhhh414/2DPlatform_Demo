namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Test LinearGroupedTileView.
	/// </summary>
	public class TestLinearGroupedTileView : MonoBehaviour
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
		protected LinearGroupedTileView GroupedTileView;

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
			GroupedTileView.Init();

			using var _ = GroupedTileView.RealDataSource.BeginUpdate();

			AddItems(DateTime.Now, 5, TestPhoto);
			AddItems(new DateTime(2019, 01, 05, 12, 14, 55), 7, TestPhoto);
			AddItems(new DateTime(2018, 01, 05, 12, 14, 55), 1, TestPhoto);
			AddItems(new DateTime(2019, 04, 05, 12, 14, 55), 4, TestPhoto);
		}

		void AddItems(DateTime dt, int count, Sprite image)
		{
			GroupedTileView.RealDataSource.Add(new Photo() { IsGroup = true, Created = dt });

			for (int i = 0; i < count; i++)
			{
				GroupedTileView.RealDataSource.Add(new Photo() { Created = dt, Image = image });
			}
		}
	}
}