namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Draggable restriction.
	/// </summary>
	public enum DraggableRestriction
	{
		/// <summary>
		/// Without restriction.
		/// </summary>
		None = 0,

		/// <summary>
		/// Does not allow drag outside parent.
		/// </summary>
		Strict = 1,

		/// <summary>
		/// Apply restriction after drag.
		/// </summary>
		AfterDrag = 2,
	}

	/// <summary>
	/// Draggable UI object.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Draggable")]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/draggable.html")]
	public class Draggable : UIBehaviourInteractable, ISnapGridSupport
	{
		/// <summary>
		/// The handle.
		/// </summary>
		[SerializeField]
		GameObject handle;

		DragSupportHandle handleDrag;

		/// <summary>
		/// Allow horizontal movement.
		/// </summary>
		[SerializeField]
		public bool Horizontal = true;

		/// <summary>
		/// Allow vertical movement.
		/// </summary>
		[SerializeField]
		public bool Vertical = true;

		/// <summary>
		/// Drag restriction.
		/// </summary>
		public DraggableRestriction Restriction = DraggableRestriction.None;

		/// <summary>
		/// Animation curve.
		/// </summary>
		[SerializeField]
		public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 0.2f, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		public bool UnscaledTime;

		/// <summary>
		/// Handle to drag.
		/// </summary>
		public GameObject Handle
		{
			get => handle;

			set => SetHandle(value);
		}

		[SerializeField]
		[Header("Snap Settings")]
		List<SnapGridBase> snapGrids;

		/// <summary>
		/// SnapGrids.
		/// </summary>
		public List<SnapGridBase> SnapGrids
		{
			get => snapGrids;

			set => snapGrids = value;
		}

		[SerializeField]
		Vector2 snapDistance = new Vector2(10f, 10f);

		/// <summary>
		/// Snap distance.
		/// </summary>
		public Vector2 SnapDistance
		{
			get => snapDistance;

			set => snapDistance = value;
		}

		/// <summary>
		/// Snap mode.
		/// </summary>
		[SerializeField]
		public SnapDragMode SnapMode = SnapDragMode.OnDrag;

		/// <summary>
		/// Restore position on end drag if the snap is not applicable.
		/// </summary>
		[SerializeField]
		[Tooltip("Restore position on end drag if the snap is not applicable.")]
		public bool RestoreIfNotSnap = false;

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>RectTransform.</value>
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
		/// Is target is self?
		/// </summary>
		protected bool IsTargetSelf;

		RectTransform target;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>RectTransform.</value>
		public RectTransform Target
		{
			get
			{
				if (target == null)
				{
					target = transform as RectTransform;
					OnTargetChanged.Invoke(this);
				}

				return target;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				IsTargetSelf = value.GetInstanceID() == transform.GetInstanceID();
				target = value;

				if (!IsTargetSelf)
				{
					var le = Utilities.RequireComponent<LayoutElement>(this);
					le.ignoreLayout = true;

					RectTransform.SetParent(target.parent, false);

					CopyRectTransformValues();
				}

				OnTargetChanged.Invoke(this);
			}
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Start resize event.
		/// </summary>
		[SerializeField]
		public DraggableEvent OnStartDrag = new DraggableEvent();

		/// <summary>
		/// During resize event.
		/// </summary>
		public DraggableEvent OnDrag = new DraggableEvent();

		/// <summary>
		/// End resize event.
		/// </summary>
		[SerializeField]
		public DraggableEvent OnEndDrag = new DraggableEvent();

		/// <summary>
		/// Snap event.
		/// </summary>
		[SerializeField]
		public DraggableSnapEvent OnSnap = new DraggableSnapEvent();

		/// <summary>
		/// Snap event on end drag.
		/// </summary>
		[SerializeField]
		public DraggableSnapEvent OnEndSnap = new DraggableSnapEvent();

		/// <summary>
		/// Target changed event.
		/// </summary>
		public DraggableEvent OnTargetChanged = new DraggableEvent();

		/// <summary>
		/// Is instance dragged?
		/// </summary>
		protected bool IsDrag;

		/// <summary>
		/// The animation.
		/// </summary>
		protected IEnumerator Animation;

		/// <summary>
		/// Start position.
		/// </summary>
		protected Vector3 StartPosition;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (target == null)
			{
				Target = transform as RectTransform;
			}

			SetHandle(handle != null ? handle : gameObject);
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			if (!interactableState)
			{
				EndDrag(null);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			RemoveListeners();
		}

		/// <summary>
		/// Sets the handle.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetHandle(GameObject value)
		{
			if (handle != null)
			{
				RemoveListeners();
				Destroy(handleDrag);
			}

			handle = value;
			handleDrag = Utilities.RequireComponent<DragSupportHandle>(handle);
			AddListeners();
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected void AddListeners()
		{
			if (handleDrag != null)
			{
				handleDrag.OnBeginDragEvent.AddListener(BeginDrag);
				handleDrag.OnDragEvent.AddListener(Drag);
				handleDrag.OnEndDragEvent.AddListener(EndDrag);
			}
		}

		/// <summary>
		/// Remove listeners,
		/// </summary>
		protected void RemoveListeners()
		{
			if (handleDrag != null)
			{
				handleDrag.OnBeginDragEvent.RemoveListener(BeginDrag);
				handleDrag.OnDragEvent.RemoveListener(Drag);
				handleDrag.OnEndDragEvent.RemoveListener(EndDrag);
			}
		}

		/// <summary>
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return IsActive() && (eventData.button == DragButton);
		}

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void BeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			if (Animation != null)
			{
				StopCoroutine(Animation);
			}

			IsDrag = true;
			InitDrag();

			OnStartDrag.Invoke(this);
		}

		/// <summary>
		/// Init drag.
		/// </summary>
		public virtual void InitDrag()
		{
			StartPosition = Target.localPosition;
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void Drag(PointerEventData eventData)
		{
			if (!IsDrag)
			{
				return;
			}

			if (eventData.used)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				EndDrag(eventData);
				return;
			}

			eventData.Use();

			Drag(eventData, false);

			OnDrag.Invoke(this);
		}

		/// <summary>
		/// Perform drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="isEnd">Is end drag?</param>
		public virtual void Drag(PointerEventData eventData, bool isEnd)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.position, eventData.pressEventCamera, out var current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.pressPosition, eventData.pressEventCamera, out var original_point);

			var base_delta = current_point - original_point;
			Drag(base_delta, isEnd);
		}

		/// <summary>
		/// Perform drag.
		/// </summary>
		/// <param name="delta">Delta.</param>
		/// <param name="isEnd">Is end drag?</param>
		public virtual void Drag(Vector2 delta, bool isEnd = false)
		{
			delta.y *= -1f;

			var angle_rad = Target.localRotation.eulerAngles.z * Mathf.Deg2Rad;
			var drag_delta = new Vector2(
				(delta.x * Mathf.Cos(angle_rad)) + (delta.y * Mathf.Sin(angle_rad)),
				(delta.x * Mathf.Sin(angle_rad)) - (delta.y * Mathf.Cos(angle_rad)));

			if (!Horizontal)
			{
				drag_delta.x = 0;
			}

			if (!Vertical)
			{
				drag_delta.y = 0;
			}

			var new_position = new Vector3(
				StartPosition.x + drag_delta.x,
				StartPosition.y + drag_delta.y,
				StartPosition.z);

			if (Restriction == DraggableRestriction.Strict)
			{
				new_position = RestrictPosition(new_position);
			}

			Target.localPosition = new_position;

			var apply_snap = (SnapMode == SnapDragMode.OnDrag)
				|| (isEnd && (SnapMode == SnapDragMode.OnEndDrag));
			if (apply_snap && (SnapGrids != null))
			{
				var snap = SnapGridBase.Snap(SnapGrids, SnapDistance, Target);
				var restore_position = isEnd && RestoreIfNotSnap && !snap.Snapped;
				if (restore_position)
				{
					Target.localPosition = StartPosition;
				}
				else
				{
					Target.localPosition += new Vector3(snap.Delta.x, snap.Delta.y, 0);
				}

				if (isEnd)
				{
					OnEndSnap.Invoke(this, snap);
				}
				else
				{
					OnSnap.Invoke(this, snap);
				}
			}

			CopyRectTransformValues();
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void EndDrag(PointerEventData eventData)
		{
			if (!IsDrag)
			{
				return;
			}

			if (eventData.used)
			{
				return;
			}

			eventData.Use();

			if (Restriction == DraggableRestriction.AfterDrag)
			{
				Animation = AnimationCoroutine();
				StartCoroutine(Animation);
			}
			else
			{
				Drag(eventData, true);
			}

			IsDrag = false;

			OnEndDrag.Invoke(this);
		}

		/// <summary>
		/// Animation coroutine.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected IEnumerator AnimationCoroutine()
		{
			var start_pos = Target.localPosition;
			var end_pos = RestrictPosition(Target.localPosition);
			if (start_pos != end_pos)
			{
				var duration = Curve[Curve.length - 1].time;
				var time = 0f;

				do
				{
					Target.localPosition = Vector3.Lerp(start_pos, end_pos, Curve.Evaluate(time));
					CopyRectTransformValues();
					yield return null;

					time += UtilitiesTime.GetDeltaTime(UnscaledTime);
				}
				while (time < duration);

				Target.localPosition = end_pos;
				CopyRectTransformValues();
			}
		}

		/// <summary>
		/// Restrict the position.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
		protected virtual Vector3 RestrictPosition(Vector3 pos)
		{
			var parent = Target.parent as RectTransform;
			var parent_pivot = parent.pivot;
			var parent_size = parent.rect.size;
			var target_size = Target.rect.size;
			var target_pivot = Target.pivot;

			var min_x = -(parent_size.x * parent_pivot.x) + (target_size.x * target_pivot.x);
			var max_x = (parent_size.x * (1f - parent_pivot.x)) - (target_size.x * (1f - target_pivot.x));

			var min_y = -(parent_size.y * parent_pivot.y) + (target_size.y * target_pivot.y);
			var max_y = (parent_size.y * (1f - parent_pivot.y)) - (target_size.y * (1f - target_pivot.y));

			return new Vector3(
				Mathf.Clamp(pos.x, min_x, max_x),
				Mathf.Clamp(pos.y, min_y, max_y),
				pos.z);
		}

		/// <summary>
		/// Copy RectTransform values.
		/// </summary>
		protected void CopyRectTransformValues()
		{
			if (!IsTargetSelf)
			{
				UtilitiesRectTransform.CopyValues(Target, RectTransform);
			}
		}
	}
}