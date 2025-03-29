namespace UIWidgets
{
	/// <summary>
	/// TreeNode used in JSON serialization.
	/// </summary>
	/// <example>
	/// // serialize
	/// var nodes = TreeNodeJson&lt;TreeViewItem&gt;.ConvertNodes(TreeView.Nodes);
	/// var json = JsonConvert.SerializeObject(nodes);
	/// // deserialize
	/// var decoded = JsonConvert.DeserializeObject&lt;TreeNodeJson&lt;TreeViewItem&gt;[]&gt;(json);
	/// TreeView.Nodes = TreeNodeJson&lt;TreeViewItem&gt;.ConvertNodes(decoded);
	/// </example>
	/// <typeparam name="TItem">Item type.</typeparam>
	public class TreeNodeJson<TItem>
	{
		/// <summary>
		/// Item.
		/// </summary>
		public TItem Item;

		/// <summary>
		/// Nodes.
		/// </summary>
		public TreeNodeJson<TItem>[] Nodes;

		/// <summary>
		/// Is node expanded.
		/// </summary>
		public bool IsExpanded;

		/// <summary>
		/// Is node visible.
		/// </summary>
		public bool IsVisible = true;

		/// <summary>
		/// Convert node to serializable type.
		/// </summary>
		/// <param name="node">Node.</param>
		public static implicit operator TreeNode<TItem>(TreeNodeJson<TItem> node)
		{
			return new TreeNode<TItem>(node.Item, ConvertNodes(node.Nodes), node.IsExpanded, node.IsVisible);
		}

		/// <summary>
		/// Convert node from serializable type.
		/// </summary>
		/// <param name="node">Node.</param>
		public static implicit operator TreeNodeJson<TItem>(TreeNode<TItem> node)
		{
			return new TreeNodeJson<TItem>()
			{
				Item = node.Item,
				Nodes = ConvertNodes(node.Nodes),
				IsExpanded = node.IsExpanded,
				IsVisible = node.IsVisible,
			};
		}

		/// <summary>
		/// Convert nodes from serializable type.
		/// </summary>
		/// <param name="nodes">Serializable nodes.</param>
		/// <returns>Nodes.</returns>
		public static ObservableList<TreeNode<TItem>> ConvertNodes(TreeNodeJson<TItem>[] nodes)
		{
			if (nodes == null)
			{
				return null;
			}

			var result = new ObservableList<TreeNode<TItem>>(nodes.Length);
			foreach (var node in nodes)
			{
				result.Add(node);
			}

			return result;
		}

		/// <summary>
		/// Convert nodes to serializable type.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <returns>Serializable nodes.</returns>
		public static TreeNodeJson<TItem>[] ConvertNodes(ObservableList<TreeNode<TItem>> nodes)
		{
			if (nodes == null)
			{
				return null;
			}

			var result = new TreeNodeJson<TItem>[nodes.Count];
			for (int i = 0; i < nodes.Count; i++)
			{
				result[i] = nodes[i];
			}

			return result;
		}
	}
}