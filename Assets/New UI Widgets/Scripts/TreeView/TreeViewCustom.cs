namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UIWidgets.Internal;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// TreeViewCustom.
	/// </summary>
	/// <typeparam name="TItemView">Type of DefaultItem.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/treeview.html")]
	public partial class TreeViewCustom<TItemView, TItem> : ListViewCustom<TItemView, ListNode<TItem>>
		where TItemView : TreeViewComponentBase<TItem>
	{
		/// <summary>
		/// NodeEvent.
		/// </summary>
		[Serializable]
		public class NodeEvent : UnityEvent<TreeNode<TItem>>
		{
		}

		ObservableList<TreeNode<TItem>> nodes;

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		[DataBindField]
		public virtual ObservableList<TreeNode<TItem>> Nodes
		{
			get
			{
				if (nodes == null)
				{
					Nodes = new ObservableList<TreeNode<TItem>>();
				}

				return nodes;
			}

			set
			{
				if (ReferenceEquals(nodes, value))
				{
					return;
				}

				Init();

				nodes?.OnChangeMono.RemoveListener(NodesChanged);

				RootNode.DetachNodes();
				nodes = value;
				RootNode.Nodes = value;

				Cleanup();
				ListRenderer.SetPosition(0f);
				Refresh();

				nodes?.OnChangeMono.AddListener(NodesChanged);
			}
		}

		/// <summary>
		/// Gets the selected node.
		/// </summary>
		/// <value>The selected node.</value>
		[DataBindField]
		public TreeNode<TItem> SelectedNode
		{
			get
			{
				var n = selectedNodes.Count;
				if (n == 0)
				{
					return null;
				}

				return selectedNodes.Last();
			}
		}

		/// <summary>
		/// NodeToggle event.
		/// </summary>
		public NodeEvent NodeToggle = new NodeEvent();

		/// <summary>
		/// NodeSelected event.
		/// </summary>
		[DataBindEvent(nameof(SelectedNode), nameof(SelectedNodes))]
		public NodeEvent NodeSelected = new NodeEvent();

		/// <summary>
		/// NodeDeselected event.
		/// </summary>
		[DataBindEvent(nameof(SelectedNode), nameof(SelectedNodes))]
		public NodeEvent NodeDeselected = new NodeEvent();

		/// <summary>
		/// The selected nodes.
		/// </summary>
		protected LinkedHashSet<TreeNode<TItem>> selectedNodes = new LinkedHashSet<TreeNode<TItem>>();

		/// <summary>
		/// Gets or sets the selected nodes.
		/// </summary>
		/// <value>The selected nodes.</value>
		[DataBindField]
		public List<TreeNode<TItem>> SelectedNodes
		{
			get => new List<TreeNode<TItem>>(selectedNodes);

			set
			{
				selectedNodes.Clear();

				using var _ = ListPool<int>.Get(out var temp);

				Nodes2Indices(value, temp);

				// safe to use because only items copied, not the list reference
				SelectedIndices = temp;
			}
		}

		[SerializeField]
		bool deselectCollapsedNodes = true;

		/// <summary>
		/// The deselect collapsed nodes.
		/// </summary>
		public bool DeselectCollapsedNodes
		{
			get => deselectCollapsedNodes;

			set
			{
				deselectCollapsedNodes = value;
				if (value)
				{
					selectedNodes.Clear();
					var selected = SelectedIndicesList;
					foreach (var index in selected)
					{
						selectedNodes.Add(DataSource[index].Node);
					}
				}
			}
		}

		/// <summary>
		/// Opened nodes converted to list.
		/// </summary>
		protected ObservableList<ListNode<TItem>> NodesList = new ObservableList<ListNode<TItem>>();

		/// <summary>
		/// Scroll with node indent.
		/// </summary>
		[SerializeField]
		public bool ScrollWithIndent = false;

		/// <summary>
		/// Allow expand node on move right event and collapse node on move left event.
		/// </summary>
		[SerializeField]
		[Tooltip("Allow expand node on move right event and collapse node on move left event.")]
		public bool ToggleOnNavigate;

		/// <summary>
		/// Allow expand node on submit event and collapse node on cancel event.
		/// </summary>
		[SerializeField]
		[Tooltip("Allow expand node on submit event and collapse node on cancel event.")]
		public bool ToggleOnSubmitCancel;

		TreeNode<TItem> rootNode;

		/// <summary>
		/// Gets the root node.
		/// </summary>
		/// <value>The root node.</value>
		public TreeNode<TItem> RootNode => rootNode;

		/// <summary>
		/// List can be looped and items is enough to make looped list.
		/// </summary>
		/// <value><c>true</c> if looped list available; otherwise, <c>false</c>.</value>
		public override bool LoopedListAvailable => false;

		/// <summary>
		/// Require EasyLayout.
		/// </summary>
		protected override bool RequireEasyLayout => true;

		/// <summary>
		/// Allows or denies the node's IsExpanded property modification.
		/// </summary>
		public Func<TreeNode<TItem>, bool> AllowToggle = DefaultAllowToggle;

		/// <summary>
		/// Always allows the node's IsExpanded property modification.
		/// </summary>
		[DomainReloadExclude]
		public static readonly Func<TreeNode<TItem>, bool> DefaultAllowToggle = node => true;

		/// <summary>
		/// Cache.
		/// </summary>
		protected List<ListNode<TItem>> Cache = new List<ListNode<TItem>>();

		/// <summary>
		/// Delegate of the ToggleNode.
		/// </summary>
		protected UnityAction<int, ListViewItem> ToggleNodeDelegate;

		/// <summary>
		/// Delegate of the ProcessInstanceSubmit.
		/// </summary>
		protected UnityAction<int, ListViewItem, BaseEventData> ProcessInstanceSubmitDelegate;

		/// <summary>
		/// Delegate of the ProcessInstanceCancel.
		/// </summary>
		protected UnityAction<int, ListViewItem, BaseEventData> ProcessInstanceCancelDelegate;

		readonly Queue<TreeNode<TItem>> tempQueue = new Queue<TreeNode<TItem>>();

		NodeInstanceSizes<TItem> instancesSizes;

		/// <summary>
		/// Prevent scrollbar blink caused by virtualization: the container will have the maximum width of all items.
		/// By default, the container has the maximum width of only visible items.
		/// Require ListType = List View with Variable Size.
		/// </summary>
		[SerializeField]
		[Tooltip("Prevent scrollbar blink caused by virtualization: the container will have the maximum width of all items.\nBy default, the container has the maximum width of only visible items.")]
		public bool ContainerMaxSize = false;

		/// <summary>
		/// Container LayoutElement.
		/// </summary>
		protected LayoutElement ContainerLayoutElement;

		/// <summary>
		/// Overridden instances sizes. Used for the animation purpose.
		/// </summary>
		protected NodeInstanceSizes<TItem> InstancesSizes
		{
			get
			{
				instancesSizes ??= new NodeInstanceSizes<TItem>();

				return instancesSizes;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void InitOnce()
		{
			if (DefaultItem != null)
			{
				if (DefaultItem.TryGetComponent<EasyLayoutNS.EasyLayout>(out var default_item_layout) && !IsHorizontal())
				{
					default_item_layout.CompactConstraint = EasyLayoutNS.CompactConstraints.MaxRowCount;
					default_item_layout.CompactConstraintCount = 1;
				}
			}

			var valid_list_type = (listType == ListViewType.ListViewWithFixedSize)
				|| (listType == ListViewType.ListViewWithVariableSize);
			if (!valid_list_type)
			{
				Debug.LogWarning("TreeView does not support TileView mode", this);
				listType = ListViewType.ListViewWithFixedSize;
			}

			rootNode = new TreeNode<TItem>(default);

			ContainerLayoutElement = Utilities.RequireComponent<LayoutElement>(Container);

			setContentSizeFitter = false;

			ToggleNodeDelegate = OnToggleNode;
			ProcessInstanceSubmitDelegate = ProcessInstanceSubmit;
			ProcessInstanceCancelDelegate = ProcessInstanceCancel;

			InstancesEventsInternal.NodeToggleClick.AddListener(ToggleNodeDelegate);
			InstancesEventsInternal.Submit.AddListener(ProcessInstanceSubmitDelegate);
			InstancesEventsInternal.Cancel.AddListener(ProcessInstanceCancelDelegate);

			base.InitOnce();

			ComponentsPool.InstanceCompare = (index, instance) => ReferenceEquals(DataSource[index].Node, instance.Node);

			Refresh();

			KeepSelection = true;

			DataSource = NodesList;
		}

		/// <inheritdoc/>
		protected override ListViewBaseScrollData CreateScrollData()
		{
			return new TreeViewScrollData(this);
		}

		/// <summary>
		/// Process instance submit event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="instance">Instance.</param>
		/// <param name="eventData">Event data.</param>
		protected virtual void ProcessInstanceSubmit(int index, ListViewItem instance, BaseEventData eventData)
		{
			if (ToggleOnSubmitCancel)
			{
				DataSource[index].Node.IsExpanded = true;
			}
		}

		/// <summary>
		/// Process instance cancel event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="instance">Instance.</param>
		/// <param name="eventData">Event data.</param>
		protected virtual void ProcessInstanceCancel(int index, ListViewItem instance, BaseEventData eventData)
		{
			if (ToggleOnSubmitCancel)
			{
				DataSource[index].Node.IsExpanded = false;
			}
		}

		/// <inheritdoc/>
		protected override void OnItemCancel(int index, ListViewItem instance, BaseEventData eventData)
		{
			if (ToggleOnSubmitCancel)
			{
				return;
			}

			base.OnItemCancel(index, instance, eventData);
		}

		/// <summary>
		/// Process the item move event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		/// <param name="eventData">Event data.</param>
		protected override void OnItemMove(int index, ListViewItem item, AxisEventData eventData)
		{
			if (!Navigation)
			{
				return;
			}

			var axis_horizontal = (eventData.moveDir == MoveDirection.Left) || (eventData.moveDir == MoveDirection.Right);
			var expand_collapse = Navigation && ToggleOnNavigate && (IsHorizontal() == !axis_horizontal);
			if (expand_collapse)
			{
				var node = DataSource[index].Node;
				var expand = (eventData.moveDir == MoveDirection.Right) || (eventData.moveDir == MoveDirection.Down);
				node.IsExpanded = expand;

				Navigate(eventData, node.Index, node.Index > item.Index);
			}
			else
			{
				base.OnItemMove(index, item, eventData);
			}
		}

		/// <summary>
		/// Get selected nodes.
		/// </summary>
		/// <param name="output">Output.</param>
		public void GetSelectedNodes(List<TreeNode<TItem>> output)
		{
			output.AddRange(selectedNodes);
		}

		/// <summary>
		/// Get ListNode.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		/// <returns>New ListNode.</returns>
		protected ListNode<TItem> GetListNode(TreeNode<TItem> node, int depth)
		{
			if (Cache.Count > 0)
			{
				return Cache.Pop().Replace(node, depth);
			}

			return new ListNode<TItem>(node, depth);
		}

		/// <inheritdoc/>
		protected override ListViewTypeBase GetRenderer(ListViewType type)
		{
			ListViewTypeBase renderer = type switch
			{
				ListViewType.ListViewWithFixedSize => new ListViewTypeFixed(this),
				ListViewType.ListViewWithVariableSize => new ListViewTypeSize(this, InstancesSizes),
				ListViewType.ListViewEllipse => throw new NotSupportedException("ListViewType.ListViewEllipse not supported for the TreeView"),
				ListViewType.TileViewWithFixedSize => throw new NotSupportedException("ListViewType.TileViewWithFixedSize not supported for the TreeView"),
				ListViewType.TileViewWithVariableSize => throw new NotSupportedException("ListViewType.TileViewWithVariableSize not supported for the TreeView"),
				ListViewType.TileViewStaggered => throw new NotSupportedException("ListViewType.TileViewStaggered not supported for the TreeView"),
				_ => throw new NotSupportedException(string.Format("Unknown ListView type: {0}", EnumHelper<ListViewType>.ToString(type))),
			};
			renderer.Init();
			renderer.ToggleScrollToItemCenter(ScrollInertiaUntilItemCenter);

			return renderer;
		}

		/// <inheritdoc/>
		protected override void SetDirection(ListViewDirection newDirection, bool updateView = true)
		{
			direction = newDirection;

			ContainerAnchoredPosition = Vector2.zero;

			if (ListRenderer.IsVirtualizationSupported())
			{
				LayoutBridge.IsHorizontal = IsHorizontal();

				CalculateMaxVisibleItems();
			}

			if (updateView)
			{
				UpdateView(forced: true, isNewData: false);
			}
		}

		/// <summary>
		/// Convert nodes tree to list.
		/// </summary>
		/// <returns>The list.</returns>
		/// <param name="sourceNodes">Source nodes.</param>
		/// <param name="depth">Depth.</param>
		/// <param name="list">List.</param>
		protected virtual int Nodes2List(IObservableList<TreeNode<TItem>> sourceNodes, int depth, ObservableList<ListNode<TItem>> list)
		{
			var added_nodes = 0;
			for (var i = 0; i < sourceNodes.Count; i++)
			{
				var node = sourceNodes[i];
				if (!node.IsVisible)
				{
					node.Index = -1;
					ResetNodesIndex(node.Nodes);
					continue;
				}

				list.Add(GetListNode(node, depth));
				node.Index = list.Count - 1;

				if (node.IsExpanded && (node.Nodes != null) && (node.Nodes.Count > 0))
				{
					var used = Nodes2List(node.Nodes, depth + 1, list);
					node.UsedNodesCount = used;
				}
				else
				{
					ResetNodesIndex(node.Nodes);
					node.UsedNodesCount = 0;
				}

				added_nodes += 1;
			}

			return added_nodes;
		}

		/// <summary>
		/// Reset nodes indexes.
		/// </summary>
		/// <param name="sourceNodes">Source nodes.</param>
		protected virtual void ResetNodesIndex(IObservableList<TreeNode<TItem>> sourceNodes)
		{
			if (sourceNodes == null)
			{
				return;
			}

			for (var i = 0; i < sourceNodes.Count; i++)
			{
				var node = sourceNodes[i];
				node.Index = -1;
				ResetNodesIndex(node.Nodes);
			}
		}

		/// <summary>
		/// Process the toggle node event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="component">Component.</param>
		protected void OnToggleNode(int index, ListViewItem component)
		{
			if (ToggleNode(index))
			{
				NodeToggle.Invoke(NodesList[index].Node);
			}
		}

		/// <summary>
		/// Expand the parent nodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void ExpandParentNodes(TreeNode<TItem> node)
		{
			var current = node.Parent;
			while (current != null)
			{
				current.IsExpanded = true;
				current = current.Parent;
			}
		}

		/// <summary>
		/// Collapse the parent nodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void CollapseParentNodes(TreeNode<TItem> node)
		{
			var current = node.Parent;
			while (current != null)
			{
				current.IsExpanded = false;
				current = current.Parent;
			}
		}

		/// <summary>
		/// Select the node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void Select(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return;
			}

			var index = Node2Index(node);
			if (IsValid(index))
			{
				Select(index);
			}
			else if (index == -1 && !DeselectCollapsedNodes)
			{
				if (!MultipleSelect)
				{
					foreach (var n in SelectedNodes)
					{
						Deselect(n);
					}
				}

				selectedNodes.Add(node);
			}
		}

		/// <summary>
		/// Select the node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void SelectNode(TreeNode<TItem> node)
		{
			Select(node);
		}

		/// <summary>
		/// Select the node with sub-nodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public void SelectNodeWithSubnodes(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return;
			}

			tempQueue.Enqueue(node);

			while (tempQueue.Count > 0)
			{
				var current_node = tempQueue.Dequeue();

				var index = Node2Index(current_node);

				if (index != -1)
				{
					Select(index);
					if (current_node.Nodes != null)
					{
						foreach (var n in current_node.Nodes)
						{
							tempQueue.Enqueue(n);
						}
					}
				}
				else if (!DeselectCollapsedNodes)
				{
					selectedNodes.Add(current_node);

					if (current_node.Nodes != null)
					{
						foreach (var n in current_node.Nodes)
						{
							tempQueue.Enqueue(n);
						}
					}
				}
			}
		}

		/// <summary>
		/// Deselect the node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void Deselect(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return;
			}

			var index = Node2Index(node);
			if (IsValid(index))
			{
				Deselect(index);
			}
			else if (index == -1 && !DeselectCollapsedNodes)
			{
				selectedNodes.Remove(node);
			}
		}

		/// <summary>
		/// Deselect the node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void DeselectNode(TreeNode<TItem> node)
		{
			Deselect(node);
		}

		/// <summary>
		/// Deselect the node with sub-nodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public void DeselectNodeWithSubnodes(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return;
			}

			tempQueue.Enqueue(node);

			while (tempQueue.Count > 0)
			{
				var current_node = tempQueue.Dequeue();

				var index = Node2Index(current_node);

				if (index != -1)
				{
					Deselect(index);
					if (current_node.Nodes != null)
					{
						foreach (var n in current_node.Nodes)
						{
							tempQueue.Enqueue(n);
						}
					}
				}
				else if (!DeselectCollapsedNodes)
				{
					selectedNodes.Remove(current_node);
					if (current_node.Nodes != null)
					{
						foreach (var n in current_node.Nodes)
						{
							tempQueue.Enqueue(n);
						}
					}
				}
			}
		}

		/// <inheritdoc/>
		protected override void InvokeSelect(int index, bool raiseEvents)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning(string.Format("Incorrect index: {0}", index.ToString()), this);
				return;
			}

			var node = NodesList[index].Node;

			selectedNodes.Add(node);

			base.InvokeSelect(index, raiseEvents);

			if (raiseEvents)
			{
				NodeSelected.Invoke(node);
			}
		}

		/// <inheritdoc/>
		protected override void InvokeDeselect(int index, bool raiseEvents)
		{
			var is_valid = IsValid(index);
			if (is_valid)
			{
				var node = NodesList[index].Node;

				if (DeselectCollapsedNodes || !IsNodeInTree(node) || Node2Index(node) != -1)
				{
					selectedNodes.Remove(node);
				}

				base.InvokeDeselect(index, raiseEvents);

				if (raiseEvents)
				{
					InvokeNodeDeselect(node);
				}
			}
			else
			{
				base.InvokeDeselect(index, raiseEvents);
			}
		}

		/// <summary>
		/// Invokes the node deselect event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected virtual void InvokeNodeDeselect(TreeNode<TItem> node)
		{
			NodeDeselected.Invoke(node);
		}

		/// <inheritdoc/>
		protected override void RecalculateSelectedIndices(ObservableList<ListNode<TItem>> newItems)
		{
			NewSelectedIndices.Clear();

			foreach (var node in selectedNodes)
			{
				var index = Node2Index(newItems, node);
				if (index == -1)
				{
					InvokeNodeDeselect(node);
				}
				else
				{
					NewSelectedIndices.Add(index);
				}
			}
		}

		/// <summary>
		/// Get node index.
		/// </summary>
		/// <param name="nodes">Nodes list.</param>
		/// <param name="node">Node.</param>
		/// <returns>Index of the node.</returns>
		protected int Node2Index(ObservableList<ListNode<TItem>> nodes, TreeNode<TItem> node)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].Node == node)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Determines whether specified node in tree.
		/// </summary>
		/// <returns><c>true</c> if tree contains specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		public bool IsNodeInTree(TreeNode<TItem> node) => ReferenceEquals(RootNode, node.RootNode);

		/// <summary>
		/// Toggles the node.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>true if node was toggled; otherwise false.</returns>
		protected virtual bool ToggleNode(int index)
		{
			var node = NodesList[index];

			if (AllowToggle(node.Node))
			{
				node.Node.IsExpanded = !node.Node.IsExpanded;

				return true;
			}

			return false;
		}

		/// <summary>
		/// Get indices of specified nodes.
		/// </summary>
		/// <param name="targetNodes">Target nodes.</param>
		/// <param name="output">Indices.</param>
		protected void Nodes2Indices(List<TreeNode<TItem>> targetNodes, List<int> output)
		{
			for (int i = 0; i < targetNodes.Count; i++)
			{
				var node = targetNodes[i];
				if (!IsNodeInTree(node))
				{
					continue;
				}

				var index = Node2Index(node);

				if (index != -1)
				{
					output.Add(index);
				}
			}
		}

		/// <summary>
		/// Get node by the index.
		/// </summary>
		/// <param name="index">The node index.</param>
		/// <returns>Node.</returns>
		public TreeNode<TItem> Index2Node(int index) => IsValid(index) ? DataSource[index].Node : null;

		/// <summary>
		/// Get index of the node.
		/// </summary>
		/// <returns>The node index.</returns>
		/// <param name="node">Node.</param>
		public int Node2Index(TreeNode<TItem> node) => node.Index;

		/// <summary>
		/// Update view when node data changed.
		/// </summary>
		protected virtual void NodesChanged()
		{
			Refresh();
		}

		/// <summary>
		/// Clear NodesList.
		/// </summary>
		protected void NodesListClear()
		{
			foreach (var node in NodesList)
			{
				node.Node = null;
				node.Depth = -1;
			}

			Cache.AddRange(NodesList);
			NodesList.Clear();
		}

		/// <summary>
		/// Refresh this instance.
		/// </summary>
		public virtual void Refresh()
		{
			if (nodes == null)
			{
				NodesListClear();

				return;
			}

			using (var _ = NodesList.BeginUpdate())
			{
				var selected_nodes = SelectedNodes;
				NodesListClear();

				Nodes2List(nodes, 0, NodesList);

				SilentDeselect(SelectedIndicesList);

				using var __ = ListPool<int>.Get(out var temp);

				Nodes2Indices(selected_nodes, temp);
				SilentSelect(temp);
			}

			if (DeselectCollapsedNodes)
			{
				selectedNodes.Clear();
				var selected = SelectedIndicesList;
				foreach (var index in selected)
				{
					selectedNodes.Add(DataSource[index].Node);
				}
			}
		}

		/// <summary>
		/// Clear items of this instance.
		/// </summary>
		public override void Clear()
		{
			nodes.Clear();
			ListRenderer.SetPosition(0f);
		}

		/// <summary>
		/// [Not supported for TreeView] Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		[Obsolete("Not supported for TreeView", true)]
		public virtual int Add(TItem item)
		{
			return -1;
		}

		/// <summary>
		/// [Not supported for TreeView] Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed TItem.</returns>
		[Obsolete("Not supported for TreeView", true)]
		public virtual int Remove(TItem item)
		{
			return -1;
		}

		/// <summary>
		/// [Not supported for TreeView] Remove item by specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public override void Remove(int index)
		{
			throw new NotSupportedException("Not supported for TreeView.");
		}

		/// <summary>
		/// [Not supported for TreeView] Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		[Obsolete("Not supported for TreeView", true)]
		public virtual int Set(TItem item, bool allowDuplicate = true)
		{
			return -1;
		}

		/// <summary>
		/// Removes first elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">Match.</param>
		/// <returns>true if item is successfully removed; otherwise, false.</returns>
		public bool Remove(Predicate<TreeNode<TItem>> match)
		{
			var index = nodes.FindIndex(match);
			if (index != -1)
			{
				nodes.RemoveAt(index);
				return true;
			}

			for (var i = 0; i < nodes.Count; i++)
			{
				var node = nodes[i];
				if (node.Nodes == null)
				{
					continue;
				}

				if (Remove(node.Nodes, match))
				{
					return true;
				}
			}

			return false;
		}

		static bool Remove(ObservableList<TreeNode<TItem>> nodes, Predicate<TreeNode<TItem>> match)
		{
			var index = nodes.FindIndex(match);
			if (index != -1)
			{
				nodes.RemoveAt(index);
				return true;
			}

			for (var i = 0; i < nodes.Count; i++)
			{
				var node = nodes[i];
				if (node.Nodes == null)
				{
					continue;
				}

				if (Remove(node.Nodes, match))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Scroll to the specified node immediately.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void ScrollTo(TreeNode<TItem> node)
		{
			ExpandParentNodes(node);
			var index = Node2Index(node);
			if (index < 0)
			{
				Debug.LogWarning("Specified node not found: node not in TreeView.Nodes or function called between .BeginUpdate() and .EndUpdate() calls");
				return;
			}

			ScrollTo(index);
		}

		/// <summary>
		/// Scroll to the specified node with animation.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void ScrollToAnimated(TreeNode<TItem> node)
		{
			ExpandParentNodes(node);
			var index = Node2Index(node);
			if (index < 0)
			{
				Debug.LogWarning("Specified node not found: node not in TreeView.Nodes or function called between .BeginUpdate() and .EndUpdate() calls");
				return;
			}

			ScrollToAnimated(index);
		}

		/// <summary>
		/// Get node indentation.
		/// </summary>
		/// <param name="index">Node index.</param>
		/// <returns>Indentation.</returns>
		public virtual float GetNodeIndentation(int index)
		{
			if (index < 0)
			{
				return 0f;
			}

			return DataSource[index].Depth * DefaultItem.PaddingPerLevel;
		}

		/// <summary>
		/// Get secondary scroll position (for the cross direction).
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Secondary scroll position.</returns>
		protected override float GetScrollPositionSecondary(int index)
		{
			if (!ScrollWithIndent)
			{
				return base.GetScrollPositionSecondary(index);
			}

			if (ListRenderer.IsVirtualizationPossible())
			{
				var scroll_value = IsHorizontal() ? Viewport.Size.y : Viewport.Size.x;

				var item_size = ListRenderer.GetInstanceFullSize(index);
				var item_value = IsHorizontal()
					? (item_size.y + LayoutBridge.GetFullMarginY())
					: (item_size.x + LayoutBridge.GetFullMarginX());

				if (scroll_value >= item_value)
				{
					return base.GetScrollPositionSecondary(index);
				}
			}

			var value = GetNodeIndentation(index);

			return IsHorizontal() ? value : -value;
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire Nodes.
		/// </summary>
		/// <param name="match">The Predicate{TItem} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, null.</returns>
		public TreeNode<TItem> FindNode(Predicate<TreeNode<TItem>> match)
		{
			return FindNode(Nodes, match);
		}

		/// <summary>
		/// Searches for the elements that matches the conditions defined by the specified predicate, and returns the all occurrences within the entire Nodes.
		/// </summary>
		/// <param name="match">The Predicate{TItem} delegate that defines the conditions of the elements to search for.</param>
		/// <param name="output">List with founded nodes.</param>
		public void FindNodes(Predicate<TreeNode<TItem>> match, List<TreeNode<TItem>> output)
		{
			FindNodes(Nodes, match, output);
		}

		/// <summary>
		/// Searches for the elements that matches the conditions defined by the specified predicate, and returns the all occurrences within the entire Nodes.
		/// </summary>
		/// <param name="match">The Predicate{TItem} delegate that defines the conditions of the elements to search for.</param>
		/// <returns>The all elements that matches the conditions defined by the specified predicate, if found; otherwise, null.</returns>
		public List<TreeNode<TItem>> FindNodes(Predicate<TreeNode<TItem>> match)
		{
			var result = new List<TreeNode<TItem>>();
			FindNodes(Nodes, match, result);

			return result;
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the specified nodes.
		/// </summary>
		/// <param name="searchNodes">Nodes to search.</param>
		/// <param name="match">The Predicate{TItem} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, null.</returns>
		protected static TreeNode<TItem> FindNode(ObservableList<TreeNode<TItem>> searchNodes, Predicate<TreeNode<TItem>> match)
		{
			if (searchNodes == null)
			{
				return null;
			}

			var result = searchNodes.Find(match);

			if (result != null)
			{
				return result;
			}

			foreach (var node in searchNodes)
			{
				var subnode = FindNode(node.Nodes, match);

				if (subnode != null)
				{
					return subnode;
				}
			}

			return null;
		}

		/// <summary>
		/// Searches for an elements that matches the conditions defined by the specified predicate, and returns the all occurrences within the specified nodes.
		/// </summary>
		/// <param name="searchNodes">Nodes to search.</param>
		/// <param name="match">The Predicate{TItem} delegate that defines the conditions of the elements to search for.</param>
		/// <param name="output">List with founded nodes.</param>
		protected static void FindNodes(ObservableList<TreeNode<TItem>> searchNodes, Predicate<TreeNode<TItem>> match, List<TreeNode<TItem>> output)
		{
			if (searchNodes == null)
			{
				return;
			}

			foreach (var node in searchNodes)
			{
				if (match(node))
				{
					output.Add(node);
				}

				FindNodes(node.Nodes, match, output);
			}
		}

		/// <summary>
		/// Toggle nodes visibility if they match predicate.
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		/// <param name="alwaysIncludeNested">Always include nested nodes of matched nodes.</param>
		/// <returns>true if any node match predicate; otherwise, false.</returns>
		public bool FilterNodes(Predicate<TreeNode<TItem>> predicate, bool alwaysIncludeNested = false) => Nodes.Filter(predicate, alwaysIncludeNested);

		/// <summary>
		/// Cleanup.
		/// </summary>
		protected void Cleanup() => InstancesSizes.RemoveNotExisting(IsNodeInTree);

		/// <inheritdoc/>
		public override void UpdateView(bool forced, bool isNewData)
		{
			base.UpdateView(forced, isNewData);

			if (!IsInited)
			{
				return;
			}

			if (ContainerMaxSize)
			{
				var size = InstancesSizes.MaxSize(DefaultInstanceSize);
				if (IsHorizontal())
				{
					ContainerLayoutElement.preferredHeight = size.y;
				}
				else
				{
					ContainerLayoutElement.preferredWidth = size.x;
				}
			}
		}

		/// <summary>
		/// Get the size of the instance for the specified node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>The instance size.</returns>
		public virtual Vector2 GetInstanceSize(TreeNode<TItem> node)
		{
			if (InstancesSizes.TryGetOverridden(node, out var size))
			{
				return size;
			}

			if (!IsValid(node.Index))
			{
				return DefaultInstanceSize;
			}

			return ListRenderer.GetInstanceFullSize(node.Index);
		}

		/// <summary>
		/// Set the size of the instance for the specified node.
		/// UpdateView() should be called after it to apply changes.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="size">Size.</param>
		public virtual void SetInstanceSize(TreeNode<TItem> node, Vector2 size)
		{
			InstancesSizes.SetOverridden(node, size);
		}

		/// <summary>
		/// Reset the size to the default for the specified node.
		/// UpdateView() should be called after it to apply changes.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void ResetInstanceSize(TreeNode<TItem> node)
		{
			InstancesSizes.RemoveOverridden(node);
			ResetInstanceSize(node.Index);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (InstancesEventsInternal != null)
			{
				InstancesEventsInternal.NodeToggleClick.RemoveListener(ToggleNodeDelegate);
				InstancesEventsInternal.Submit.RemoveListener(ProcessInstanceSubmitDelegate);
				InstancesEventsInternal.Cancel.RemoveListener(ProcessInstanceCancelDelegate);
			}

			rootNode?.DetachNodes();

			Cleanup();

			nodes = null;

			base.OnDestroy();
		}
	}
}