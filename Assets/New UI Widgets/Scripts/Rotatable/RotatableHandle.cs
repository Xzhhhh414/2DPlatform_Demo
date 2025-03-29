namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;

	/// <summary>
	/// Handle for the Rotatable component.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rotatable))]
	[AddComponentMenu("UI/New UI Widgets/Interactions/Rotatable Handle")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/rotatable-handle.html")]
	public class RotatableHandle : UIBehaviourInteractable
	{
		/// <summary>
		/// Use own handle or handle from the specified source.
		/// </summary>
		[SerializeField]
		public bool OwnHandle = true;

		/// <summary>
		/// Handle source.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(OwnHandle), false)]
		public RotatableHandle HandleSource;

		/// <summary>
		/// Handle.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("handler")]
		[EditorConditionBool(nameof(OwnHandle))]
		protected DragListener handle;

		/// <summary>
		/// Handle.
		/// </summary>
		public DragListener Handle
		{
			get => handle;

			set
			{
				if (handle != null)
				{
					handle.OnBeginDragEvent.RemoveListener(OnBeginDrag);
					handle.OnDragEvent.RemoveListener(OnDrag);
					handle.OnEndDragEvent.RemoveListener(OnEndDrag);

					handle.OnSelectEvent.RemoveListener(ShowHandle);
					handle.OnDeselectEvent.RemoveListener(HideHandle);
				}

				handle = value;

				if (handle != null)
				{
					handle.OnBeginDragEvent.AddListener(OnBeginDrag);
					handle.OnDragEvent.AddListener(OnDrag);
					handle.OnEndDragEvent.AddListener(OnEndDrag);

					handle.OnSelectEvent.AddListener(ShowHandle);
					handle.OnDeselectEvent.AddListener(HideHandle);
				}
			}
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Is currently dragged?
		/// </summary>
		protected bool IsDrag;

		/// <summary>
		/// Target.
		/// </summary>
		protected Rotatable Target;

		/// <summary>
		/// Target listeners for focus and focus out events (select/deselect).
		/// </summary>
		protected SelectListener TargetFocusListener;

		/// <summary>
		/// Start rotation event.
		/// </summary>
		[SerializeField]
		public RotatableEvent OnStartRotate = new RotatableEvent();

		/// <summary>
		/// During rotation event.
		/// </summary>
		[SerializeField]
		public RotatableEvent OnRotate = new RotatableEvent();

		/// <summary>
		/// End rotation event.
		/// </summary>
		[SerializeField]
		public RotatableEvent OnEndRotate = new RotatableEvent();

		/// <summary>
		/// Return handle state (enabled/disabled) on select/deselect event (got or lost focus).
		/// Use case: show Rotatable and Resizable handles only if target (or one of handles) is selected, otherwise deselect.
		/// </summary>
		public Func<RotatableHandle, BaseEventData, bool, bool> HandleState = null;

		/// <summary>
		/// Interactable state of the handle source.
		/// </summary>
		protected bool HandleSourceInteractable;

		/// <summary>
		/// Show Rotatable handle only if target (or one of handles) is selected.
		/// </summary>
		[DomainReloadExclude]
		public static readonly Func<RotatableHandle, BaseEventData, bool, bool> ShowHandleOnSelect = (rotatableHandle, eventData, select) =>
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

			return rotatableHandle.IsControlled(ev.pointerEnter);
		};

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out Target);
			Target.Interactable = false;
			Target.OnTargetChanged.AddListener(TargetChanged);

			Handle = handle;
			TargetChanged(Target);
		}

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
			return Utilities.IsSame(id, Target.gameObject)
				|| Utilities.IsSame(id, Handle != null ? Handle.gameObject : null);
		}

		/// <summary>
		/// Process target changed.
		/// </summary>
		/// <param name="rotatable">Rotatable.</param>
		protected void TargetChanged(Rotatable rotatable)
		{
			if (TargetFocusListener != null)
			{
				TargetFocusListener.onSelect.RemoveListener(ShowHandle);
				TargetFocusListener.onDeselect.RemoveListener(HideHandle);
			}

			TargetFocusListener = Utilities.RequireComponent<SelectListener>(rotatable.Target);
			TargetFocusListener.onSelect.AddListener(ShowHandle);
			TargetFocusListener.onDeselect.AddListener(HideHandle);
			ToggleHandle();
		}

		/// <summary>
		/// Toggle handle.
		/// </summary>
		protected virtual void ToggleHandle()
		{
			if (HandleState != null)
			{
				ToggleHandle((TargetFocusListener != null) && IsControlled(EventSystem.current.currentSelectedGameObject));
			}
			else
			{
				ToggleHandle(true);
			}
		}

		/// <summary>
		/// Show handle.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void ShowHandle(BaseEventData eventData)
		{
			if (HandleState == null)
			{
				return;
			}

			ToggleHandle(HandleState(this, eventData, true));
		}

		/// <summary>
		/// Hide handle.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void HideHandle(BaseEventData eventData)
		{
			if (HandleState == null)
			{
				return;
			}

			ToggleHandle(HandleState(this, eventData, false));
		}

		/// <summary>
		/// Toggle handle.
		/// </summary>
		/// <param name="active">Activate or deactivate handle.</param>
		public virtual void ToggleHandle(bool active)
		{
			if (Handle == null)
			{
				return;
			}

			Handle.gameObject.SetActive(active);
		}

		/// <summary>
		/// Get source handle.
		/// </summary>
		public void GetSourceHandle()
		{
			HandleSourceInteractable = HandleSource.interactable;
			HandleSource.interactable = false;
			SetHandleParent(HandleSource, transform);

			Handle = HandleSource.Handle;
		}

		/// <summary>
		/// Release source handle.
		/// </summary>
		public void ReleaseSourceHandle()
		{
			HandleSource.interactable = HandleSourceInteractable;
			HandleSource.Init();
			SetHandleParent(HandleSource, HandleSource.transform);
		}

		/// <summary>
		/// Set new parent to the handle.
		/// </summary>
		/// <param name="rotatableHandle">Rotatable handle.</param>
		/// <param name="parent">Parent.</param>
		protected static void SetHandleParent(RotatableHandle rotatableHandle, Transform parent)
		{
			if (rotatableHandle.Handle != null)
			{
				rotatableHandle.Handle.transform.SetParent(parent, false);
			}
		}

		/// <summary>
		/// Enable handle.
		/// </summary>
		public void EnableHandle()
		{
			Init();

			if (Handle != null)
			{
				Handle.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Disable handle.
		/// </summary>
		public void DisableHandle()
		{
			if (Handle != null)
			{
				Handle.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Get angle between handle and target.
		/// </summary>
		/// <returns>Angle.</returns>
		protected virtual float GetAngle()
		{
			var handle_rt = handle.transform as RectTransform;
			var rt = transform as RectTransform;

			var rotation = rt.localRotation;
			rt.localRotation = Quaternion.Euler(Vector3.zero);

			var relative = GetCenter(rt) - GetCenter(handle_rt);
			relative.x *= -1f;

			var angle = Rotatable.Point2Angle(relative);

			handle_rt.localRotation = rotation;

			return angle;
		}

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return IsActive() && (eventData.button == DragButton);
		}

		/// <summary>
		/// Process begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			IsDrag = true;

			var angle = GetAngle();
			Target.InitRotate(angle);
			OnStartRotate.Invoke(Target);
		}

		/// <summary>
		/// Get center of the specified RectTransform.
		/// </summary>
		/// <param name="rt">RectTransform.</param>
		/// <returns>Center.</returns>
		protected static Vector2 GetCenter(RectTransform rt)
		{
			var pos = rt.position;
			var size = rt.rect.size;
			var pivot = rt.pivot;
			var center = new Vector2(pos.x - (size.x * (pivot.x - 0.5f)), pos.y - (size.y * (pivot.y - 0.5f)));

			return center;
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

			OnEndRotate.Invoke(Target);
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

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			Target.Rotate(eventData);
			OnRotate.Invoke(Target);
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Target != null)
			{
				Target.OnTargetChanged.RemoveListener(TargetChanged);
			}

			if (TargetFocusListener != null)
			{
				TargetFocusListener.onSelect.RemoveListener(ShowHandle);
				TargetFocusListener.onDeselect.RemoveListener(HideHandle);
			}

			Handle = null;

			base.OnDestroy();
		}
	}
}