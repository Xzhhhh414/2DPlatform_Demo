namespace UIWidgets
{
	using System;
	using EasyLayoutNS;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Layout utilities.
	/// </summary>
	public static class LayoutUtilities
	{
		/// <summary>
		/// Updates the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public static void UpdateLayout(LayoutGroup layout)
		{
			if (layout == null)
			{
				return;
			}

			layout.CalculateLayoutInputHorizontal();
			layout.SetLayoutHorizontal();
			layout.CalculateLayoutInputVertical();
			layout.SetLayoutVertical();
		}

		/// <summary>
		/// Updates the layouts for the specified object and all nested objects.
		/// </summary>
		/// <param name="component">Component.</param>
		public static void UpdateLayoutsRecursive(Component component)
		{
			if (component == null)
			{
				return;
			}

			UpdateLayoutsRecursive(component.transform);
		}

		/// <summary>
		/// Updates the layouts for the specified object and all nested objects.
		/// </summary>
		/// <param name="transform">Transform.</param>
		public static void UpdateLayoutsRecursive(Transform transform)
		{
			if (transform == null)
			{
				return;
			}

			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);

				UpdateLayoutsRecursive(child);
				UpdateLayout(child);
			}
		}

		static void UpdateLayout(Transform transform)
		{
			using var _ = ListPool<ILayoutElement>.Get(out var elements);
			using var __ = ListPool<ILayoutController>.Get(out var controllers);

			transform.GetComponents(elements);
			transform.GetComponents(controllers);

			foreach (var el in elements)
			{
				el.CalculateLayoutInputHorizontal();
			}

			foreach (var el in controllers)
			{
				el.SetLayoutHorizontal();
			}

			foreach (var el in elements)
			{
				el.CalculateLayoutInputVertical();
			}

			foreach (var el in controllers)
			{
				el.SetLayoutVertical();
			}
		}

		/// <summary>
		/// Set padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingLeft(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.left = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Left = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingRight(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.right = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Right = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingTop(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.top = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Top = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingBottom(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.bottom = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Bottom = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingLeft(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.left;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Left;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingRight(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.right;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Right;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingTop(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.top;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Top;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingBottom(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.bottom;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Bottom;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Is target width under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target width under layout group or fitter control; otherwise false.</returns>
		public static bool IsWidthControlled(RectTransform target)
		{
			if (target.TryGetComponent<ILayoutIgnorer>(out var ignorer) && ignorer.ignoreLayout)
			{
				return false;
			}

			if (target.TryGetComponent<ContentSizeFitter>(out var fitter) && (fitter.horizontalFit != ContentSizeFitter.FitMode.Unconstrained))
			{
				return true;
			}

			var parent = target.transform.parent as RectTransform;
			if (parent != null)
			{
				if (!parent.TryGetComponent<LayoutGroup>(out var layout_group))
				{
					return false;
				}

				if ((layout_group is GridLayoutGroup g_layout_group) && g_layout_group.enabled)
				{
					return true;
				}

				if ((layout_group is HorizontalOrVerticalLayoutGroup hv_layout_group) && hv_layout_group.enabled)
				{
					return Compatibility.GetLayoutChildControlWidth(hv_layout_group);
				}

				if ((layout_group is EasyLayout e_layout_group) && e_layout_group.enabled)
				{
					return e_layout_group.ChildrenWidth != ChildrenSize.DoNothing;
				}
			}

			return false;
		}

		/// <summary>
		/// Is target height under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target height under layout group or fitter control; otherwise false.</returns>
		public static bool IsHeightControlled(RectTransform target)
		{
			if (target.TryGetComponent<ILayoutIgnorer>(out var ignorer) && ignorer.ignoreLayout)
			{
				return false;
			}

			if (target.TryGetComponent<ContentSizeFitter>(out var fitter) && (fitter.verticalFit != ContentSizeFitter.FitMode.Unconstrained))
			{
				return true;
			}

			var parent = target.transform.parent as RectTransform;
			if (parent != null)
			{
				if (!parent.TryGetComponent<LayoutGroup>(out var layout_group))
				{
					return false;
				}

				if ((layout_group is GridLayoutGroup g_layout_group) && g_layout_group.enabled)
				{
					return true;
				}

				if ((layout_group is HorizontalOrVerticalLayoutGroup hv_layout_group) && hv_layout_group.enabled)
				{
					return Compatibility.GetLayoutChildControlHeight(hv_layout_group);
				}

				if ((layout_group is EasyLayout e_layout_group) && e_layout_group.enabled)
				{
					return e_layout_group.ChildrenHeight != ChildrenSize.DoNothing;
				}
			}

			return false;
		}
	}
}