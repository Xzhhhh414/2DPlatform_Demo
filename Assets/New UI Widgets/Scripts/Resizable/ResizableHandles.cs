namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Handles for the Resizable component.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Resizable))]
	[AddComponentMenu("UI/New UI Widgets/Interactions/Resizable Handles")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/resizable-handles.html")]
	public class ResizableHandles : UIBehaviourInteractable
	{
		/// <summary>
		/// ResizableHandle.
		/// </summary>
		protected class ResizableHandle
		{
			/// <summary>
			/// Owner.
			/// </summary>
			protected ResizableHandles Owner;

			/// <summary>
			/// Regions.
			/// </summary>
			protected Resizable.Regions Regions;

			/// <summary>
			/// Handle.
			/// </summary>
			protected DragListener Handle;

			/// <summary>
			/// RectTransform.
			/// </summary>
			protected RectTransform RectTransform;

			/// <summary>
			/// Is currently dragged?
			/// </summary>
			protected bool IsDrag;

			/// <summary>
			/// Initializes a new instance of the <see cref="ResizableHandle"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="regions">Regions.</param>
			/// <param name="handle">Handle.</param>
			public ResizableHandle(ResizableHandles owner, Resizable.Regions regions, DragListener handle)
			{
				Owner = owner;
				Regions = regions;
				Handle = handle;
				RectTransform = handle.transform as RectTransform;

				Handle.OnBeginDragEvent.AddListener(OnBeginDrag);
				Handle.OnDragEvent.AddListener(OnDrag);
				Handle.OnEndDragEvent.AddListener(OnEndDrag);

				Handle.OnSelectEvent.AddListener(owner.ShowHandles);
				Handle.OnDeselectEvent.AddListener(owner.HideHandles);
			}

			/// <summary>
			/// Process begin drag event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			protected virtual void OnBeginDrag(PointerEventData eventData)
			{
				if (!Owner.CanDrag(eventData))
				{
					return;
				}

				IsDrag = true;
				Owner.Target.InitResize();

				Owner.OnStartResize.Invoke(Owner.Target);
			}

			/// <summary>
			/// Process end drag event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			protected virtual void OnEndDrag(PointerEventData eventData)
			{
				if (!IsDrag)
				{
					return;
				}

				IsDrag = false;

				Owner.OnEndResize.Invoke(Owner.Target);
			}

			/// <summary>
			/// Process drag event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			protected virtual void OnDrag(PointerEventData eventData)
			{
				if (!IsDrag)
				{
					return;
				}

				if (!Owner.CanDrag(eventData))
				{
					OnEndDrag(eventData);
					return;
				}

				RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out var current_point);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out var original_point);

				var delta = current_point - original_point;
				Owner.Target.Resize(Regions, delta);
				Owner.OnResize.Invoke(Owner.Target, Regions, delta);
			}

			/// <summary>
			/// Process destroy event.
			/// </summary>
			public void Destroy()
			{
				if (Handle != null)
				{
					Handle.OnBeginDragEvent.RemoveListener(OnBeginDrag);
					Handle.OnDragEvent.RemoveListener(OnDrag);
					Handle.OnEndDragEvent.RemoveListener(OnEndDrag);

					Handle.OnSelectEvent.RemoveListener(Owner.ShowHandles);
					Handle.OnDeselectEvent.RemoveListener(Owner.HideHandles);
				}

				Owner = null;
				Handle = null;
				RectTransform = null;
			}
		}

		/// <summary>
		/// Use own handles or handles from the specified source.
		/// </summary>
		[SerializeField]
		public bool OwnHandles = true;

		/// <summary>
		/// Handles source.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles), false)]
		public ResizableHandles HandlesSource;

		/// <summary>
		/// Top left drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener topLeft;

		/// <summary>
		/// Top left drag handle.
		/// </summary>
		public DragListener TopLeft
		{
			get => topLeft;

			set
			{
				topLeft = value;
				CreateHandle(ref topLeftHandle, new Resizable.Regions() { Top = true, Left = true }, topLeft);
			}
		}

		/// <summary>
		/// Top center drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener topCenter;

		/// <summary>
		/// Top center drag handle.
		/// </summary>
		public DragListener TopCenter
		{
			get => topCenter;

			set
			{
				topCenter = value;
				CreateHandle(ref topCenterHandle, new Resizable.Regions() { Top = true, }, topCenter);
			}
		}

		/// <summary>
		/// Top right drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener topRight;

		/// <summary>
		/// Top right drag handle.
		/// </summary>
		public DragListener TopRight
		{
			get => topRight;

			set
			{
				topRight = value;
				CreateHandle(ref topRightHandle, new Resizable.Regions() { Top = true, Right = true }, topRight);
			}
		}

		/// <summary>
		/// Middle left drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener middleLeft;

		/// <summary>
		/// Middle left drag handle.
		/// </summary>
		public DragListener MiddleLeft
		{
			get => middleLeft;

			set
			{
				middleLeft = value;
				CreateHandle(ref middleLeftHandle, new Resizable.Regions() { Left = true }, middleLeft);
			}
		}

		/// <summary>
		/// Middle right drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener middleRight;

		/// <summary>
		/// Middle right drag handle.
		/// </summary>
		public DragListener MiddleRight
		{
			get => middleRight;

			set
			{
				middleRight = value;
				CreateHandle(ref middleRightHandle, new Resizable.Regions() { Right = true }, middleRight);
			}
		}

		/// <summary>
		/// Bottom left drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener bottomLeft;

		/// <summary>
		/// Bottom left drag handle.
		/// </summary>
		public DragListener BottomLeft
		{
			get => bottomLeft;

			set
			{
				bottomLeft = value;
				CreateHandle(ref bottomLeftHandle, new Resizable.Regions() { Bottom = true, Left = true }, bottomLeft);
			}
		}

		/// <summary>
		/// Bottom center drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener bottomCenter;

		/// <summary>
		/// Bottom center drag handle.
		/// </summary>
		public DragListener BottomCenter
		{
			get => bottomCenter;

			set
			{
				bottomCenter = value;
				CreateHandle(ref bottomCenterHandle, new Resizable.Regions() { Bottom = true, }, bottomCenter);
			}
		}

		/// <summary>
		/// Bottom right drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandles))]
		protected DragListener bottomRight;

		/// <summary>
		/// Bottom right drag handle.
		/// </summary>
		public DragListener BottomRight
		{
			get => bottomRight;

			set
			{
				bottomRight = value;
				CreateHandle(ref bottomRightHandle, new Resizable.Regions() { Bottom = true, Right = true }, bottomRight);
			}
		}

		/// <summary>
		/// Top left handle.
		/// </summary>
		protected ResizableHandle topLeftHandle;

		/// <summary>
		/// Top center handle.
		/// </summary>
		protected ResizableHandle topCenterHandle;

		/// <summary>
		/// Top right handle.
		/// </summary>
		protected ResizableHandle topRightHandle;

		/// <summary>
		/// Middle left handle.
		/// </summary>
		protected ResizableHandle middleLeftHandle;

		/// <summary>
		/// Middle right handle.
		/// </summary>
		protected ResizableHandle middleRightHandle;

		/// <summary>
		/// Bottom left handle.
		/// </summary>
		protected ResizableHandle bottomLeftHandle;

		/// <summary>
		/// Bottom center handle.
		/// </summary>
		protected ResizableHandle bottomCenterHandle;

		/// <summary>
		/// Bottom right handle.
		/// </summary>
		protected ResizableHandle bottomRightHandle;

		/// <summary>
		/// Target.
		/// </summary>
		protected Resizable Target;

		/// <summary>
		/// Target listeners for focus and focus out events (select/deselect).
		/// </summary>
		protected SelectListener TargetFocusListener;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// On start resize event.
		/// </summary>
		[SerializeField]
		public ResizableEvent OnStartResize = new ResizableEvent();

		/// <summary>
		/// On resize event.
		/// </summary>
		[SerializeField]
		public ResizableDeltaEvent OnResize = new ResizableDeltaEvent();

		/// <summary>
		/// On end resize event.
		/// </summary>
		[SerializeField]
		public ResizableEvent OnEndResize = new ResizableEvent();

		/// <summary>
		/// Return handle state (enabled/disabled) on select/deselect event (got or lost focus).
		/// Use case: show Rotatable and Resizable handles only if target (or one of handles) is selected, otherwise deselect.
		/// </summary>
		public Func<ResizableHandles, BaseEventData, bool, bool> HandlesState = null;

		/// <summary>
		/// Show Rotatable handle only if target (or one of handles) is selected.
		/// </summary>
		[DomainReloadExclude]
		public static readonly Func<ResizableHandles, BaseEventData, bool, bool> ShowHandlesOnSelect = (resizableHandles, eventData, select) =>
		{
			if (select)
			{
				return true;
			}

			var ev = eventData as PointerEventData;
			if (eventData == null)
			{
				return false;
			}

			return resizableHandles.IsControlled(ev.pointerEnter);
		};

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out Target);
			Target.OnResizeDirectionsChanged.AddListener(SetHandlesState);
			Target.OnTargetChanged.AddListener(TargetChanged);
			Target.Interactable = false;

			TopLeft = topLeft;
			TopCenter = topCenter;
			TopRight = topRight;

			MiddleLeft = middleLeft;
			MiddleRight = middleRight;

			BottomLeft = bottomLeft;
			BottomCenter = bottomCenter;
			BottomRight = bottomRight;

			SetHandlesState(Target);
			TargetChanged(Target);
		}

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData) => IsActive() && (eventData.button == DragButton);

		/// <summary>
		/// Check if specified game object is Target or one of Handles.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <returns>true if specified game object is Target or one of Handles; otherwise false.</returns>
		public virtual bool IsControlled(GameObject go)
		{
			if (go == null)
			{
				return false;
			}

			var id = new InstanceID(go);
			return Utilities.IsSame(id, Target != null ? Target.gameObject : null)
				|| Utilities.IsSame(id, TopLeft != null ? TopLeft.gameObject : null)
				|| Utilities.IsSame(id, TopCenter != null ? TopCenter.gameObject : null)
				|| Utilities.IsSame(id, TopRight != null ? TopRight.gameObject : null)
				|| Utilities.IsSame(id, MiddleLeft != null ? MiddleLeft.gameObject : null)
				|| Utilities.IsSame(id, MiddleRight != null ? MiddleRight.gameObject : null)
				|| Utilities.IsSame(id, BottomLeft != null ? BottomLeft.gameObject : null)
				|| Utilities.IsSame(id, BottomCenter != null ? BottomCenter.gameObject : null)
				|| Utilities.IsSame(id, BottomRight != null ? BottomRight.gameObject : null);
		}

		/// <summary>
		/// Process target changed.
		/// </summary>
		/// <param name="resizable">Resizable.</param>
		protected void TargetChanged(Resizable resizable)
		{
			if (TargetFocusListener != null)
			{
				TargetFocusListener.onSelect.RemoveListener(ShowHandles);
				TargetFocusListener.onDeselect.RemoveListener(HideHandles);
			}

			TargetFocusListener = Utilities.RequireComponent<SelectListener>(resizable.Target);
			TargetFocusListener.onSelect.AddListener(ShowHandles);
			TargetFocusListener.onDeselect.AddListener(HideHandles);
			ToggleHandles();
		}

		/// <summary>
		/// Toggle handles.
		/// </summary>
		protected virtual void ToggleHandles()
		{
			if (HandlesState != null)
			{
				ToggleHandles((TargetFocusListener != null) && IsControlled(EventSystem.current.currentSelectedGameObject));
			}
			else
			{
				ToggleHandles(true);
			}
		}

		/// <summary>
		/// Show handles.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void ShowHandles(BaseEventData eventData)
		{
			if (HandlesState == null)
			{
				return;
			}

			ToggleHandles(HandlesState(this, eventData, true));
		}

		/// <summary>
		/// Hide handles.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void HideHandles(BaseEventData eventData)
		{
			if (HandlesState == null)
			{
				return;
			}

			ToggleHandles(HandlesState(this, eventData, false));
		}

		/// <summary>
		/// Toggle handles.
		/// </summary>
		/// <param name="active">Activate or deactivate handles.</param>
		public virtual void ToggleHandles(bool active)
		{
			ToggleHandle(TopLeft, active);
			ToggleHandle(TopCenter, active);
			ToggleHandle(TopRight, active);

			ToggleHandle(MiddleLeft, active);
			ToggleHandle(MiddleRight, active);

			ToggleHandle(BottomLeft, active);
			ToggleHandle(BottomCenter, active);
			ToggleHandle(BottomRight, active);
		}

		/// <summary>
		/// Toggle handle.
		/// </summary>
		/// <param name="handle">Handle.</param>
		/// <param name="active">Activate or deactivate handle.</param>
		protected virtual void ToggleHandle(DragListener handle, bool active)
		{
			if (handle == null)
			{
				return;
			}

			handle.gameObject.SetActive(active);
		}

		/// <summary>
		/// Set handles state.
		/// </summary>
		/// <param name="resizable">Resizable component.</param>
		protected void SetHandlesState(Resizable resizable)
		{
			var directions = resizable.ResizeDirections;
			SetHandleState(TopLeft, directions.TopLeft);
			SetHandleState(TopCenter, directions.Top);
			SetHandleState(TopRight, directions.TopRight);

			SetHandleState(MiddleLeft, directions.Left);
			SetHandleState(MiddleRight, directions.Right);

			SetHandleState(BottomLeft, directions.BottomLeft);
			SetHandleState(BottomCenter, directions.Bottom);
			SetHandleState(BottomRight, directions.BottomRight);
		}

		/// <summary>
		/// Set handles state.
		/// </summary>
		/// <param name="handle">Handle.</param>
		/// <param name="state">State.</param>
		protected virtual void SetHandleState(DragListener handle, bool state)
		{
			if (handle != null)
			{
				handle.gameObject.SetActive(state);
			}
		}

		/// <summary>
		/// Interactable state of the handles source.
		/// </summary>
		protected bool HandlesSourceInteractable;

		/// <summary>
		/// Get source handles.
		/// </summary>
		public void GetSourceHandles()
		{
			HandlesSourceInteractable = HandlesSource.interactable;
			HandlesSource.interactable = false;
			SetHandlesParent(HandlesSource, transform);

			TopLeft = HandlesSource.TopLeft;
			TopCenter = HandlesSource.TopCenter;
			TopRight = HandlesSource.TopRight;

			MiddleLeft = HandlesSource.MiddleLeft;
			MiddleRight = HandlesSource.MiddleRight;

			BottomLeft = HandlesSource.BottomLeft;
			BottomCenter = HandlesSource.BottomCenter;
			BottomRight = HandlesSource.BottomRight;

			SetHandlesState(Target);
		}

		/// <summary>
		/// Release source handles.
		/// </summary>
		public void ReleaseSourceHandles()
		{
			HandlesSource.interactable = HandlesSourceInteractable;
			HandlesSource.Init();
			SetHandlesParent(HandlesSource, HandlesSource.transform);
			HandlesSource.SetHandlesState(HandlesSource.Target);
		}

		/// <summary>
		/// Set new parent to the handles.
		/// </summary>
		/// <param name="handles">Handles.</param>
		/// <param name="parent">Parent.</param>
		protected static void SetHandlesParent(ResizableHandles handles, Transform parent)
		{
			SetHandleParent(handles.TopLeft, parent);
			SetHandleParent(handles.TopCenter, parent);
			SetHandleParent(handles.TopRight, parent);

			SetHandleParent(handles.MiddleLeft, parent);
			SetHandleParent(handles.MiddleRight, parent);

			SetHandleParent(handles.BottomLeft, parent);
			SetHandleParent(handles.BottomCenter, parent);
			SetHandleParent(handles.BottomRight, parent);
		}

		/// <summary>
		/// Set new parent to the handle.
		/// </summary>
		/// <param name="handle">Handle.</param>
		/// <param name="parent">Parent.</param>
		protected static void SetHandleParent(DragListener handle, Transform parent)
		{
			if (handle != null)
			{
				handle.transform.SetParent(parent, false);
			}
		}

		/// <summary>
		/// Enable handles.
		/// </summary>
		public void EnableHandles()
		{
			Init();

			SetHandlesState(Target);
		}

		/// <summary>
		/// Disable handles.
		/// </summary>
		public void DisableHandles()
		{
			DisableHandle(topLeft);
			DisableHandle(topCenter);
			DisableHandle(topRight);

			DisableHandle(middleLeft);
			DisableHandle(middleRight);

			DisableHandle(bottomLeft);
			DisableHandle(bottomCenter);
			DisableHandle(bottomRight);
		}

		/// <summary>
		/// Disable handle.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public virtual void DisableHandle(DragListener handle)
		{
			if (handle != null)
			{
				handle.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Create handle.
		/// </summary>
		/// <param name="handle">Handle to create.</param>
		/// <param name="regions">Regions.</param>
		/// <param name="drag">Drag handle.</param>
		protected virtual void CreateHandle(ref ResizableHandle handle, Resizable.Regions regions, DragListener drag)
		{
			DestroyHandle(ref handle);

			if (drag == null)
			{
				return;
			}

			handle = new ResizableHandle(this, regions, drag);
		}

		/// <summary>
		/// Destroy handle.
		/// </summary>
		/// <param name="handle">Handle.</param>
		protected virtual void DestroyHandle(ref ResizableHandle handle)
		{
			if (handle == null)
			{
				return;
			}

			handle.Destroy();
			handle = null;
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Target != null)
			{
				Target.OnResizeDirectionsChanged.RemoveListener(SetHandlesState);
				Target.OnTargetChanged.RemoveListener(TargetChanged);
			}

			if (TargetFocusListener != null)
			{
				TargetFocusListener.onSelect.RemoveListener(ShowHandles);
				TargetFocusListener.onDeselect.RemoveListener(HideHandles);
			}

			DestroyHandle(ref topLeftHandle);
			DestroyHandle(ref topCenterHandle);
			DestroyHandle(ref topRightHandle);

			DestroyHandle(ref middleLeftHandle);
			DestroyHandle(ref middleRightHandle);

			DestroyHandle(ref bottomLeftHandle);
			DestroyHandle(ref bottomCenterHandle);
			DestroyHandle(ref bottomRightHandle);

			base.OnDestroy();
		}
	}
}