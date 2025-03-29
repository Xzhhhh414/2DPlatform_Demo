namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for the data view.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/tracksview.html")]
	public abstract class TrackDataViewBase<TData, TPoint> : MonoBehaviourInitiable, IMovableToCache
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Resizable component.
		/// </summary>
		[NonSerialized]
		protected Resizable Resizable;

		/// <summary>
		/// Show dialog to edit data on double click.
		/// </summary>
		[SerializeField]
		[Tooltip("Show dialog to edit data on double click.")]
		public bool AllowDialog = true;

		/// <summary>
		/// Owner.
		/// </summary>
		public TracksViewBase<TData, TPoint> Owner;

		/// <summary>
		/// Data.
		/// </summary>
		public TData Data
		{
			get;
			protected set;
		}

		RectTransform rt;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rt == null)
				{
					rt = transform as RectTransform;
				}

				return rt;
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			AddListeners();
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			var click = Utilities.RequireComponent<ClickListener>(this);
			click.DoubleClickEvent.AddListener(OnDoubleClick);

			if (TryGetComponent(out Resizable))
			{
				var directions = Resizable.ResizeDirections;
				directions.Top = false;
				directions.TopLeft = false;
				directions.TopRight = false;
				directions.Bottom = false;
				directions.BottomLeft = false;
				directions.BottomRight = false;

				Resizable.ResizeDirections = directions;
				Resizable.OnEndResize.AddListener(OnEndResize);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			if (TryGetComponent<ClickListener>(out var click))
			{
				click.DoubleClickEvent.RemoveListener(OnDoubleClick);
			}

			if (TryGetComponent(out Resizable))
			{
				Resizable.OnEndResize.RemoveListener(OnEndResize);
			}
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}

		/// <summary>
		/// Process end resize event.
		/// </summary>
		/// <param name="resizable">Resizable component.</param>
		protected virtual void OnEndResize(Resizable resizable)
		{
			var start = RectTransform.localPosition.x;
			var end = RectTransform.localPosition.x + RectTransform.rect.width;
			Data.ChangePoints(Owner.Position2Point(start), Owner.Position2Point(end));
		}

		/// <summary>
		/// Enable resizable component.
		/// </summary>
		public void EnableResizable()
		{
			if (Resizable != null)
			{
				Resizable.Interactable = true;
			}
		}

		/// <summary>
		/// Disable resizable component.
		/// </summary>
		public void DisableResizable()
		{
			if (Resizable != null)
			{
				Resizable.Interactable = false;
			}
		}

		/// <summary>
		/// Process double click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnDoubleClick(PointerEventData eventData)
		{
			if (!Owner.IsInteractable())
			{
				return;
			}

			if (!AllowDialog)
			{
				return;
			}

			Owner.OpenEditTrackDataDialog(Data);
		}

		/// <summary>
		/// Process move to cache event.
		/// </summary>
		public virtual void MovedToCache()
		{
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="track">Track.</param>
		/// <param name="data">Data.</param>
		public abstract void SetData(Track<TData, TPoint> track, TData data);
	}
}