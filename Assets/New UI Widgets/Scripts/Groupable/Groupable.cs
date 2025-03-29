namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Groupable component.
	/// </summary>
	[RequireComponent(typeof(Resizable))]
	[RequireComponent(typeof(Rotatable))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Interactions/Groupable")]
	[DisallowMultipleComponent]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/groupable.html")]
	public class Groupable : UIBehaviourInteractable
	{
		/// <summary>
		/// Selection mode.
		/// </summary>
		public enum Mode
		{
			/// <summary>
			/// Selects gameobjects fully inside the selection area.
			/// </summary>
			Contains = 0,

			/// <summary>
			/// Selects gameobjects inside the selection area or partially overlaps the selection area.
			/// </summary>
			Overlaps = 1,
		}

		/// <summary>
		/// Selected elements.
		/// </summary>
		[NonSerialized]
		protected List<RectTransform> selected = new List<RectTransform>();

		/// <summary>
		/// Resizable components of the selected elements.
		/// </summary>
		[NonSerialized]
		protected List<Resizable> SelectedResizable = new List<Resizable>();

		/// <summary>
		/// Selected elements.
		/// </summary>
		public IReadOnlyList<RectTransform> Selected => selected;

		/// <summary>
		/// RectTransform.
		/// </summary>
		[NonSerialized]
		protected RectTransform RectTransform;

		/// <summary>
		/// Parent RectTransform.
		/// </summary>
		[NonSerialized]
		protected RectTransform ParentRectTransform;

		/// <summary>
		/// Parent DragListener.
		/// </summary>
		[NonSerialized]
		protected DragListener ParentDragListener;

		/// <summary>
		/// Parent ClickListener.
		/// </summary>
		[NonSerialized]
		protected ClickListener ParentClickListener;

		/// <summary>
		/// Resizable.
		/// </summary>
		[NonSerialized]
		protected Resizable Resizable;

		/// <summary>
		/// ResizableHandles.
		/// </summary>
		[NonSerialized]
		protected ResizableHandles ResizableHandles;

		/// <summary>
		/// Rotatable.
		/// </summary>
		[NonSerialized]
		protected Rotatable Rotatable;

		/// <summary>
		/// RotatableHandle.
		/// </summary>
		[NonSerialized]
		protected RotatableHandle RotatableHandle;

		/// <summary>
		/// Graphic.
		/// </summary>
		[NonSerialized]
		protected Graphic Graphic;

		/// <summary>
		/// Highlight template.
		/// </summary>
		[SerializeField]
		protected RectTransform highlightTemplate;

		/// <summary>
		/// Highlight template.
		/// </summary>
		public RectTransform HighlightTemplate
		{
			get
			{
				return highlightTemplate;
			}

			set
			{
				if (highlightTemplate != value)
				{
					highlightTemplate = value;

					HighlightsPool.Template = highlightTemplate;
				}
			}
		}

		/// <summary>
		/// Used instances of the DefaultItem.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<RectTransform> Highlights = new List<RectTransform>();

		/// <summary>
		/// Unused instances of the DefaultItem.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<RectTransform> HighlightsCache = new List<RectTransform>();

		ListComponentPool<RectTransform> highlightsPool;

		/// <summary>
		/// Components pool.
		/// </summary>
		protected ListComponentPool<RectTransform> HighlightsPool
		{
			get
			{
				if ((highlightsPool == null) || (highlightsPool.Template == null))
				{
					highlightsPool = new ListComponentPool<RectTransform>(HighlightTemplate, Highlights, HighlightsCache, transform.parent as RectTransform);
				}

				return highlightsPool;
			}
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Selection mode.
		/// </summary>
		[SerializeField]
		public Mode SelectionMode = Mode.Contains;

		/// <summary>
		/// Rotation mode.
		/// </summary>
		[SerializeField]
		public bool GroupRotation = true;

		/// <summary>
		/// Start selection event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnStartSelection = new UnityEvent();

		/// <summary>
		/// Selection event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnSelection = new UnityEvent();

		/// <summary>
		/// End selection event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnEndSelection = new UnityEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			RectTransform = transform as RectTransform;

			ParentRectTransform = transform.parent as RectTransform;
			ParentDragListener = Utilities.RequireComponent<DragListener>(ParentRectTransform);
			ParentDragListener.OnBeginDragEvent.AddListener(OnBeginDrag);
			ParentDragListener.OnDragEvent.AddListener(OnDrag);
			ParentDragListener.OnEndDragEvent.AddListener(OnEndDrag);

			ParentClickListener = Utilities.RequireComponent<ClickListener>(ParentRectTransform);
			ParentClickListener.ClickEvent.AddListener(OnClick);

			TryGetComponent(out Resizable);
			Resizable.OnStartResize.AddListener(OnStartResize);
			Resizable.OnResizeDelta.AddListener(OnResize);

			if (TryGetComponent(out ResizableHandles))
			{
				ResizableHandles.OnStartResize.AddListener(OnStartResize);
				ResizableHandles.OnResize.AddListener(OnResize);
			}

			TryGetComponent(out Rotatable);
			Rotatable.OnStartRotate.AddListener(OnStartRotate);
			Rotatable.OnRotate.AddListener(OnRotate);
			Rotatable.OnEndRotate.AddListener(OnEndRotate);

			if (TryGetComponent(out RotatableHandle))
			{
				RotatableHandle.OnStartRotate.AddListener(OnStartRotate);
				RotatableHandle.OnRotate.AddListener(OnRotate);
				RotatableHandle.OnEndRotate.AddListener(OnEndRotate);
			}

			TryGetComponent(out Graphic);
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			Graphic.raycastTarget = false;
			#endif

			if (HighlightTemplate != null)
			{
				HighlightTemplate.gameObject.SetActive(false);
			}

			DisableSelection();
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			highlightsPool = null;

			if (ParentDragListener != null)
			{
				ParentDragListener.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				ParentDragListener.OnDragEvent.RemoveListener(OnDrag);
				ParentDragListener.OnEndDragEvent.RemoveListener(OnEndDrag);
			}

			if (ParentClickListener != null)
			{
				ParentClickListener.ClickEvent.RemoveListener(OnClick);
			}

			if (Resizable != null)
			{
				Resizable.OnStartResize.RemoveListener(OnStartResize);
				Resizable.OnResizeDelta.RemoveListener(OnResize);
			}

			if (ResizableHandles != null)
			{
				ResizableHandles.OnStartResize.RemoveListener(OnStartResize);
				ResizableHandles.OnResize.RemoveListener(OnResize);
			}

			if (Rotatable != null)
			{
				Rotatable.OnStartRotate.RemoveListener(OnStartRotate);
				Rotatable.OnRotate.RemoveListener(OnRotate);
				Rotatable.OnEndRotate.RemoveListener(OnEndRotate);
			}

			if (RotatableHandle != null)
			{
				RotatableHandle.OnStartRotate.RemoveListener(OnStartRotate);
				RotatableHandle.OnRotate.RemoveListener(OnRotate);
				RotatableHandle.OnEndRotate.RemoveListener(OnEndRotate);
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Process click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnClick(PointerEventData eventData)
		{
			if (IsDrag)
			{
				return;
			}

			DisableSelection();
		}

		/// <summary>
		/// Process start rotation event.
		/// </summary>
		/// <param name="rotatable">Rotatable.</param>
		protected virtual void OnStartRotate(Rotatable rotatable)
		{
			if (GroupRotation)
			{
				var highlight = HighlightTemplate != null;
				for (int i = 0; i < selected.Count; i++)
				{
					selected[i].SetParent(RectTransform, true);

					if (highlight)
					{
						HighlightsPool[i].SetParent(RectTransform, true);
					}
				}
			}
		}

		/// <summary>
		/// Process rotation event.
		/// </summary>
		/// <param name="rotatable">Rotatable.</param>
		protected virtual void OnRotate(Rotatable rotatable)
		{
			if (!GroupRotation)
			{
				var rotation = RectTransform.localRotation;

				for (int i = 0; i < selected.Count; i++)
				{
					selected[i].localRotation = rotation;
				}

				UpdateHighlights();
			}
		}

		/// <summary>
		/// Process end rotation event.
		/// </summary>
		/// <param name="rotatable">Rotatable.</param>
		protected virtual void OnEndRotate(Rotatable rotatable)
		{
			if (GroupRotation)
			{
				var highlight = HighlightTemplate != null;
				for (int i = 0; i < selected.Count; i++)
				{
					selected[i].SetParent(ParentRectTransform, true);

					if (highlight)
					{
						HighlightsPool[i].SetParent(ParentRectTransform, true);
					}
				}
			}
		}

		/// <summary>
		/// Process start resize event.
		/// </summary>
		/// <param name="resizable">Resizable.</param>
		protected virtual void OnStartResize(Resizable resizable)
		{
			for (int i = 0; i < SelectedResizable.Count; i++)
			{
				SelectedResizable[i].InitResize();
				SelectedResizable[i].KeepAspectRatio = resizable.KeepAspectRatio;
			}
		}

		/// <summary>
		/// Process resize event.
		/// </summary>
		/// <param name="resizable">Resizable.</param>
		/// <param name="regions">Regions.</param>
		/// <param name="delta">Delta.</param>
		protected virtual void OnResize(Resizable resizable, Resizable.Regions regions, Vector2 delta)
		{
			for (int i = 0; i < SelectedResizable.Count; i++)
			{
				SelectedResizable[i].Resize(regions, delta);
			}

			UpdateHighlights();
		}

		/// <summary>
		/// Is currently dragged?
		/// </summary>
		protected bool IsDrag;

		/// <summary>
		/// Original value of Resizable.AspectRatio.
		/// </summary>
		protected bool KeepAspectRatio;

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return IsInteractable() && (eventData.button == DragButton);
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

			RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRectTransform, eventData.position, eventData.pressEventCamera, out var point);

			var size = ParentRectTransform.rect.size;
			var pivot = ParentRectTransform.pivot;
			var position = new Vector2(point.x + (size.x * pivot.x), (size.y * pivot.y) + point.y);

			IsDrag = true;

			RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, position.x, 0f);
			RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, position.y, 0f);

			gameObject.SetActive(true);
			RectTransform.localRotation = Quaternion.Euler(Vector3.zero);

			Graphic.enabled = true;

			KeepAspectRatio = Resizable.KeepAspectRatio;
			Resizable.KeepAspectRatio = false;
			Resizable.Init();
			Resizable.InitResize();
			RectTransform.SetAsLastSibling();

			DisableHandles();

			FindSelected();

			OnStartSelection.Invoke();
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

			RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRectTransform, eventData.position, eventData.pressEventCamera, out var current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRectTransform, eventData.pressPosition, eventData.pressEventCamera, out var original_point);

			var delta = current_point - original_point;
			var regions = new Resizable.Regions()
			{
				Top = delta.y >= 0f,
				Bottom = delta.y < 0f,
				Left = delta.x < 0f,
				Right = delta.x >= 0f,
			};

			Resizable.Resize(regions, delta);

			FindSelected();

			OnSelection.Invoke();
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

			FindSelected();
			UpdateSelectedResizable();

			SelectionFinished();

			Resizable.KeepAspectRatio = KeepAspectRatio;

			OnEndSelection.Invoke();
		}

		/// <summary>
		/// Toggle state to selection finished.
		/// </summary>
		protected virtual void SelectionFinished()
		{
			if (selected.Count == 0)
			{
				DisableSelection();
			}
			else
			{
				EnableHandles();

				Graphic.enabled = false;
			}
		}

		/// <summary>
		/// Enable handles.
		/// </summary>
		public void EnableHandles()
		{
			if (ResizableHandles != null)
			{
				ResizableHandles.EnableHandles();
			}

			if (RotatableHandle != null)
			{
				RotatableHandle.EnableHandle();
			}
		}

		/// <summary>
		/// Disable handles.
		/// </summary>
		public void DisableHandles()
		{
			if (ResizableHandles != null)
			{
				ResizableHandles.DisableHandles();
			}

			if (RotatableHandle != null)
			{
				RotatableHandle.DisableHandle();
			}
		}

		/// <summary>
		/// Disable selection.
		/// </summary>
		public virtual void DisableSelection()
		{
			selected.Clear();
			gameObject.SetActive(false);
			DisableHandles();

			HighlightsPool.Require(0);
		}

		/// <summary>
		/// Set selected elements.
		/// </summary>
		/// <param name="newSelected">Selected elements.</param>
		public virtual void SetSelected(IList<RectTransform> newSelected)
		{
			Init();

			selected.Clear();
			selected.AddRange(newSelected);

			var selection = default(Rect);
			for (int i = 0; i < selected.Count; i++)
			{
				var rect = GetRect(selected[i]);

				selection.xMin = Mathf.Min(selection.xMin, rect.xMin);
				selection.xMax = Mathf.Max(selection.xMax, rect.xMax);

				selection.yMin = Mathf.Min(selection.yMin, rect.yMin);
				selection.yMax = Mathf.Max(selection.yMax, rect.yMax);
			}

			selection.xMin = Mathf.FloorToInt(selection.xMin);
			selection.xMax = Mathf.RoundToInt(selection.xMax);

			selection.yMin = Mathf.FloorToInt(selection.yMin);
			selection.yMax = Mathf.RoundToInt(selection.yMax);

			var parent = RectTransform.parent as RectTransform;
			var parent_size = parent.rect.size;
			var parent_pivot = parent.pivot;

			selection.x += parent_size.x * parent_pivot.x;
			selection.y += parent_size.y * parent_pivot.y;

			RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, selection.x, selection.width);
			RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, selection.y, selection.height);

			gameObject.SetActive(true);

			SelectionFinished();
			UpdateHighlights();
		}

		/// <summary>
		/// Get Rect with values relative to the parent.
		/// </summary>
		/// <param name="rectTransform">RectTransform</param>
		/// <returns>Rect.</returns>
		protected virtual Rect GetRect(RectTransform rectTransform)
		{
			var rect = rectTransform.rect;
			var pivot = rectTransform.pivot;
			var pos = rectTransform.localPosition;
			rect.x = pos.x - (pivot.x * rect.width);
			rect.y = pos.y - (pivot.y * rect.height);

			return rect;
		}

		/// <summary>
		/// Find selected elements.
		/// </summary>
		protected virtual void FindSelected()
		{
			selected.Clear();
			SelectedResizable.Clear();

			var main = GetRect(RectTransform);

			for (int i = 0; i < ParentRectTransform.childCount; i++)
			{
				var rt = ParentRectTransform.GetChild(i) as RectTransform;
				if (!CanBeSelected(rt))
				{
					continue;
				}

				var sub = GetRect(rt);
				if (IsSelected(main, sub))
				{
					selected.Add(rt);
				}
			}

			UpdateHighlights();
		}

		/// <summary>
		/// Check is element can be selected.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <returns>true if element can selected; otherwise false.</returns>
		protected virtual bool CanBeSelected(RectTransform element)
		{
			if (!element.gameObject.activeSelf)
			{
				return false;
			}

			if (element.GetInstanceID() == RectTransform.GetInstanceID())
			{
				return false;
			}

			if (HighlightTemplate != null)
			{
				if (element.GetInstanceID() == HighlightTemplate.GetInstanceID())
				{
					return false;
				}
			}

			if (Highlights.Contains(element))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Update list of the resizable components.
		/// </summary>
		protected virtual void UpdateSelectedResizable()
		{
			SelectedResizable.Clear();

			for (int i = 0; i < selected.Count; i++)
			{
				if (!selected[i].TryGetComponent<Resizable>(out var resizable))
				{
					resizable = selected[i].gameObject.AddComponent<Resizable>();
					resizable.Interactable = false;
				}

				SelectedResizable.Add(resizable);
			}
		}

		/// <summary>
		/// Check is element inside selection.
		/// </summary>
		/// <param name="selection">Selection.</param>
		/// <param name="element">Element.</param>
		/// <returns>true if element inside selection.</returns>
		protected bool IsSelected(Rect selection, Rect element)
		{
			if (SelectionMode == Mode.Contains)
			{
				var top_left = new Vector2(element.xMin, element.yMin);
				var top_right = new Vector2(element.xMax, element.yMin);
				var bottom_left = new Vector2(element.xMin, element.yMax);
				var bottom_right = new Vector2(element.xMax, element.yMax);

				return selection.Contains(top_left)
					&& selection.Contains(top_right)
					&& selection.Contains(bottom_left)
					&& selection.Contains(bottom_right);
			}

			if (SelectionMode == Mode.Overlaps)
			{
				return selection.Overlaps(element);
			}

			throw new NotSupportedException("Specified SelectionMode is not supported.");
		}

		/// <summary>
		/// Update highlights.
		/// </summary>
		public virtual void UpdateHighlights()
		{
			if (HighlightTemplate == null)
			{
				return;
			}

			HighlightsPool.Require(selected.Count);

			for (int i = 0; i < HighlightsPool.Count; i++)
			{
				SetHighlighted(selected[i], HighlightsPool[i]);
			}
		}

		/// <summary>
		/// Set highlight for the specified element.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="highlight">Highlight.</param>
		protected virtual void SetHighlighted(RectTransform element, RectTransform highlight)
		{
			UtilitiesRectTransform.CopyValues(element, highlight);
			highlight.SetAsLastSibling();
		}

#region Align

		/// <summary>
		/// Left align.
		/// </summary>
		public void AlignLeft()
		{
			foreach (var s in selected)
			{
				UtilitiesAlign.Left(s);
			}

			UpdateHighlights();
		}

		/// <summary>
		/// Center align.
		/// </summary>
		public void AlignCenter()
		{
			foreach (var s in selected)
			{
				UtilitiesAlign.Center(s);
			}

			UpdateHighlights();
		}

		/// <summary>
		/// Right align.
		/// </summary>
		public void AlignRight()
		{
			foreach (var s in selected)
			{
				UtilitiesAlign.Right(s);
			}

			UpdateHighlights();
		}

		/// <summary>
		/// Top align.
		/// </summary>
		public void AlignTop()
		{
			foreach (var s in selected)
			{
				UtilitiesAlign.Top(s);
			}

			UpdateHighlights();
		}

		/// <summary>
		/// Middle align.
		/// </summary>
		public void AlignMiddle()
		{
			foreach (var s in selected)
			{
				UtilitiesAlign.Middle(s);
			}

			UpdateHighlights();
		}

		/// <summary>
		/// Bottom align.
		/// </summary>
		public void AlignBottom()
		{
			foreach (var s in selected)
			{
				UtilitiesAlign.Bottom(s);
			}

			UpdateHighlights();
		}
#endregion
	}
}