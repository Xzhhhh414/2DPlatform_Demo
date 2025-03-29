namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UnityEngine;

	/// <summary>
	/// Generic node toggle animation for the TreeView.
	/// Warning: node toggle is disabled if the node is already animated because it can lead to an infinite loop (can look like partially visible node).
	/// Warning: no child node animation on expanding when parent node animation on collapse already running.
	/// Recommendation: using TreeView.DefaultItem without any layout group to avoid performance problems, refer to TreeViewComponentWithoutLayoutGroup.cs how to do it.
	/// </summary>
	/// <typeparam name="TTreeView">TreeView type.</typeparam>
	/// <typeparam name="TItemView">ItemView type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/collections/treeview-toggle-animation.html")]
	public class TreeViewCustomToggleAnimation<TTreeView, TItemView, TItem> : MonoBehaviourConditional, IUpdatable
		where TTreeView : TreeViewCustom<TItemView, TItem>
		where TItemView : TreeViewComponentBase<TItem>
	{
		/// <summary>
		/// Mode type.
		/// </summary>
		public enum ModeType
		{
			/// <summary>
			/// Constant time.
			/// </summary>
			ConstantTime = 0,

			/// <summary>
			/// Constant speed.
			/// </summary>
			ConstantSpeed = 1,
		}

		/// <summary>
		/// Toggle animation.
		/// </summary>
		protected class ToggleAnimation
		{
			struct ResizeResult
			{
				public bool Finish;

				public bool ViewUpdated;

				public ResizeResult(bool finish, bool updated)
				{
					Finish = finish;
					ViewUpdated = updated;
				}

				public readonly ResizeResult Finished() => new ResizeResult(true, ViewUpdated);

				public readonly ResizeResult Updated() => new ResizeResult(Finish, true);
			}

			TTreeView treeView;

			/// <summary>
			/// Node.
			/// </summary>
			public TreeNode<TItem> Node
			{
				get;
				private set;
			}

			/// <summary>
			/// Expand animation.
			/// </summary>
			public bool Expand;

			/// <summary>
			/// Collapse animation.
			/// </summary>
			public bool Collapse => !Expand;

			readonly List<TreeNode<TItem>> displayedNodes = new List<TreeNode<TItem>>();

			int nodeIndex;

			TItemView nodeInstance;

			RectTransform nodeToggleRect;

			Vector3 rotation;

			float rotationSpeed;

			bool horizontal;
			int index = 0;

			float speed;
			Vector2 defaultSize;
			Vector2 currentSize;
			TreeNode<TItem> currentNode;

			static readonly List<ToggleAnimation> Cache = new List<ToggleAnimation>();

			#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
			/// <summary>
			/// Reload support.
			/// </summary>
			[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
			[DomainReload(nameof(Cache))]
			static void StaticInit()
			{
				Cache.Clear();
			}
			#endif

			/// <summary>
			/// Create animation.
			/// </summary>
			/// <param name="treeView">TreeView.</param>
			/// <param name="node">Toggled node.</param>
			/// <param name="expand">Determine expand or collapse animation.</param>
			/// <param name="mode">Animation mode.</param>
			/// <param name="time">Time of animation.</param>
			/// <param name="speed">Speed of animation.</param>
			/// <returns>Animation.</returns>
			public static ToggleAnimation Create(TTreeView treeView, TreeNode<TItem> node, bool expand, ModeType mode, float time, float speed)
			{
				var animation = (Cache.Count > 0) ? Cache.Pop() : new ToggleAnimation();
				animation.Start(treeView, node, expand, mode, time, speed);

				return animation;
			}

			void Start(TTreeView treeView, TreeNode<TItem> node, bool expand, ModeType mode, float time, float speed)
			{
				this.treeView = treeView;
				horizontal = treeView.IsHorizontal();

				currentSize = Vector2.zero;
				currentNode = null;

				Node = node;
				Expand = expand;

				FindNodes(Node);

				var size = 0f;
				foreach (var n in displayedNodes)
				{
					var instance_size = treeView.GetInstanceSize(n);
					size += horizontal ? instance_size.x : instance_size.y;
				}

				this.speed = mode == ModeType.ConstantTime ? (size / time) : speed;

				InitRotation(mode == ModeType.ConstantTime ? time : (size / speed));

				index = expand ? 0 : displayedNodes.Count - 1;

				if (Expand)
				{
					HideNodes();
				}
			}

			void InitRotation(float time)
			{
				nodeIndex = Node.Index;
				nodeInstance = treeView.GetItemInstance(nodeIndex);
				if (nodeInstance.AnimateArrow)
				{
					nodeInstance.IgnoreRotation = true;
					nodeToggleRect = nodeInstance.Toggle.transform as RectTransform;

					rotation = nodeToggleRect.localEulerAngles;
					rotation.z = Expand ? 0 : -90f;

					rotationSpeed = 90f / time;
					if (Expand)
					{
						rotationSpeed = -rotationSpeed;
					}
				}
			}

			void HideNodes()
			{
				using (var _ = Node.Nodes.BeginUpdate())
				{
					foreach (var node in displayedNodes)
					{
						node.IsVisible = false;
					}
				}

				treeView.UpdateView(forced: false, isNewData: false);
			}

			void FindNodes(TreeNode<TItem> node)
			{
				foreach (var nested in node.Nodes)
				{
					if (!nested.IsVisible)
					{
						continue;
					}

					displayedNodes.Add(nested);

					if (nested.IsExpanded)
					{
						FindNodes(nested);
					}
				}
			}

			/// <summary>
			/// Update animation.
			/// </summary>
			/// <param name="dt">Delta time.</param>
			/// <returns>true if animation finished; otherwise false.</returns>
			public bool Update(float dt)
			{
				Rotate(dt);

				var result = new ResizeResult(false, false);
				using (var _ = treeView.Nodes.BeginUpdate())
				{
					result = Resize(dt * speed, result);
				}

				if (result.Finish)
				{
					Finish();
				}
				else if (!result.ViewUpdated)
				{
					treeView.UpdateView(forced: false, isNewData: false);
				}

				return result.Finish;
			}

			void Rotate(float dt)
			{
				if (!nodeInstance.AnimateArrow)
				{
					return;
				}

				if (nodeInstance.Index == nodeIndex)
				{
					var z = rotation.z;
					var target = Expand ? -90f : 0f;
					if (!Mathf.Approximately(z, target))
					{
						if (z > 0)
						{
							z -= 360f;
						}

						rotation.z = z + (rotationSpeed * dt);
						rotation.z = Expand ? Mathf.Max(rotation.z, -90f) : Mathf.Min(rotation.z, 0f);

						nodeToggleRect.localEulerAngles = rotation;
					}
				}
				else
				{
					nodeInstance.IgnoreRotation = false;
				}
			}

			ResizeResult Next(float delta, ResizeResult result)
			{
				currentNode = null;
				index += Expand ? 1 : -1;
				return Resize(delta, result);
			}

			ResizeResult Resize(float delta, ResizeResult result)
			{
				delta = Expand ? Mathf.Abs(delta) : -Mathf.Abs(delta);

				if (Mathf.Approximately(delta, 0f))
				{
					return result;
				}

				if ((index < 0) || (index >= displayedNodes.Count))
				{
					return result.Finished();
				}

				if (currentNode == null)
				{
					currentNode = displayedNodes[index];

					if (Expand)
					{
						currentNode.IsVisible = true;
						result = result.Updated();
					}

					defaultSize = treeView.GetInstanceSize(currentNode);
					currentSize = defaultSize;

					if (Expand)
					{
						currentSize[horizontal ? 0 : 1] = 0f;
					}
				}
				else
				{
					// if node animated twice: by parent collapse and child collapse
					currentSize = treeView.GetInstanceSize(currentNode);
				}

				// collapse: skip node if it already hidden or parent collapsed
				if (Collapse && (!currentNode.IsVisible || (!currentNode.Parent?.IsExpanded ?? false)))
				{
					return Next(delta, result);
				}

				var target_size = Expand ? defaultSize : Vector2.zero;
				var target_axis_size = horizontal ? target_size.x : target_size.y;
				var axis_size = horizontal ? currentSize.x : currentSize.y;

				var prev_size = axis_size;
				axis_size = Expand
					? Mathf.Min(target_axis_size, axis_size + delta)
					: Mathf.Max(target_axis_size, axis_size + delta);

				if (Mathf.Approximately(axis_size, target_axis_size))
				{
					treeView.ResetInstanceSize(currentNode);

					if (Collapse)
					{
						currentNode.IsVisible = false;
						result = result.Updated();
					}

					return Next(delta - (target_axis_size - prev_size), result);
				}

				currentSize[horizontal ? 0 : 1] = axis_size;

				treeView.SetInstanceSize(currentNode, currentSize);

				return result;
			}

			void Finish()
			{
				if (nodeInstance.Index == nodeIndex)
				{
					nodeInstance.IgnoreRotation = false;
				}

				using (var _ = treeView.Nodes.BeginUpdate())
				{
					if (Collapse)
					{
						foreach (var node in displayedNodes)
						{
							node.IsVisible = true;
						}
					}

					Node.IsExpanded = Expand;
				}

				currentNode = null;
				displayedNodes.Clear();

				Cache.Add(this);
			}

			/// <summary>
			/// Stop animation.
			/// </summary>
			public void Stop()
			{
				treeView.Nodes.BeginUpdate();

				foreach (var node in displayedNodes)
				{
					treeView.ResetInstanceSize(node);
					node.IsVisible = true;
				}

				Finish();
			}
		}

		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		protected TTreeView TreeView;

		/// <summary>
		/// Animation mode.
		/// </summary>
		[SerializeField]
		public ModeType Mode = ModeType.ConstantTime;

		/// <summary>
		/// Time in seconds to expand or collapse all nested nodes.
		/// </summary>
		[SerializeField]
		[EditorConditionEnum(nameof(Mode), (int)ModeType.ConstantTime)]
		[Tooltip("Time in seconds to expand or collapse all nested nodes.")]
		public float Time = 0.25f; // seconds to close

		/// <summary>
		/// Animation speed in points per second.
		/// </summary>
		[SerializeField]
		[EditorConditionEnum(nameof(Mode), (int)ModeType.ConstantSpeed)]
		[Tooltip("Speed in points per second")]
		public float Speed = 200f; // points per second

		/// <summary>
		/// Run animation with unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Animations.
		/// </summary>
		protected List<ToggleAnimation> Animations = new List<ToggleAnimation>();

		bool enabledUpdate;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TreeView.ListType = ListViewType.ListViewWithVariableSize;
			TreeView.PrecalculateItemSizes = false;
			TreeView.Init();

			TreeView.NodeToggle.AddListener(ProcessNodeToggle);
			TreeView.AllowToggle = AllowToggle;
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			if (TreeView != null)
			{
				TreeView.NodeToggle.RemoveListener(ProcessNodeToggle);
				TreeView.AllowToggle = null;
			}
		}

		bool AllowToggle(TreeNode<TItem> targetNode)
		{
			var future_collapse = targetNode.IsExpanded; // because will be changed after AllowToggle
			foreach (var a in Animations)
			{
				if (a.Node == targetNode)
				{
					a.Expand = !a.Expand;
					return false;
				}

				// deny child node collapse if its parent animation on expand running
				// OR deny parent node collapse if its child animation on expand running
				// because infinite loop possible:
				// size increased by parent node animation and then decreased by the child node animation on same value
				// => size change is 0 so animations never end
				var deny = (a.Expand && future_collapse)
					&& (a.Node.IsParentOfNode(targetNode) || targetNode.IsParentOfNode(a.Node));
				if (deny)
				{
					return false;
				}
			}

			return true;
		}

		void ProcessNodeToggle(TreeNode<TItem> targetNode)
		{
			if (!targetNode.HasVisibleNodes)
			{
				return;
			}

			var expand = targetNode.IsExpanded;
			targetNode.IsExpanded = true;

			foreach (var a in Animations)
			{
				if (a.Node == targetNode)
				{
					a.Expand = expand;
					return;
				}

				// do not animate child collapse if parent animation on expand running
				if (a.Expand && !expand && a.Node.IsParentOfNode(targetNode))
				{
					return;
				}
			}

			Animations.Add(ToggleAnimation.Create(TreeView, targetNode, expand, Mode, Time, Speed));

			if (!enabledUpdate)
			{
				Updater.Add(this);
			}
		}

		/// <summary>
		/// Run update.
		/// </summary>
		public void RunUpdate()
		{
			var dt = UtilitiesTime.GetDeltaTime(UnscaledTime);
			for (var i = Animations.Count - 1; i >= 0; i--)
			{
				var animation = Animations[i];
				if (animation.Update(dt))
				{
					Animations.RemoveAt(i);
				}
			}

			if (Animations.Count == 0)
			{
				enabledUpdate = false;
				Updater.Remove(this);
			}
		}

		/// <summary>
		/// Stop animations.
		/// </summary>
		public void Stop()
		{
			foreach (var a in Animations)
			{
				a.Stop();
			}

			Animations.Clear();
		}
	}
}