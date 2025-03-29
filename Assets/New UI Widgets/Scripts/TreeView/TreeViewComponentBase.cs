namespace UIWidgets
{
	using System;
	using System.Collections;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Node toggle type.
	/// </summary>
	public enum NodeToggle
	{
		/// <summary>
		/// Rotate.
		/// </summary>
		Rotate = 0,

		/// <summary>
		/// ChangeSprite.
		/// </summary>
		ChangeSprite = 1,
	}

	/// <summary>
	/// Tree view component base.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/treeview.html")]
	public abstract class TreeViewComponentBase : ListViewItem
	{
		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TextAdapter), };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with TextAdapter.")]
		public Text Text;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// The toggle.
		/// </summary>
		[SerializeField]
		[Tooltip("Expand / Collapse node.")]
		public TreeNodeToggle Toggle;

		Image toggleImage;

		/// <summary>
		/// Gets the toggle image.
		/// </summary>
		/// <value>The toggle image.</value>
		protected Image ToggleImage
		{
			get
			{
				if (toggleImage == null)
				{
					Toggle.TryGetComponent(out toggleImage);
				}

				return toggleImage;
			}
		}

		/// <summary>
		/// The toggle event.
		/// </summary>
		[SerializeField]
		public NodeToggleEvent ToggleEvent = new NodeToggleEvent();

		/// <summary>
		/// Indentation.
		/// </summary>
		[FormerlySerializedAs("Filler")]
		[SerializeField]
		public LayoutElement Indentation;

		/// <summary>
		/// Gets or sets the indentation.
		/// </summary>
		/// <value>The indentation.</value>
		[Obsolete("Use Indentation instead.")]
		public LayoutElement Filler
		{
			get => Indentation;

			set => Indentation = value;
		}

		/// <summary>
		/// The on node expand.
		/// </summary>
		[SerializeField]
		public NodeToggle OnNodeExpand = NodeToggle.Rotate;

		/// <summary>
		/// Is need animate arrow?
		/// </summary>
		[SerializeField]
		public bool AnimateArrow;

		/// <summary>
		/// Arrow animation length.
		/// </summary>
		[SerializeField]
		public float ArrowAnimationLength = 0.2f;

		[SerializeField]
		[FormerlySerializedAs("NodeOpened")]
		Sprite nodeOpened;

		/// <summary>
		/// Sprite when node opened.
		/// </summary>
		public Sprite NodeOpened
		{
			get => nodeOpened;

			set
			{
				nodeOpened = value;

				SetToggleSprite(IsExpanded);
			}
		}

		[SerializeField]
		[FormerlySerializedAs("NodeClosed")]
		Sprite nodeClosed;

		/// <summary>
		/// Sprite when node closed.
		/// </summary>
		public Sprite NodeClosed
		{
			get => nodeClosed;

			set
			{
				nodeClosed = value;

				SetToggleSprite(IsExpanded);
			}
		}

		/// <summary>
		/// The padding per level.
		/// </summary>
		[SerializeField]
		public float PaddingPerLevel = 30;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		[SerializeField]
		public bool SetNativeSize = true;

		/// <summary>
		/// Do not change toggle rotation.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public bool IgnoreRotation = false;

		/// <summary>
		/// Node depth.
		/// </summary>
		protected float NodeDepth
		{
			get;
			set;
		}

		/// <summary>
		/// Is node expanded?
		/// </summary>
		protected abstract bool IsExpanded
		{
			get;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (Toggle != null)
			{
				Toggle.OnClick.AddListener(ToggleNode);
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Toggle != null)
			{
				Toggle.OnClick.RemoveListener(ToggleNode);
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Toggles the node.
		/// </summary>
		protected virtual void ToggleNode()
		{
			if (!IsInteractable())
			{
				return;
			}

			var owner = Owner;
			var index = Index;

			ToggleEvent.Invoke(index);

			SetToggle(IsExpanded);

			if (owner != null)
			{
				owner.InstancesEventsInternal.NodeToggleClick.Invoke(index, this);
				owner.InstancesEvents.NodeToggleClick.Invoke(index, this);

				if (AnimationCoroutine != null)
				{
					owner.StopCoroutine(AnimationCoroutine);
					AnimationCoroutine = null;
				}
			}

			if (OnNodeExpand == NodeToggle.Rotate)
			{
				if (!IgnoreRotation && AnimateArrow)
				{
					AnimationCoroutine = IsExpanded ? CloseCoroutine() : OpenCoroutine();
					if (owner != null)
					{
						owner.StartCoroutine(AnimationCoroutine);
					}
				}
			}
			else
			{
				SetToggle(IsExpanded);
			}
		}

		/// <summary>
		/// Set the toggle sprite.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggleSprite(bool isExpanded)
		{
			if (OnNodeExpand == NodeToggle.ChangeSprite)
			{
				ToggleImage.sprite = isExpanded ? NodeOpened : NodeClosed;
			}
		}

		/// <summary>
		/// The animation coroutine.
		/// </summary>
		protected IEnumerator AnimationCoroutine;

		/// <summary>
		/// Animate arrow on open.
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator OpenCoroutine()
		{
			var rect = Toggle.transform as RectTransform;
			return Animations.RotateZ(rect, ArrowAnimationLength, -90, 0);
		}

		/// <summary>
		/// Animate arrow on close.
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator CloseCoroutine()
		{
			var rect = Toggle.transform as RectTransform;
			return Animations.RotateZ(rect, ArrowAnimationLength, 0, -90);
		}

		/// <summary>
		/// Sets the toggle rotation.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggleRotation(bool isExpanded)
		{
			if (IgnoreRotation || (Toggle == null))
			{
				return;
			}

			Toggle.transform.localRotation = Quaternion.Euler(0, 0, isExpanded ? -90 : 0);
		}

		/// <summary>
		/// Sets the toggle.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggle(bool isExpanded)
		{
			if (OnNodeExpand == NodeToggle.Rotate)
			{
				SetToggleRotation(isExpanded);
			}
			else
			{
				SetToggleSprite(isExpanded);
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(Text, ref TextAdapter);
#pragma warning restore 0612, 0618
		}

		/// <inheritdoc/>
		public override void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.SetStyle(styleBackground, styleText, style);

			PaddingPerLevel = style.TreeView.PaddingPerLevel;

			style.TreeView.Toggle.ApplyTo(ToggleImage);

			OnNodeExpand = style.TreeView.OnNodeExpand;

			AnimateArrow = style.TreeView.AnimateArrow;

			NodeOpened = style.TreeView.NodeOpened;

			NodeClosed = style.TreeView.NodeClosed;

			if (TextAdapter != null)
			{
				styleText.ApplyTo(TextAdapter.gameObject);
			}
		}

		/// <inheritdoc/>
		public override void GetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.GetStyle(styleBackground, styleText, style);

			style.TreeView.PaddingPerLevel = Mathf.RoundToInt(PaddingPerLevel);

			style.TreeView.Toggle.GetFrom(ToggleImage);

			style.TreeView.OnNodeExpand = OnNodeExpand;

			style.TreeView.AnimateArrow = AnimateArrow;

			style.TreeView.NodeOpened = NodeOpened;

			style.TreeView.NodeClosed = NodeClosed;

			if (TextAdapter != null)
			{
				styleText.GetFrom(TextAdapter.gameObject);
			}
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Icon, nameof(Icon.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Icon, nameof(Icon.color), owner);
		}
	}
}