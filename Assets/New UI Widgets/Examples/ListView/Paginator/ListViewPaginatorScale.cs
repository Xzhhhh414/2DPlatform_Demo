namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Helper for ListViewPaginator to scale centered slide.
	/// </summary>
	public class ListViewPaginatorScale : BasePaginatorScale<ListViewItem>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		protected ListViewBase ListView;

		/// <inheritdoc/>
		protected override ListViewItem GetInstance(int index)
		{
			return ListView.GetInstance(index);
		}
	}
}