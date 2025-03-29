namespace UIWidgets.Examples
{
	using UIWidgets;
	using UIWidgets.Extensions;
	using UnityEngine;

	/// <summary>
	/// Test nodes filter.
	/// </summary>
	public class TestNodesFilter : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView TreeView;

		/// <summary>
		/// InputField.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter InputField;

		/// <summary>
		/// NewItem InputField.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter NewItemInputField;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			InputField.onValueChanged.AddListener(UpdateNodes);
			UpdateNodes();
		}

		bool Predicate(TreeNode<TreeViewItem> node) => UtilitiesCompare.Contains(node.Item.Name, InputField.Value, false);

		void UpdateNodes(string ignore = null) => TreeView.Nodes.Filter(Predicate);

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			if (InputField != null)
			{
				InputField.onValueChanged.RemoveListener(UpdateNodes);
			}
		}

		/// <summary>
		/// Add new item
		/// </summary>
		public void Add()
		{
			var name = string.Format("Node {0}", TreeView.Nodes.Count.ToString());
			TreeView.Nodes.Add(new TreeNode<TreeViewItem>(new TreeViewItem(name)));
			UpdateNodes();
		}

		/// <summary>
		/// Add new item
		/// </summary>
		public void AddFromInput()
		{
			if (string.IsNullOrEmpty(NewItemInputField.Value))
			{
				return;
			}

			TreeView.Nodes.Add(new TreeNode<TreeViewItem>(new TreeViewItem(NewItemInputField.Value)));
			NewItemInputField.Value = string.Empty;
			UpdateNodes();
		}
	}
}