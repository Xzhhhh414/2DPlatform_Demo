namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Changes the RectTransform size (while preserving its ratio) to the smallest possible size to fill the parent, leaving no empty space.
	/// It is recommended to add a Mask component to the parent.
	/// The aspect ratio is taken from ILayoutElement's preferred width and height.
	/// </summary>
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(ILayoutElement))]
	[DisallowMultipleComponent]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/layout/recttransform-cover.html")]
	public class RectTransformCover : UIBehaviour, ILayoutSelfController, ILayoutController
	{
		/// <summary>
		/// Flag to update size.
		/// </summary>
		[NonSerialized]
		protected bool NeedUpdate;

		/// <summary>
		/// Parent.
		/// </summary>
		[NonSerialized]
		protected RectTransform Parent;

		/// <summary>
		/// Updating.
		/// </summary>
		[NonSerialized]
		protected bool Updating = false;

		/// <summary>
		/// Tracker.
		/// </summary>
		protected DrivenRectTransformTracker Tracker;

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
					TryGetComponent(out rectTransform);
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (!CanUpdateSize())
			{
				enabled = false;
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Parent = RectTransform.parent as RectTransform;
			UpdateSize();
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
			base.OnDisable();
		}

		/// <summary>
		/// Process the parent changed event.
		/// </summary>
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			Parent = RectTransform.parent as RectTransform;
			UpdateSize();
		}

		/// <summary>
		/// Process resize event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			UpdateSize();
		}

		/// <summary>
		/// Can update size?
		/// </summary>
		/// <returns>true if can update; otherwise false.</returns>
		protected virtual bool CanUpdateSize()
		{
			if (TryGetComponent<Canvas>(out var canvas) && canvas.isRootCanvas && canvas.renderMode != RenderMode.WorldSpace)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Update size.
		/// </summary>
		protected virtual void UpdateSize()
		{
			if (Updating || !IsActive() || !CanUpdateSize() || (Parent == null))
			{
				return;
			}

			Updating = true;

			var size = new LayoutElementData(RectTransform);
			var width = size.PreferredWidth == 0f ? 1f : size.PreferredWidth;
			var height = size.PreferredHeight == 0f ? 1f : size.PreferredHeight;

			var aspect_ratio = width / height;

			var parent_size = Parent.rect.size;
			var parent_aspect_ratio = parent_size.x / parent_size.y;

			Tracker.Clear();
			Tracker.Add(this, RectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta);

			RectTransform.anchorMin = Vector2.zero;
			RectTransform.anchorMax = Vector2.one;
			RectTransform.anchoredPosition = Vector2.zero;

			var delta_size = Vector2.zero;

			if (aspect_ratio > parent_aspect_ratio)
			{
				delta_size.x = (parent_size.y * aspect_ratio) - parent_size.x;
			}
			else
			{
				delta_size.y = (parent_size.x / aspect_ratio) - parent_size.y;
			}

			RectTransform.sizeDelta = delta_size;

			Updating = false;
		}

		/// <summary>
		/// Set layout horizontal.
		/// </summary>
		public virtual void SetLayoutHorizontal()
		{
			UpdateSize();
		}

		/// <summary>
		/// Set layout vertical.
		/// </summary>
		public virtual void SetLayoutVertical()
		{
		}

		/// <summary>
		/// Process the update event.
		/// </summary>
		protected virtual void Update()
		{
			if (NeedUpdate)
			{
				NeedUpdate = false;
				UpdateSize();
			}
		}

		#if UNITY_EDITOR

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			Parent = RectTransform.parent as RectTransform;
			NeedUpdate = true;
		}
		#endif
	}
}