namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Root component for the Resizable.
	/// Automatically attached to the canvas.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/resizable.html")]
	public class ResizableRoot : UIBehaviourConditional,
		IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler,
		IPointerDownHandler, IPointerUpHandler,
		ILateUpdatable
	{
		/// <summary>
		/// Helper struct to sort targets by depth in hierarchy.
		/// </summary>
		/// <typeparam name="T">Type of component.</typeparam>
		protected readonly struct Depth<T> : IComparable<Depth<T>>
			where T : Component
		{
			/// <summary>
			/// Target.
			/// </summary>
			public readonly T Target;

			/// <summary>
			/// Depths.
			/// </summary>
			public readonly List<int> Depths;

			/// <summary>
			/// Initializes a new instance of the <see cref="Depth{T}"/> struct.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="root">Root.</param>
			public Depth(T target, Transform root)
			{
				Target = target;
				Depths = ListPool<int>.Get();

				var current = target.transform;
				while (!ReferenceEquals(current, root))
				{
					Depths.Add(current.GetSiblingIndex());
					current = current.parent;
				}

				Depths.Reverse();
			}

			/// <summary>
			/// Compare.
			/// </summary>
			/// <param name="other">Other instance.</param>
			/// <returns>Comparison result.</returns>
			public int CompareTo(Depth<T> other)
			{
				var n = Mathf.Min(Depths.Count, other.Depths.Count);
				for (int i = 0; i < n; i++)
				{
					var c = -Depths[i].CompareTo(other.Depths[i]);
					if (c != 0)
					{
						return c;
					}
				}

				return -Depths.Count.CompareTo(other.Depths.Count);
			}

			/// <summary>
			/// Release.
			/// </summary>
			public void Release() => ListPool<int>.Release(Depths);
		}

		/// <summary>
		/// Substrate to prevent receiving drag event by other components when cursor in outer region of the target.
		/// </summary>
		protected struct Substrate
		{
			readonly Transform root;

			RectTransform rectTransform;

			/// <summary>
			/// RectTransform.
			/// </summary>
			public RectTransform RectTransform
			{
				get
				{
					if (rectTransform == null)
					{
						var go = new GameObject("ResizableRoot Substrate");
						rectTransform = go.AddComponent<RectTransform>();
						rectTransform.SetParent(root);

						rectTransform.anchorMin = Vector2.zero;
						rectTransform.anchorMax = Vector2.one;

						var img = go.AddComponent<Image>();
						img.color = Color.clear;

						var le = go.AddComponent<LayoutElement>();
						le.ignoreLayout = true;

						go.SetActive(false);
					}

					return rectTransform;
				}
			}

			RectTransform target;

			bool active;

			/// <summary>
			/// Initializes a new instance of the <see cref="Substrate"/> struct.
			/// </summary>
			/// <param name="root">Root.</param>
			public Substrate(Transform root)
			{
				this.root = root;
				rectTransform = null;
				target = null;
				active = false;
			}

			/// <summary>
			/// Show.
			/// </summary>
			/// <param name="resizable">Resizable.</param>
			public void Show(Resizable resizable)
			{
				if (!resizable.IncludeOuterRegion)
				{
					Hide();
					return;
				}

				var rt = RectTransform;
				if (!ReferenceEquals(target, resizable.Target))
				{
					target = resizable.Target;
					rt.SetParent(resizable.Target);
				}

				var border = resizable.ActiveRegion * 2;
				rt.sizeDelta = new Vector2(border, border);
				rt.anchoredPosition = Vector2.zero;

				active = true;
				rt.gameObject.SetActive(true);
			}

			/// <summary>
			/// Hide.
			/// </summary>
			public void Hide()
			{
				if (!active)
				{
					return;
				}

				target = null;
				active = false;
				RectTransform.gameObject.SetActive(false);

				RectTransform.SetParent(root);
			}
		}

		/// <summary>
		/// Targets.
		/// </summary>
		[NonSerialized]
		protected List<Resizable> Targets = new List<Resizable>();

		/// <summary>
		/// Active target.
		/// </summary>
		[NonSerialized]
		protected Resizable ActiveTarget;

		/// <summary>
		/// Current camera.
		/// </summary>
		[NonSerialized]
		protected Camera CurrentCamera;

		/// <summary>
		/// RectTransform.
		/// </summary>
		[NonSerialized]
		protected RectTransform RectTransform;

		/// <summary>
		/// Is pointer down?
		/// </summary>
		[NonSerialized]
		protected bool IsPointerDown;

		/// <summary>
		/// Substrate to prevent receiving drag event by other components when cursor in outer region of the target.
		/// </summary>
		[NonSerialized]
		protected Substrate ResizableSubstrate;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			ResizableSubstrate = new Substrate(transform);
		}

		/// <summary>
		/// Add target.
		/// </summary>
		/// <param name="target">Target.</param>
		public virtual void Add(Resizable target) => Targets.Add(target);

		/// <summary>
		/// Remove target.
		/// </summary>
		/// <param name="target">Target.</param>
		public virtual void Remove(Resizable target) => Targets.Remove(target);

		/// <summary>
		/// Find nearest target with active region under specified screen point.
		/// </summary>
		/// <param name="screenPoint">Screen point.</param>
		/// <param name="camera">Camera.</param>
		/// <returns>Target.</returns>
		protected virtual Resizable FindTarget(Vector2 screenPoint, Camera camera)
		{
			using var _ = ListPool<Resizable>.Get(out var active);

			var root = transform;
			foreach (var target in Targets)
			{
				if (target.IsActive() && target.InActiveRegion(screenPoint, camera))
				{
					active.Add(target);
				}
			}

			if (active.Count == 0)
			{
				return null;
			}

			var result = active[0];
			if (active.Count > 1)
			{
				using var __ = ListPool<Depth<Resizable>>.Get(out var sort);
				foreach (var a in active)
				{
					sort.Add(new Depth<Resizable>(a, RectTransform));
				}

				sort.Sort();

				result = sort[0].Target;
				foreach (var s in sort)
				{
					s.Release();
				}
			}

			return IsTop(result, screenPoint) ? result : null;
		}

		/// <summary>
		/// Check is resizable part under cursor is not hidden behind other objects.
		/// </summary>
		/// <param name="resizable">Resizable.</param>
		/// <param name="screenPoint">Screen point.</param>
		/// <returns>true if resizable is visible; otherwise false.</returns>
		protected virtual bool IsTop(Resizable resizable, Vector2 screenPoint)
		{
			using var _ = ListPool<RaycastResult>.Get(out var ray_casts);
			using var __ = ListPool<Depth<RectTransform>>.Get(out var targets);

			var event_data = new PointerEventData(EventSystem.current)
			{
				position = screenPoint,
			};

			EventSystem.current.RaycastAll(event_data, ray_casts);

			foreach (var ray_cast in ray_casts)
			{
				if (!ray_cast.isValid)
				{
					continue;
				}

				if (!ray_cast.gameObject.TryGetComponent<Graphic>(out var target))
				{
					continue;
				}

				var rt = target.transform as RectTransform;
				if ((rt == null) || rt.IsChildOf(resizable.RectTransform) || ReferenceEquals(rt, RectTransform))
				{
					continue;
				}

				if (!rt.IsChildOf(RectTransform))
				{
					continue;
				}

				targets.Add(new Depth<RectTransform>(rt, RectTransform));
			}

			if (targets.Count == 0)
			{
				return true;
			}

			targets.Add(new Depth<RectTransform>(resizable.RectTransform, RectTransform));
			targets.Sort();

			foreach (var s in targets)
			{
				s.Release();
			}

			return ReferenceEquals(targets[0].Target, resizable.RectTransform);
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			RectTransform = transform as RectTransform;
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
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if ((ActiveTarget != null) && !eventData.used)
			{
				ActiveTarget.BeginDrag(eventData);
			}
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if ((ActiveTarget != null) && !eventData.used)
			{
				ActiveTarget.Drag(eventData);
			}
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if ((ActiveTarget != null) && !eventData.used)
			{
				ActiveTarget.EndDrag(eventData);
			}
		}

		/// <summary>
		/// Process the pointer event event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			CurrentCamera = eventData.pressEventCamera;
		}

		/// <summary>
		/// Process the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerExit(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Process the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (ActiveTarget != null && (eventData.button == ActiveTarget.DragButton))
			{
				IsPointerDown = true;
			}
		}

		/// <summary>
		/// Process the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			IsPointerDown = false;
		}

		/// <summary>
		/// Late update.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (IsPointerDown)
			{
				return;
			}

			var target = FindTarget(CompatibilityInput.MousePosition, CurrentCamera);
			if ((ActiveTarget != null) && !ReferenceEquals(ActiveTarget, target))
			{
				ActiveTarget.ResetCursor();
			}

			if (target != null)
			{
				ActiveTarget = target;
				ActiveTarget.UpdateCursor();

				ResizableSubstrate.Show(target);
			}
			else
			{
				ResizableSubstrate.Hide();
			}
		}
	}
}