namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// ScrollBlock resizer.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/scroll-block-resizer.html")]
	public class ScrollBlockResizer : MonoBehaviourInitiable
	{
		/// <summary>
		/// Blocks.
		/// </summary>
		[SerializeField]
		protected List<ScrollBlockBase> Blocks = new List<ScrollBlockBase>();

		/// <summary>
		/// First block.
		/// </summary>
		protected ScrollBlockBase FirstBlock => Blocks.Count > 0 ? Blocks[0] : null;

		/// <summary>
		/// Highlight.
		/// </summary>
		[SerializeField]
		protected RectTransform Highlight;

		/// <summary>
		/// Visible items.
		/// </summary>
		[SerializeField]
		protected int visibleItems;

		/// <summary>
		/// Visible items.
		/// </summary>
		public virtual int VisibleItems
		{
			get => visibleItems;

			set
			{
				if (visibleItems != value)
				{
					visibleItems = value;
					Resize();
				}
			}
		}

		/// <summary>
		/// Resize listener.
		/// </summary>
		protected ResizeListener ResizeListener;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (!Utilities.IsNull(FirstBlock))
			{
				ResizeListener = Utilities.RequireComponent<ResizeListener>(FirstBlock);
				ResizeListener.OnResizeNextFrame.AddListener(Resize);
				Resize();
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (!Utilities.IsNull(ResizeListener))
			{
				ResizeListener.OnResizeNextFrame.RemoveListener(Resize);
			}
		}

		/// <summary>
		/// Resize.
		/// </summary>
		protected virtual void Resize()
		{
			foreach (var block in Blocks)
			{
				block.SetVisibleItems(VisibleItems);
			}

			if (!Utilities.IsNull(FirstBlock) && !Utilities.IsNull(Highlight))
			{
				var axis = FirstBlock.IsHorizontal
					? RectTransform.Axis.Horizontal
					: RectTransform.Axis.Vertical;
				Highlight.SetSizeWithCurrentAnchors(axis, FirstBlock.DefaultItemSize[(int)axis]);
			}
		}
	}
}