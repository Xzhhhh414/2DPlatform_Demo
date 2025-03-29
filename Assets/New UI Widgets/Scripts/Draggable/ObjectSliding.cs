namespace UIWidgets
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Object sliding direction.
	/// </summary>
	public enum ObjectSlidingDirection
	{
		/// <summary>
		/// Horizontal direction.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical direction.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Allow to drag objects between specified positions.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Object Sliding")]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/object-sliding.html")]
	public class ObjectSliding : UIBehaviourInteractable, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		/// <summary>
		/// Allowed positions.
		/// </summary>
		public List<float> Positions = new List<float>();

		/// <summary>
		/// Slide direction.
		/// </summary>
		public ObjectSlidingDirection Direction = ObjectSlidingDirection.Horizontal;

		/// <summary>
		/// Movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		public AnimationCurve Movement = AnimationCurve.EaseInOut(0, 0, 1, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		protected RectTransform RectTransform
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

		ScrollRect parentScrollRect;

		/// <summary>
		/// Parent ScrollRect.
		/// </summary>
		protected ScrollRect ParentScrollRect
		{
			get
			{
				if (parentScrollRect == null)
				{
					parentScrollRect = GetComponentInParent<ScrollRect>();
				}

				return parentScrollRect;
			}
		}

		/// <summary>
		/// The current animation.
		/// </summary>
		protected IEnumerator CurrentAnimation;

		/// <summary>
		/// Is animation running?
		/// </summary>
		protected bool IsAnimationRunning;

		/// <summary>
		/// Is drag allowed?
		/// </summary>
		protected bool AllowDrag;

		/// <summary>
		/// Start position.
		/// </summary>
		protected Vector3 StartPosition;

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			if (!interactableState)
			{
				SetEndPosition(false);
			}
		}

		/// <summary>
		/// Is direction is horizontal?
		/// </summary>
		/// <returns>true if direction is horizontal; otherwise, false.</returns>
		protected bool IsHorizontal() => Direction == ObjectSlidingDirection.Horizontal;

		/// <summary>
		/// Get end position.
		/// </summary>
		/// <returns>End position.</returns>
		protected float GetEndPosition()
		{
			var cur_position = IsHorizontal() ? RectTransform.anchoredPosition.x : RectTransform.anchoredPosition.y;

			var nearest_position = Positions[0];

			for (int i = 1; i < Positions.Count; i++)
			{
				if (Mathf.Abs(Positions[i] - cur_position) < Mathf.Abs(nearest_position - cur_position))
				{
					nearest_position = Positions[i];
				}
			}

			return nearest_position;
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
		/// Handle begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			if (IsAnimationRunning)
			{
				IsAnimationRunning = false;
				if (CurrentAnimation != null)
				{
					StopCoroutine(CurrentAnimation);
				}
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, null, out var current_position);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out var original_position);
			StartPosition = RectTransform.localPosition;

			var delta = current_position - original_position;
			AllowDrag = (IsHorizontal() && (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)))
				|| (!IsHorizontal() && (Mathf.Abs(delta.y) > Mathf.Abs(delta.x)));

			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnBeginDrag(eventData);
				}
			}
		}

		/// <summary>
		/// Handle drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnDrag(eventData);
				}

				return;
			}

			if (eventData.used)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			eventData.Use();

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out var current_position);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out var original_position);

			var position = new Vector3(
				IsHorizontal() ? StartPosition.x + (current_position.x - original_position.x) : StartPosition.x,
				!IsHorizontal() ? StartPosition.y + (current_position.y - original_position.y) : StartPosition.y,
				StartPosition.z);

			RectTransform.localPosition = position;
		}

		/// <summary>
		/// Handle end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnEndDrag(eventData);
				}

				return;
			}

			SetEndPosition(true);
		}

		/// <summary>
		/// Set end position.
		/// </summary>
		/// <param name="animate">Animate.</param>
		protected virtual void SetEndPosition(bool animate)
		{
			if (Positions.Count == 0)
			{
				return;
			}

			var end_position = GetEndPosition();
			if (animate)
			{
				IsAnimationRunning = true;
				var start_position = IsHorizontal() ? RectTransform.anchoredPosition.x : RectTransform.anchoredPosition.y;
				CurrentAnimation = RunAnimation(IsHorizontal(), start_position, end_position, UnscaledTime);
				StartCoroutine(CurrentAnimation);
			}
			else
			{
				RectTransform.anchoredPosition = IsHorizontal()
					? new Vector2(end_position, RectTransform.anchoredPosition.y)
					: new Vector2(RectTransform.anchoredPosition.x, end_position);
			}
		}

		/// <summary>
		/// Runs the animation.
		/// </summary>
		/// <returns>The animation.</returns>
		/// <param name="isHorizontal">If set to <c>true</c> is horizontal.</param>
		/// <param name="startPosition">Start position.</param>
		/// <param name="endPosition">End position.</param>
		/// <param name="unscaledTime">If set to <c>true</c> use unscaled time.</param>
		protected virtual IEnumerator RunAnimation(bool isHorizontal, float startPosition, float endPosition, bool unscaledTime)
		{
			var duration = Movement[Movement.length - 1].time;
			var time = 0f;

			do
			{
				var position = startPosition + ((endPosition - startPosition) * Movement.Evaluate(time));
				RectTransform.anchoredPosition = isHorizontal
					? new Vector2(position, RectTransform.anchoredPosition.y)
					: new Vector2(RectTransform.anchoredPosition.x, position);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			RectTransform.anchoredPosition = isHorizontal
				? new Vector2(endPosition, RectTransform.anchoredPosition.y)
				: new Vector2(RectTransform.anchoredPosition.x, endPosition);

			IsAnimationRunning = false;
		}
	}
}