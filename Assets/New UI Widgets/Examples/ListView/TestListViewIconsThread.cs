namespace UIWidgets.Examples
{
	using System.Threading.Tasks;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test ListViewIcons with threads.
	/// </summary>
	public class TestListViewIconsThread : MonoBehaviour
	{
		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		protected ListViewIcons ListView;

		/// <summary>
		/// Test adding items from other thread.
		/// </summary>
		public void TestAdd()
		{
			Task.Run(() => AddInForeground());
		}

		/// <summary>
		/// Test setting new items list from other thread.
		/// </summary>
		public void TestSet()
		{
			Task.Run(() => SetInForeground());
		}

		/// <summary>
		/// Select and scroll to specified index.
		/// </summary>
		/// <param name="i">Index.</param>
		public void Scroll(int i)
		{
			ListView.Select(i);
			ListView.ScrollTo(i);
		}

		void AddInForeground()
		{
			var items = ListView.DataSource;

			using var _ = items.BeginUpdate();

			items.Add(new ListViewIconsItemDescription() { Name = "Added from thread 1" });
			items.Add(new ListViewIconsItemDescription() { Name = "Added from thread 2" });
			items.Add(new ListViewIconsItemDescription() { Name = "Added from thread 3" });
		}

		void SetInForeground()
		{
			var items = new ObservableList<ListViewIconsItemDescription>
			{
				new ListViewIconsItemDescription() { Name = "Added from thread 1" },
				new ListViewIconsItemDescription() { Name = "Added from thread 2" },
				new ListViewIconsItemDescription() { Name = "Added from thread 3" },
				new ListViewIconsItemDescription() { Name = "Added from thread 4" },
				new ListViewIconsItemDescription() { Name = "Added from thread 5" },
				new ListViewIconsItemDescription() { Name = "Added from thread 6" },
			};

			ListView.DataSource = items;
		}
	}
}