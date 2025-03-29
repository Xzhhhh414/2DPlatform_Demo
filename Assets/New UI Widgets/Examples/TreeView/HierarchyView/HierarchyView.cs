namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// HierarchyView.
	/// </summary>
	public class HierarchyView : TreeViewCustom<HierarchyItemView, GameObject>
	{
		[SerializeField]
		GameObject rootGameObject;

		/// <summary>
		/// Root GameObject.
		/// </summary>
		public GameObject RootGameObject
		{
			get => rootGameObject;

			set
			{
				if (rootGameObject != value)
				{
					rootGameObject = value;
					ReloadHierarchy();
				}
			}
		}

		readonly Dictionary<int, TreeNode<GameObject>> nodesCache = new Dictionary<int, TreeNode<GameObject>>();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			ReloadHierarchy();
		}

		/// <summary>
		/// Recreate hierarchy.
		/// </summary>
		public virtual void ReloadHierarchy()
		{
			void UpdateNodesCache(ObservableList<TreeNode<GameObject>> nodes)
			{
				foreach (var node in nodes)
				{
					if (node.Item == null)
					{
						continue;
					}

					nodesCache[node.Item.GetInstanceID()] = node;

					UpdateNodesCache(node.Nodes);
				}
			}

			Nodes ??= new ObservableList<TreeNode<GameObject>>();

			if (RootGameObject == null)
			{
				Nodes.Clear();
				return;
			}

			nodesCache.Clear();
			UpdateNodesCache(Nodes);

			UpdateNodes(RootGameObject.transform, Nodes);

			nodesCache.Clear();
		}

		/// <summary>
		/// Update nodes.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="nodes">Nodes.</param>
		protected virtual void UpdateNodes(Transform root, ObservableList<TreeNode<GameObject>> nodes)
		{
			using var _ = nodes.BeginUpdate();
			nodes.Clear();

			for (int i = 0; i < root.childCount; i++)
			{
				var t = root.GetChild(i);
				var go = t.gameObject;

				if (!nodesCache.TryGetValue(go.GetInstanceID(), out var node))
				{
					node = new TreeNode<GameObject>(t.gameObject, new ObservableList<TreeNode<GameObject>>());
				}

				UpdateNodes(t, node.Nodes);

				nodes.Add(node);
			}
		}
	}
}