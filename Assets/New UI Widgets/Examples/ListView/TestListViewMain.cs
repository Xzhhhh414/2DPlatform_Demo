namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test ListViewMain.
	/// </summary>
	public class TestListViewMain : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public FileListView ListView;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			#if UNITY_EDITOR
			ListView.CurrentDirectory = System.IO.Path.Combine(Application.dataPath, "New UI Widgets", "Scripts");
			#endif
		}
	}
}