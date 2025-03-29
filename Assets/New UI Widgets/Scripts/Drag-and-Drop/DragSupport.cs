namespace UIWidgets
{
	using System;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag support.
	/// Drop component should implement IDropSupport{TItem} with same type.
	/// </summary>
	/// <typeparam name="TItem">Type of draggable data.</typeparam>
	public abstract class DragSupport<TItem> : BaseDragSupport, ICancelHandler
	{
		/// <summary>
		/// Gets or sets the data.
		/// Data will be passed to Drop component.
		/// </summary>
		/// <value>The data.</value>
		public TItem Data
		{
			get;
			protected set;
		}

		/// <summary>
		/// The Allow drop cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D AllowDropCursor;

		/// <summary>
		/// The Allow drop cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 AllowDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The Denied drop cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DeniedDropCursor;

		/// <summary>
		/// The Denied drop cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DeniedDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// Drag handle.
		/// </summary>
		[Obsolete("Renamed to Handle.")]
		public DragSupportHandle DragHandle
		{
			get => Handle;

			set => Handle = value;
		}

		/// <summary>
		/// The current drop target.
		/// </summary>
		protected IDropSupport<TItem> CurrentTarget;

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
		/// Set target.
		/// </summary>
		/// <param name="newTarget">New target.</param>
		/// <param name="eventData">Event data.</param>
		protected virtual void SetTarget(IDropSupport<TItem> newTarget, PointerEventData eventData)
		{
			if (CurrentTarget == newTarget)
			{
				return;
			}

			if (!Utilities.IsNull(CurrentTarget))
			{
				CurrentTarget.DropCanceled(Data, eventData);
			}

			OnTargetChanged(CurrentTarget, newTarget);

			CurrentTarget = newTarget;
		}

		/// <inheritdoc/>
		public override void Dropped(bool success)
		{
			Data = default;
		}

		/// <inheritdoc/>
		protected override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			Init();

			DelayTimer.Start(eventData);

			if (!CanDrag(eventData))
			{
				return;
			}

			SetTarget(null, eventData);
		}

		/// <inheritdoc/>
		protected override void FindCurrentTarget(PointerEventData eventData)
		{
			var new_target = FindTarget(eventData);

			SetTarget(new_target, eventData);

			if (UICursor.CanSet(this))
			{
				var cursor = Utilities.IsNull(CurrentTarget) ? GetDeniedCursor() : GetAllowedCursor();
				UICursor.Set(this, cursor);
			}
		}

		/// <summary>
		/// Process current drop target changed event.
		/// </summary>
		/// <param name="old">Previous drop target.</param>
		/// <param name="current">Current drop target.</param>
		protected virtual void OnTargetChanged(IDropSupport<TItem> old, IDropSupport<TItem> current)
		{
		}

		/// <summary>
		/// Finds the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="eventData">Event data.</param>
		protected virtual IDropSupport<TItem> FindTarget(PointerEventData eventData)
		{
			using var _ = ListPool<RaycastResult>.Get(out var ray_casts);

			EventSystem.current.RaycastAll(eventData, ray_casts);

			foreach (var ray_cast in ray_casts)
			{
				if (!ray_cast.isValid)
				{
					continue;
				}

				#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				var target = ray_cast.gameObject.GetComponent<IDropSupport<TItem>>();
				#else
				var target = raycast.gameObject.GetComponent(typeof(IDropSupport<TItem>)) as IDropSupport<TItem>;
				#endif
				if (!Utilities.IsNull(target))
				{
					return CheckTarget(target, eventData) ? target : null;
				}
			}

			return null;
		}

		/// <summary>
		/// Check if target can receive drop.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if target can receive drop; otherwise false.</returns>
		protected virtual bool CheckTarget(IDropSupport<TItem> target, PointerEventData eventData)
		{
			return target.CanReceiveDrop(Data, eventData);
		}

		/// <inheritdoc/>
		protected override void OnEndDrag(PointerEventData eventData)
		{
			if (!DelayTimer.Done)
			{
				DelayTimer.OnEndDrag(eventData);
				return;
			}

			if (!IsDragged)
			{
				return;
			}

			FindCurrentTarget(eventData);

			if (Utilities.IsNull(CurrentTarget))
			{
				Dropped(false);
			}
			else
			{
				CurrentTarget.Drop(Data, eventData);

				Dropped(true);
			}

			if (!Utilities.IsNull(CurrentAutoScrollTarget))
			{
				CurrentAutoScrollTarget.AutoScrollStop();
				CurrentAutoScrollTarget = null;
			}

			ResetCursor();

			EndDragEvent.Invoke();
		}

		/// <summary>
		/// Process disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			if (!IsDragged)
			{
				return;
			}

			if (!Utilities.IsNull(CurrentTarget))
			{
				CurrentTarget.DropCanceled(Data, null);
			}

			Dropped(false);

			if (!Utilities.IsNull(CurrentAutoScrollTarget))
			{
				CurrentAutoScrollTarget.AutoScrollStop();
				CurrentAutoScrollTarget = null;
			}

			ResetCursor();

			EndDragEvent.Invoke();
		}

		/// <inheritdoc/>
		public override void OnCancel(BaseEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			if (!Utilities.IsNull(CurrentTarget))
			{
				CurrentTarget.DropCanceled(Data, null);
			}

			Dropped(false);

			ResetCursor();
		}
	}
}