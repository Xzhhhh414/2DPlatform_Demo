namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TableHeader cell info.
	/// </summary>
	[Serializable]
	public class TableHeaderCellInfo
	{
		/// <summary>
		/// The cell RectTransform component.
		/// </summary>
		public RectTransform Rect;

		/// <summary>
		/// The cell LayoutElement component.
		/// </summary>
		public LayoutElement LayoutElement;

		/// <summary>
		/// The cell position.
		/// </summary>
		public int Position;

		/// <summary>
		/// Gets the cell width.
		/// </summary>
		/// <value>The width.</value>
		public float Width => Rect.rect.width;

		/// <summary>
		/// Gets the cell height.
		/// </summary>
		/// <value>The height.</value>
		public float Height => Rect.rect.height;

		/// <summary>
		/// Gets a value indicating whether this gameobject active self.
		/// </summary>
		/// <value><c>true</c> if active self; otherwise, <c>false</c>.</value>
		public bool ActiveSelf => Rect.gameObject.activeSelf;

		/// <summary>
		/// Set width.
		/// </summary>
		/// <param name="width">Width.</param>
		public virtual void SetWidth(float width)
		{
			if (LayoutElement != null)
			{
				LayoutElement.preferredWidth = width;
			}

			Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		/// <summary>
		/// Set height.
		/// </summary>
		/// <param name="height">Height.</param>
		public virtual void SetHeight(float height)
		{
			if (LayoutElement != null)
			{
				LayoutElement.preferredHeight = height;
			}

			Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
	}
}