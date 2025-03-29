namespace UIWidgets
{
	/// <summary>
	/// Tree view component base.
	/// </summary>
	/// <typeparam name="TItem">Type of node item.</typeparam>
	public class TreeViewComponentBase<TItem> : TreeViewComponentBase, IViewData<ListNode<TItem>>
	{
		/// <summary>
		/// The node.
		/// </summary>
		public TreeNode<TItem> Node
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		protected override bool IsExpanded => Node != null && Node.IsExpanded;

		/// <inheritdoc/>
		public override void RemoveItem()
		{
			if (Owner != null)
			{
				Node.Parent = null;
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListNode<TItem> item)
		{
			SetData(item.Node, item.Depth);
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public virtual void SetData(TreeNode<TItem> node, int depth)
		{
			NodeDepth = depth;

			if (node != null)
			{
				Node = node;
				SetToggle(Node.IsExpanded);
			}

			if (Indentation != null)
			{
				Indentation.minWidth = NodeDepth * PaddingPerLevel;
				Indentation.preferredWidth = NodeDepth * PaddingPerLevel;
				Indentation.flexibleWidth = 0;
			}

			if ((Toggle != null) && (Toggle.gameObject != null))
			{
				var toggle_active = (node != null) && (node.Nodes != null) && (node.Nodes.Count > 0);
				if (Toggle.gameObject.activeSelf != toggle_active)
				{
					Toggle.gameObject.SetActive(toggle_active);
				}
			}
		}
	}
}