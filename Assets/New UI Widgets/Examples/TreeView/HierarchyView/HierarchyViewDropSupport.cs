namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// HierarchyView drop support.
	/// </summary>
	[RequireComponent(typeof(HierarchyView))]
	public partial class HierarchyViewDropSupport : TreeViewCustomDropSupport<HierarchyView, HierarchyItemView, GameObject>
	{
		/// <inheritdoc/>
		public override void Drop(TreeNode<GameObject> data, PointerEventData eventData)
		{
			if (Source.Nodes == null)
			{
				Source.Nodes = new ObservableList<TreeNode<GameObject>>();
			}

			using (var _ = Source.Nodes.BeginUpdate())
			{
				var index = Source.GetNearestIndex(eventData, DropPosition);
				var dropped = false;
				if (Source.IsValid(index))
				{
					var nearest_node = Source.DataSource[index].Node;
					var nearest_parent = nearest_node.Parent;
					if (nearest_parent == null)
					{
						MoveNode(data, nearest_node, Source.Nodes, Source.RootGameObject);
						dropped = true;
					}
					else if (data.CanBeParent(nearest_parent))
					{
						MoveNode(data, nearest_node, nearest_parent.Nodes, nearest_parent.Item);
						dropped = true;
					}
				}

				if (!dropped)
				{
					data.Parent = null;
					Source.Nodes.Add(data);

					data.Item.transform.SetParent(Source.RootGameObject.transform);
					UpdateTransformOrder(Source.Nodes);
				}
			}

			HideDropIndicator();
		}

		void MoveNode(TreeNode<GameObject> data, TreeNode<GameObject> nearestNode, ObservableList<TreeNode<GameObject>> nodes, GameObject root)
		{
			var index = nodes.IndexOf(nearestNode);
			data.Parent = null;
			nodes.Insert(index, data);

			data.Item.transform.SetParent(root.transform);
			UpdateTransformOrder(nodes);
		}

		void UpdateTransformOrder(ObservableList<TreeNode<GameObject>> nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].Item.transform.SetSiblingIndex(i);
			}
		}
	}
}