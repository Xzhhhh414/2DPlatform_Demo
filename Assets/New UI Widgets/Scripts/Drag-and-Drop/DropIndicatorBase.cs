namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class DropIndicator.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/drag-and-drop.html")]
	public class DropIndicatorBase : MonoBehaviour, IStylable
	{
		LayoutElement layoutElement;

		/// <summary>
		/// Gets the layout element.
		/// </summary>
		/// <value>The layout element.</value>
		public LayoutElement LayoutElement
		{
			get
			{
				if (layoutElement == null)
				{
					layoutElement = Utilities.RequireComponent<LayoutElement>(this);
				}

				return layoutElement;
			}
		}

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Hide indicator.
		/// </summary>
		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var img))
			{
				style.DropIndicator.Image.ApplyTo(img);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var img))
			{
				style.DropIndicator.Image.GetFrom(img);
			}

			return true;
		}
		#endregion
	}
}