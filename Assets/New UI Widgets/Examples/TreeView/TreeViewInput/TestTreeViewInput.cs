namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test TreeViewInput.
	/// </summary>
	public class TestTreeViewInput : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView TreeView;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			TreeView.Nodes = GetNodes();
		}

		ObservableList<TreeNode<TreeViewItem>> GetNodes()
		{
			var result = new ObservableList<TreeNode<TreeViewItem>>();
			var data = new TreeNode<TreeViewItem>(new TreeViewItem("Data"))
			{
				IsExpanded = true,
				Nodes = new ObservableList<TreeNode<TreeViewItem>>()
				{
					new TreeNode<TreeViewItem>(new TreeViewItem("Archivable") { Tag = false, }),
					new TreeNode<TreeViewItem>(new TreeViewItem("AccountId") { Tag = 123, }),
					new TreeNode<TreeViewItem>(new TreeViewItem("ClassName") { Tag = "Player", }),
					new TreeNode<TreeViewItem>(new TreeViewItem("Vector3") { Tag = new Vector3(20f, 32f, 7f), }),
					new TreeNode<TreeViewItem>(new TreeViewItem("Color") { Tag = Color.green, }),
				},
			};

			result.Add(data);

			var behavior = new TreeNode<TreeViewItem>(new TreeViewItem("Behavior"))
			{
				IsExpanded = true,
				Nodes = new ObservableList<TreeNode<TreeViewItem>>()
				{
					new TreeNode<TreeViewItem>(new TreeViewItem("Option 1") { Tag = false, }),
					new TreeNode<TreeViewItem>(new TreeViewItem("Option 2") { Tag = true, }),
				},
			};

			result.Add(behavior);

			return result;
		}
	}
}