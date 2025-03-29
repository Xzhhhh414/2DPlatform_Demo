namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// DialogTreeViewInputHelper
	/// </summary>
	public class DialogTreeViewInputHelper : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView Folders;

		ObservableList<TreeNode<TreeViewItem>> nodes;

		/// <summary>
		/// Init.
		/// </summary>
		public void Refresh()
		{
			if (nodes != null)
			{
				return;
			}

			nodes = TestTreeView.GenerateTreeNodes(new int[] { 5, 5, 2 }, isExpanded: true);

			// Set nodes
			Folders.Init();
			Folders.Nodes = nodes;
		}
	}
}