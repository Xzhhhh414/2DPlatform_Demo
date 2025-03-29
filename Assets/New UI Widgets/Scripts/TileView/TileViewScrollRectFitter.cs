namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// TileView ScrollRect fitter. Resize ScrollRect to fit whole number of items.
	/// </summary>
	[RequireComponent(typeof(ListViewBase))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/collections/tileview-scrollrectfitter.html")]
	public class TileViewScrollRectFitter : MonoBehaviourInitiable
	{
		ListViewBase tileView;

		/// <summary>
		/// Gets the TileView.
		/// </summary>
		/// <value>The TileView.</value>
		public ListViewBase TileView
		{
			get
			{
				if (tileView == null)
				{
					TryGetComponent(out tileView);
				}

				return tileView;
			}
		}

		/// <summary>
		/// The base size delta.
		/// </summary>
		protected Vector2 BaseSizeDelta;

		/// <summary>
		/// The resize listener.
		/// </summary>
		protected ResizeListener ResizeListener;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			var scrollRect = TileView.GetScrollRect();

			if (scrollRect != null)
			{
				ResizeListener = Utilities.RequireComponent<ResizeListener>(scrollRect);

				BaseSizeDelta = (scrollRect.transform as RectTransform).sizeDelta;

				ApplyFitter();
			}
		}

		/// <summary>
		/// Add resize listener.
		/// </summary>
		protected void ResizeListenerOn()
		{
			ResizeListener.OnResizeNextFrame.AddListener(ApplyFitter);
		}

		/// <summary>
		/// Remove resize listener.
		/// </summary>
		protected void ResizeListenerOff()
		{
			ResizeListener.OnResizeNextFrame.RemoveListener(ApplyFitter);
		}

		/// <summary>
		/// Applies the fitter.
		/// </summary>
		public virtual void ApplyFitter()
		{
			ResizeListenerOff();

			var spacing_x = TileView.GetItemSpacingX();
			var spacing_y = TileView.GetItemSpacingY();

			var scrollRect = TileView.GetScrollRect();
			var scrollRectTransform = scrollRect.transform as RectTransform;
			var size = scrollRectTransform.rect.size;

			var margin = TileView.GetLayoutMargin();
			size += BaseSizeDelta - scrollRectTransform.sizeDelta;
			size.x -= margin.x + margin.y;
			size.y -= margin.z + margin.w;

			var item_width = TileView.GetDefaultItemWidth();
			var items_in_row = TileView.IsHorizontal()
				? Mathf.Max(1, Mathf.FloorToInt((size.x + spacing_x) / (item_width + spacing_x)))
				: TileView.GetItemsPerBlock();
			var required_width = (items_in_row * (item_width + spacing_x)) + margin.x + margin.y;

			if (size.x != required_width)
			{
				scrollRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, required_width);
			}

			var item_height = TileView.GetDefaultItemHeight();
			var items_in_column = TileView.IsHorizontal()
				? TileView.GetItemsPerBlock()
				: Mathf.Max(1, Mathf.FloorToInt((size.y + spacing_y) / (item_height + spacing_y)));
			var required_height = (items_in_column * (item_height + spacing_y)) + margin.z;

			if (size.y != required_height)
			{
				scrollRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, required_height);
			}

			ResizeListenerOn();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		public virtual void OnDestroy()
		{
			if (ResizeListener != null)
			{
				ResizeListenerOff();
			}
		}
	}
}