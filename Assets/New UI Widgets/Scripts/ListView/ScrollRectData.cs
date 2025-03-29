namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	public abstract partial class ListViewCustomBase : ListViewBase, IAutoScroll
	{
		/// <summary>
		/// ScrollRectData.
		/// </summary>
		protected class ScrollRectData
		{
			readonly ListViewCustomBase owner;

			ScrollRect scrollRect;

			RectTransform viewport;

			/// <summary>
			/// ScrollRect.
			/// </summary>
			public ScrollRect ScrollRect
			{
				get => scrollRect;

				set
				{
					scrollRect = value;
					if (scrollRect != null)
					{
						viewport = scrollRect.viewport != null ? scrollRect.viewport : (scrollRect.transform as RectTransform);
					}
					else
					{
						viewport = null;
					}
				}
			}

			/// <summary>
			/// Size.
			/// </summary>
			public Vector2 Size
			{
				get;
				private set;
			}

			/// <summary>
			/// Scaled size.
			/// </summary>
			public Vector2 ScaledSize
			{
				get;
				private set;
			}

			/// <summary>
			/// Axis size.
			/// </summary>
			public float AxisSize => owner.IsHorizontal() ? Size.x : Size.y;

			/// <summary>
			/// Scaled axis size.
			/// </summary>
			public float ScaledAxisSize => owner.IsHorizontal() ? ScaledSize.x : ScaledSize.y;

			/// <summary>
			/// Initializes a new instance of the <see cref="ScrollRectData"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="scrollRect">ScrollRect.</param>
			public ScrollRectData(ListViewCustomBase owner, ScrollRect scrollRect)
			{
				this.owner = owner;
				ScrollRect = scrollRect;
			}

			/// <summary>
			/// Recalculate sizes.
			/// </summary>
			public void RecalculateSizes()
			{
				if (viewport == null)
				{
					Size = Vector2.one;
				}
				else
				{
					var size = viewport.rect.size;
					size.x = float.IsNaN(size.x) ? 1f : Mathf.Max(size.x, 1f);
					size.y = float.IsNaN(size.y) ? 1f : Mathf.Max(size.y, 1f);

					Size = size;
				}

				var scale = owner.Container.localScale;
				ScaledSize = new Vector2(Size.x / scale.x, Size.y / scale.y);
			}
		}
	}
}