namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the highlighted color of SelectableHelper.
	/// </summary>
	public class SelectableHelperHighlightedColor : SelectableHelperColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperHighlightedColor"/> class.
		/// </summary>
		public SelectableHelperHighlightedColor()
		{
			Name = nameof(SelectableHelper.Colors.highlightedColor);
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
		protected override bool ShouldAttachValue(SelectableHelper widget)
		{
			return widget.Colors.highlightedColor != ColorBlock.defaultColorBlock.highlightedColor;
		}
	}
}