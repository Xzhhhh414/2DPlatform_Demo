namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the highlighted color of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListHighlightedColor : SelectableHelperListColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListHighlightedColor"/> class.
		/// </summary>
		public SelectableHelperListHighlightedColor()
		{
			Name = nameof(SelectableHelperList.Colors.highlightedColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors)
		{
			return colors.highlightedColor;
		}

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value)
		{
			colors.highlightedColor = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelperList widget)
		{
			return widget.Colors.highlightedColor != ColorBlock.defaultColorBlock.highlightedColor;
		}
	}
}