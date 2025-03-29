namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Double carousel.
	/// Requires Graphic component on slides gameobject, it can be transparent.
	/// </summary>
	/// <typeparam name="T">Component type.</typeparam>
	public class DoubleCarousel<T> : MonoBehaviourInitiable
		where T : Component
	{
		/// <summary>
		/// Slides count.
		/// </summary>
		public int Count => DirectChildren.Count;

		/// <summary>
		/// Active slides count.
		/// </summary>
		[Obsolete("Use Count property.")]
		public int ActiveCount => DirectSlides.Count;

		/// <summary>
		/// Custom set direct slide state.
		/// </summary>
		public Action<T, float> CustomSetDirectSlideState;

		/// <summary>
		/// Custom set reverse slide state.
		/// </summary>
		public Action<T, float> CustomSetReverseSlideState;

		/// <summary>
		/// Paginator with direct scroll.
		/// </summary>
		[SerializeField]
		protected ScrollRectPaginator DirectPaginator;

		/// <summary>
		/// ScrollRect to reverse scroll.
		/// </summary>
		[SerializeField]
		protected ScrollRect ReverseScrollRect;

		/// <summary>
		/// Images scale.
		/// </summary>
		[SerializeField]
		public float Scale = 1.5f;

		/// <summary>
		/// Resize slides.
		/// </summary>
		[SerializeField]
		protected bool ResizeSlides = true;

		/// <summary>
		/// Current RectTransform.
		/// </summary>
		protected RectTransform CurrentRectTransform;

		/// <summary>
		/// Resize listener.
		/// </summary>
		protected ResizeListener ResizeListener;

		/// <summary>
		/// Direct content.
		/// </summary>
		protected RectTransform DirectContent;

		/// <summary>
		/// DirectContent children.
		/// </summary>
		protected List<RectTransform> DirectChildren = new List<RectTransform>();

		/// <summary>
		/// DirectContent slides.
		/// </summary>
		protected List<RectTransform> DirectSlides = new List<RectTransform>();

		/// <summary>
		/// DirectContent targets.
		/// </summary>
		protected List<T> DirectChildrenTargets = new List<T>();

		/// <summary>
		/// Reverse content.
		/// </summary>
		protected RectTransform ReverseContent;

		/// <summary>
		/// ReverseContent children.
		/// </summary>
		protected List<RectTransform> ReverseChildren = new List<RectTransform>();

		/// <summary>
		/// ReverseContent slides.
		/// </summary>
		protected List<RectTransform> ReverseSlides = new List<RectTransform>();

		/// <summary>
		/// ReverseContent targets.
		/// </summary>
		protected List<T> ReverseChildrenTargets = new List<T>();

		/// <summary>
		/// Duplicate of the first slide.
		/// </summary>
		protected RectTransform FirstDuplicate;

		/// <summary>
		/// Duplicate of the last slide.
		/// </summary>
		protected RectTransform LastDuplicate;

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected override void InitOnce()
		{
			// TODO move from examples
			base.InitOnce();

			CurrentRectTransform = transform as RectTransform;
			ReverseContent = ReverseScrollRect.content;
			DirectContent = DirectPaginator.ScrollRect.content;

			for (int i = 0; i < DirectContent.childCount; i++)
			{
				DirectChildren.Add(DirectContent.GetChild(i) as RectTransform);
			}

			for (int i = 0; i < ReverseContent.childCount; i++)
			{
				ReverseChildren.Add(ReverseContent.GetChild(i) as RectTransform);
			}

			CreateDuplicates();

			GetSlides(ReverseChildren, ReverseSlides, ReverseChildrenTargets);
			GetSlides(DirectChildren, DirectSlides, DirectChildrenTargets);

			if (ResizeSlides)
			{
				EnsureSlidesImage(DirectSlides);

				ResizeListener = Utilities.RequireComponent<ResizeListener>(this);
				ResizeListener.OnResizeNextFrame.AddListener(UpdateSlidesSize);
				UpdateSlidesSize();
			}

			DirectPaginator.OnMovement.AddListener(UpdateSlidesState);
			DirectPaginator.Init();
			UpdateSlidesState(DirectPaginator.CurrentPage, 0f);
		}

		/// <summary>
		/// Destroy duplicates.
		/// </summary>
		protected virtual void DestroyDuplicates()
		{
			if (FirstDuplicate != null)
			{
				FirstDuplicate.gameObject.SetActive(false);
				Destroy(FirstDuplicate.gameObject);
				ReverseChildren.RemoveAt(0);
			}

			if (LastDuplicate != null)
			{
				LastDuplicate.gameObject.SetActive(false);
				Destroy(LastDuplicate.gameObject);
				ReverseChildren.RemoveAt(ReverseChildren.Count - 1);
			}
		}

		/// <summary>
		/// Create duplicates.
		/// </summary>
		protected virtual void CreateDuplicates()
		{
			DestroyDuplicates();

			FirstDuplicate = Instantiate(ReverseChildren[0], ReverseContent, true);
			FirstDuplicate.SetAsLastSibling();
			ReverseChildren.Insert(0, FirstDuplicate);

			LastDuplicate = Instantiate(ReverseChildren[ReverseChildren.Count - 1], ReverseContent, true);
			LastDuplicate.SetAsFirstSibling();
			ReverseChildren.Add(LastDuplicate);
		}

		/// <summary>
		/// Refresh.
		/// </summary>
		public virtual void Refresh()
		{
			Init();

			CreateDuplicates();

			GetSlides(ReverseChildren, ReverseSlides, ReverseChildrenTargets);
			GetSlides(DirectChildren, DirectSlides, DirectChildrenTargets);

			if (ResizeSlides)
			{
				EnsureSlidesImage(DirectSlides);
				UpdateSlidesSize();
			}

			DirectPaginator.Refresh();
			UpdateSlidesState(DirectPaginator.CurrentPage, 0f);
		}

		/// <summary>
		/// Add slide.
		/// </summary>
		/// <param name="index">Slide index.</param>
		/// <param name="direct">Direct (foreground) slide.</param>
		/// <param name="reverse">Reverse (background) slide.</param>
		public virtual void AddSlide(int index, RectTransform direct, RectTransform reverse)
		{
			Init();

			DirectChildren.Insert(index, direct);
			direct.SetParent(DirectContent, true);
			direct.SetSiblingIndex(index);

			var reverse_index = ReverseChildren.Count - index - 2;
			ReverseChildren.Insert(reverse_index, reverse);
			reverse.SetParent(ReverseContent, true);
			reverse.SetSiblingIndex(reverse_index);

			Refresh();
		}

		/// <summary>
		/// Enable slide.
		/// </summary>
		/// <param name="index">Slide index.</param>
		public virtual void EnableSlide(int index)
		{
			ToggleSlide(index, true);
		}

		/// <summary>
		/// Disable slide.
		/// </summary>
		/// <param name="index">Slide index.</param>
		public virtual void DisableSlide(int index)
		{
			ToggleSlide(index, false);
		}

		/// <summary>
		/// Remove slide.
		/// </summary>
		/// <param name="index">Slide index.</param>
		[Obsolete("Renamed to DisableSlide()")]
		public virtual void RemoveSlide(int index)
		{
			ToggleSlide(index, false);
		}

		/// <summary>
		/// Toggle slide.
		/// </summary>
		/// <param name="index">Slide index.</param>
		/// <param name="active">Active.</param>
		public virtual void ToggleSlide(int index, bool active)
		{
			Init();

			DirectChildren[index].gameObject.SetActive(active);
			ReverseChildren[ReverseChildren.Count - index - 2].gameObject.SetActive(active);

			Refresh();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected void OnDestroy()
		{
			if (DirectPaginator != null)
			{
				DirectPaginator.OnMovement.RemoveListener(UpdateSlidesState);
			}

			if (ResizeListener != null)
			{
				ResizeListener.OnResizeNextFrame.RemoveListener(UpdateSlidesSize);
			}
		}

		/// <summary>
		/// Get slides.
		/// </summary>
		/// <param name="all">All slides.</param>
		/// <param name="slides">Result children list.</param>
		/// <param name="targets">Result targets list.</param>
		protected virtual void GetSlides(List<RectTransform> all, List<RectTransform> slides, List<T> targets)
		{
			slides.Clear();
			targets.Clear();

			foreach (var slide in all)
			{
				if (!slide.gameObject.activeSelf)
				{
					continue;
				}

				slides.Add(slide);

				if (slide.TryGetComponent<T>(out var target))
				{
					targets.Add(target);
				}
			}
		}

		/// <summary>
		/// Ensure all slides have an Image component.
		/// </summary>
		/// <param name="slides">Slides.</param>
		protected virtual void EnsureSlidesImage(List<RectTransform> slides)
		{
			foreach (var slide in slides)
			{
				if (!slide.TryGetComponent<Graphic>(out var _))
				{
					var img = slide.gameObject.AddComponent<Image>();
					img.enabled = false;
				}
			}
		}

		/// <summary>
		/// Update slides size.
		/// </summary>
		protected virtual void UpdateSlidesSize()
		{
			SetSlidesSize(CurrentRectTransform.rect.size);
		}

		/// <summary>
		/// Set slides size.
		/// </summary>
		/// <param name="size">Size.</param>
		protected virtual void SetSlidesSize(Vector2 size)
		{
			var page = DirectPaginator.CurrentPage;
			SetSlidesSize(size, DirectSlides);
			SetSlidesSize(size, ReverseSlides);

			DirectPaginator.PageSizeType = PageSizeType.Fixed;
			DirectPaginator.PageSize = DirectPaginator.IsHorizontal() ? size.x : size.y;
			DirectPaginator.SetPage(page);
		}

		/// <summary>
		/// Set slides size.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="slides">Slides.</param>
		protected virtual void SetSlidesSize(Vector2 size, List<RectTransform> slides)
		{
			foreach (var slide in slides)
			{
				slide.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				slide.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}
		}

		/// <summary>
		/// Update slides state.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <param name="ratio">Ratio.</param>
		protected virtual void UpdateSlidesState(int slideIndex, float ratio)
		{
			// scroll in the reverse direction
			var position = DirectPaginator.GetContentSize() - DirectPaginator.GetPosition();
			ReverseContent.anchoredPosition = DirectPaginator.IsHorizontal()
				? new Vector2(-position, ReverseContent.anchoredPosition.y)
				: new Vector2(ReverseContent.anchoredPosition.x, position);

			var clamped_index = ClampDirectSlide(slideIndex);
			var reverse_index = (slideIndex == -1)
				? ReverseSlides.Count - 1
				: ClampReverseSlide(ReverseSlides.Count - clamped_index - 2);
			SetReverseSlideState(reverse_index, ratio);
			SetReverseSlideState(ClampReverseSlide(reverse_index - 1), 1f - ratio);

			SetDirectSlideState(clamped_index, ratio);
			SetDirectSlideState(ClampDirectSlide(clamped_index + 1), 1f - ratio);
		}

		/// <summary>
		/// Clamp direct slide index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <returns>Clamped index.</returns>
		protected virtual int ClampDirectSlide(int slideIndex)
		{
			if (slideIndex < 0)
			{
				slideIndex += DirectPaginator.Pages;
			}

			if (slideIndex >= DirectPaginator.Pages)
			{
				slideIndex -= DirectPaginator.Pages;
			}

			return slideIndex;
		}

		/// <summary>
		/// Clamp reverse slide index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <returns>Clamped index.</returns>
		protected virtual int ClampReverseSlide(int slideIndex)
		{
			if (slideIndex < 0)
			{
				slideIndex += ReverseSlides.Count;
			}

			if (slideIndex >= ReverseSlides.Count)
			{
				slideIndex -= ReverseSlides.Count;
			}

			return slideIndex;
		}

		/// <summary>
		/// Set state for the slide with the specified index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <param name="ratio">Ratio.</param>
		protected virtual void SetDirectSlideState(int slideIndex, float ratio)
		{
			if (CustomSetDirectSlideState != null)
			{
				if (slideIndex >= DirectSlides.Count)
				{
					slideIndex = DirectSlides.Count - 1;
				}

				CustomSetDirectSlideState(DirectChildrenTargets[slideIndex], ratio);
			}
		}

		/// <summary>
		/// Set state for the slide with the specified index.
		/// </summary>
		/// <param name="slideIndex">Slide index.</param>
		/// <param name="ratio">Ratio.</param>
		protected virtual void SetReverseSlideState(int slideIndex, float ratio)
		{
			if (CustomSetReverseSlideState != null)
			{
				CustomSetReverseSlideState(ReverseChildrenTargets[slideIndex], ratio);
			}
			else
			{
				SetReverseSlideState(ReverseSlides[slideIndex], ratio);
			}
		}

		/// <summary>
		/// Set state for the specified slide.
		/// </summary>
		/// <param name="slide">Slide.</param>
		/// <param name="ratio">Ratio.</param>
		protected void SetReverseSlideState(RectTransform slide, float ratio)
		{
			var scale = Mathf.Lerp(1f, Scale, ratio);
			slide.localScale = new Vector3(scale, scale, scale);

			if (slide.TryGetComponent<Graphic>(out var graphic))
			{
				var color = graphic.color;
				color.a = 1f - ratio;
				graphic.color = color;
			}
		}
	}
}