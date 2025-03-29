namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// TreeView component.
	/// </summary>
	public class TreeViewComponentWithoutLayoutGroup : TreeViewComponent
	{
		/// <summary>
		/// Padding from left border.
		/// </summary>
		[SerializeField]
		protected float PaddingLeft = 5f;

		/// <summary>
		/// Padding from right border.
		/// </summary>
		[SerializeField]
		protected float PaddingRight = 10f;

		/// <summary>
		/// Spacing.
		/// </summary>
		[SerializeField]
		protected float Spacing = 5f;

		/// <summary>
		/// Indentation RectTransform.
		/// </summary>
		[SerializeField]
		protected RectTransform IndentationRect;

		/// <summary>
		/// Toggle RectTransform.
		/// </summary>
		[NonSerialized]
		protected RectTransform ToggleRect;

		/// <summary>
		/// Icon RectTransform.
		/// </summary>
		[NonSerialized]
		protected RectTransform IconRect;

		/// <summary>
		/// Text RectTransform.
		/// </summary>
		[NonSerialized]
		protected RectTransform TextRect;

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			if (Toggle != null)
			{
				ToggleRect = Toggle.transform.parent as RectTransform;
			}

			if (Icon != null)
			{
				IconRect = Icon.transform as RectTransform;
			}

			if (TextAdapter != null)
			{
				TextRect = TextAdapter.transform as RectTransform;
			}
		}

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			base.UpdateView();

			var pos = PaddingLeft;

			if (IndentationRect != null)
			{
				IndentationRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, NodeDepth * PaddingPerLevel);
			}

			if ((IconRect != null) && (Item.Icon == null))
			{
				IconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			}

			pos = SetPosition(IndentationRect, pos, Spacing);
			pos = SetPosition(ToggleRect, pos, Spacing);
			pos = SetPosition(IconRect, pos, Spacing);

			if (TextRect != null)
			{
				TextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RectTransform.rect.width - pos - PaddingRight);
				SetPosition(TextRect, pos, Spacing);
			}
		}

		static float SetPosition(RectTransform target, float position, float spacing)
		{
			if (target == null)
			{
				return position;
			}

			var width = target.rect.width;
			target.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, position, width);

			return position + width + spacing;
		}
	}
}