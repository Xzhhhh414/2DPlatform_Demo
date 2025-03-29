namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Splitter type.
	/// </summary>
	public enum SplitterType
	{
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Splitter mode.
	/// </summary>
	public enum SplitterMode
	{
		/// <summary>
		/// Auto mode. Use previous and next siblings in hierarchy.
		/// </summary>
		Auto = 0,

		/// <summary>
		/// Manual mode. Use specified targets to resize.
		/// </summary>
		Manual = 1,
	}

	/// <summary>
	/// Splitter.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Splitter")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/splitter.html")]
	public class Splitter : UIBehaviourInteractable,
		IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler, ILateUpdatable
	{
		/// <summary>
		/// Target.
		/// </summary>
		protected struct Target
		{
			/// <summary>
			/// RectTransform.
			/// </summary>
			public readonly RectTransform RectTransform;

			/// <summary>
			/// LayoutElement.
			/// </summary>
			public readonly LayoutElement LayoutElement;

			/// <summary>
			/// SplitterMaxSize.
			/// </summary>
			public readonly SplitterMaxSize SplitterMaxSize;

			/// <summary>
			/// Maximum size.
			/// </summary>
			public readonly float MaxSize => SplitterMaxSize != null ? SplitterMaxSize.Value : 0f;

			/// <summary>
			/// Base size.
			/// </summary>
			public Vector2 BaseSize
			{
				get;
				private set;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Target"/> struct.
			/// </summary>
			/// <param name="rectTransform">RectTransform.</param>
			public Target(RectTransform rectTransform)
			{
				RectTransform = rectTransform;
				LayoutElement = Utilities.RequireComponent<LayoutElement>(RectTransform);
				RectTransform.TryGetComponent<SplitterMaxSize>(out SplitterMaxSize);
				BaseSize = Vector2.zero;
				SetLayoutSize();
			}

			/// <summary>
			/// Set layout size.
			/// </summary>
			public void SetLayoutSize()
			{
				BaseSize = RectTransform.rect.size;
				LayoutElement.preferredWidth = BaseSize.x;
				LayoutElement.preferredHeight = BaseSize.y;
			}

			/// <summary>
			/// Is RectTransform is same as the specified one?
			/// </summary>
			/// <param name="rectTransform">RectTransform.</param>
			/// <returns>true if RectTransform is same as the specified one; otherwise false.</returns>
			public readonly bool Same(RectTransform rectTransform) => (RectTransform != null) && (RectTransform.GetInstanceID() == rectTransform.GetInstanceID());

			readonly (float Current, float Sibling) CalculateWidth(float delta, Target sibling, bool integerSize)
			{
				var total = BaseSize.x + sibling.BaseSize.x;
				var current_width = Mathf.Min(BaseSize.x + delta, total - sibling.LayoutElement.minWidth);
				if (current_width < LayoutElement.minWidth)
				{
					current_width = LayoutElement.minWidth;
				}

				if (integerSize)
				{
					current_width = Mathf.Round(current_width);
				}

				var sibling_width = total - current_width;

				return ValidateMax(sibling, current_width, sibling_width);
			}

			readonly (float Current, float Sibling) CalculateHeight(float delta, Target sibling, bool integerSize)
			{
				var total = BaseSize.y + sibling.BaseSize.y;
				var current_height = Mathf.Min(BaseSize.y + delta, total - sibling.LayoutElement.minHeight);
				if (current_height < LayoutElement.minHeight)
				{
					current_height = LayoutElement.minHeight;
				}

				if (integerSize)
				{
					current_height = Mathf.Round(current_height);
				}

				var sibling_height = total - current_height;

				return ValidateMax(sibling, current_height, sibling_height);
			}

			readonly (float Current, float Sibling) ValidateMax(Target sibling, float current_size, float sibling_size)
			{
				var current_max = MaxSize;
				var sibling_max = sibling.MaxSize;
				var total = current_size + sibling_size;

				if ((current_max > 0f) && (current_size > current_max))
				{
					current_size = current_max;
					sibling_size = total - current_size;
				}

				if ((sibling_max > 0f) && (sibling_size > sibling_max))
				{
					sibling_size = sibling_max;
					current_size = total - sibling_size;
				}

				return (current_size, sibling_size);
			}

			/// <summary>
			/// Change RectTransform width.
			/// </summary>
			/// <param name="delta">Delta.</param>
			/// <param name="sibling">Sibling target.</param>
			/// <param name="integerSize">Integer size.</param>
			/// <param name="updateRectTransform">Update RectTransform height.</param>
			/// <param name="updateLayoutElement">Update LayoutElement height.</param>
			public readonly void ChangeWidth(float delta, Target sibling, bool integerSize, bool updateRectTransform, bool updateLayoutElement)
			{
				(var current_width, var sibling_width) = CalculateWidth(delta, sibling, integerSize);

				if (updateRectTransform)
				{
					RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current_width);
					sibling.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sibling_width);
				}

				if (updateLayoutElement)
				{
					LayoutElement.preferredWidth = current_width;
					sibling.LayoutElement.preferredWidth = sibling_width;
				}
			}

			/// <summary>
			/// Change height.
			/// </summary>
			/// <param name="delta">Delta.</param>
			/// <param name="sibling">Sibling target.</param>
			/// <param name="integerSize">Integer size.</param>
			/// <param name="updateRectTransform">Update RectTransform height.</param>
			/// <param name="updateLayoutElement">Update LayoutElement height.</param>
			public readonly void ChangeHeight(float delta, Target sibling, bool integerSize, bool updateRectTransform, bool updateLayoutElement)
			{
				(var current_height, var sibling_height) = CalculateHeight(delta, sibling, integerSize);

				if (updateRectTransform)
				{
					RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, current_height);
					sibling.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sibling_height);
				}

				if (updateLayoutElement)
				{
					LayoutElement.preferredHeight = current_height;
					sibling.LayoutElement.preferredHeight = sibling_height;
				}
			}
		}

		/// <summary>
		/// Allow resize.
		/// </summary>
		[Obsolete("Replaced with Interactable.")]
		public bool AllowResize
		{
			get => Interactable;

			set => Interactable = value;
		}

		/// <summary>
		/// The type.
		/// </summary>
		public SplitterType Type = SplitterType.Vertical;

		/// <summary>
		/// Is need to update RectTransform on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateRectTransforms = true;

		/// <summary>
		/// Is need to update LayoutElement on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateLayoutElements = true;

		/// <summary>
		/// The current camera. For Screen Space - Overlay let it empty.
		/// </summary>
		[NonSerialized]
		protected Camera CurrentCamera;

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorTexture;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// Start resize event.
		/// </summary>
		public SplitterResizeEvent OnStartResize = new SplitterResizeEvent();

		/// <summary>
		/// During resize event.
		/// </summary>
		public SplitterResizeEvent OnResize = new SplitterResizeEvent();

		/// <summary>
		/// End resize event.
		/// </summary>
		public SplitterResizeEvent OnEndResize = new SplitterResizeEvent();

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>The RectTransform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Mode.
		/// </summary>
		[SerializeField]
		protected SplitterMode Mode = SplitterMode.Auto;

		[SerializeField]
		[FormerlySerializedAs("leftTarget")]
		[FormerlySerializedAs("PreviousObject")]
		[EditorConditionEnum(nameof(Mode), (int)SplitterMode.Manual)]
		RectTransform previousObject;

		/// <summary>
		/// Previous object.
		/// </summary>
		public RectTransform PreviousObject
		{
			get => previousObject;
			protected set => previousObject = value;
		}

		/// <summary>
		/// Next object.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("rightTarget")]
		[FormerlySerializedAs("NextObject")]
		[EditorConditionEnum(nameof(Mode), (int)SplitterMode.Manual)]
		RectTransform nextObject;

		/// <summary>
		/// Previous object.
		/// </summary>
		public RectTransform NextObject
		{
			get => nextObject;
			protected set => nextObject = value;
		}

		Target previousTarget;

		/// <summary>
		/// Previous target.
		/// </summary>
		protected ref Target PreviousTarget
		{
			get
			{
				if (!previousTarget.Same(PreviousObject))
				{
					previousTarget = new Target(PreviousObject);
				}

				return ref previousTarget;
			}
		}

		/// <summary>
		/// LayoutElement of the previous target.
		/// </summary>
		public LayoutElement PreviousLayoutElement => PreviousTarget.LayoutElement;

		Target nextTarget;

		/// <summary>
		/// Next target.
		/// </summary>
		protected ref Target NextTarget
		{
			get
			{
				if (!nextTarget.Same(NextObject))
				{
					nextTarget = new Target(NextObject);
				}

				return ref nextTarget;
			}
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Integer size.
		/// </summary>
		[SerializeField]
		public bool IntegerSize = true;

		/// <summary>
		/// Cursors.
		/// </summary>
		[SerializeField]
		public Cursors Cursors;

		/// <summary>
		/// LayoutElement of the next target.
		/// </summary>
		public LayoutElement NextLayoutElement => NextTarget.LayoutElement;

		bool processDrag;

		bool cursorChanged;

		/// <summary>
		/// Is cursor over?
		/// </summary>
		protected bool IsCursorOver;

		/// <summary>
		/// Can change cursor?
		/// </summary>
		protected bool CanChangeCursor => UICursor.CanSet(this) && CompatibilityInput.MousePresent && IsCursorOver;

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			if (!interactableState && IsCursorOver)
			{
				IsCursorOver = false;

				if (!processDrag)
				{
					ResetCursor();
				}
			}
		}

		/// <summary>
		/// Process the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (transform.parent.TryGetComponent<LayoutGroup>(out var layout))
			{
				LayoutUtilities.UpdateLayout(layout);
			}

			using var _ = ListPool<Splitter>.Get(out var splitters);

			transform.parent.GetComponentsInChildren(splitters);

			foreach (var splitter in splitters)
			{
				InitSizes(splitter);
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

#pragma warning disable 0618
			if (DefaultCursorTexture != null)
			{
				UICursor.ObsoleteWarning();
			}
#pragma warning restore 0618
		}

		/// <summary>
		/// Init splitter sizes.
		/// </summary>
		/// <param name="splitter">Splitter.</param>
		protected virtual void InitSizes(Splitter splitter)
		{
			splitter.InitSizes();
		}

		/// <summary>
		/// Init sizes.
		/// </summary>
		protected void InitSizes()
		{
			var index = transform.GetSiblingIndex();

			if (index == 0 || transform.parent.childCount == (index + 1))
			{
				return;
			}

			if (Mode == SplitterMode.Auto)
			{
				PreviousObject = transform.parent.GetChild(index - 1) as RectTransform;
				NextObject = transform.parent.GetChild(index + 1) as RectTransform;
			}

			PreviousTarget.SetLayoutSize();
			NextTarget.SetLayoutSize();

			if ((PreviousTarget.MaxSize > 0f) && (NextTarget.MaxSize > 0f))
			{
				var total = !IsHorizontal()
					? PreviousTarget.BaseSize.x + NextTarget.BaseSize.x
					: PreviousTarget.BaseSize.y + NextTarget.BaseSize.y;
				var max = PreviousTarget.MaxSize + NextTarget.MaxSize;
				if (max < total)
				{
					var message = string.Format(
						"Total size limit (SplitterMaxSize: {0} + {1} = {2}) should not be less than total size ({3})",
						PreviousTarget.MaxSize,
						NextTarget.MaxSize,
						max,
						total);
					Debug.LogWarning(message, this);
				}
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerEnter event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			IsCursorOver = true;
			CurrentCamera = eventData.pressEventCamera;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerExit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOver = false;

			if (!processDrag)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Reset cursor.
		/// </summary>
		protected void ResetCursor()
		{
			cursorChanged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// Get cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetCursor()
		{
			if (Cursors != null)
			{
				return IsHorizontal() ? Cursors.NorthSouthArrow : Cursors.EastWestArrow;
			}

			if (UICursor.Cursors != null)
			{
				return IsHorizontal() ? UICursor.Cursors.NorthSouthArrow : UICursor.Cursors.EastWestArrow;
			}

			return default;
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Update cursor.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (!IsActive())
			{
				return;
			}

			if (!CanChangeCursor)
			{
				return;
			}

			if (CompatibilityInput.MouseLeftButtonPressed)
			{
				return;
			}

			if (processDrag)
			{
				return;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, CompatibilityInput.MousePosition, CurrentCamera, out var point);

			var rect = RectTransform.rect;
			if (rect.Contains(point))
			{
				cursorChanged = true;
				UICursor.Set(this, GetCursor());
			}
			else if (cursorChanged)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData) => IsActive() && (eventData.button == DragButton);

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			processDrag = false;

			var index = transform.GetSiblingIndex();
			if (index == 0 || transform.parent.childCount == (index + 1))
			{
				return;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out var point);

			UICursor.Set(this, GetCursor());
			cursorChanged = true;

			processDrag = true;

			if (Mode == SplitterMode.Auto)
			{
				PreviousObject = transform.parent.GetChild(index - 1) as RectTransform;
				NextObject = transform.parent.GetChild(index + 1) as RectTransform;
			}

			PreviousTarget.SetLayoutSize();
			NextTarget.SetLayoutSize();

			OnStartResize.Invoke(this);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			ResetCursor();

			if (processDrag)
			{
				processDrag = false;

				OnEndResize.Invoke(this);
			}
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out var current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out var original_point);

			var delta = current_point - original_point;

			if (!IsHorizontal())
			{
				PreviousTarget.ChangeWidth(delta.x, NextTarget, IntegerSize, UpdateRectTransforms, UpdateLayoutElements);
			}
			else
			{
				PreviousTarget.ChangeHeight(-delta.y, NextTarget, IntegerSize, UpdateRectTransforms, UpdateLayoutElements);
			}

			OnResize.Invoke(this);
		}

		/// <summary>
		/// Is horizontal direction?
		/// </summary>
		/// <returns>true if direction is horizontal; otherwise false.</returns>
		protected bool IsHorizontal() => Type == SplitterType.Horizontal;

		#if UNITY_EDITOR

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			CursorsDPISelector.Require(this);

			base.Reset();
		}

		#endif
	}
}