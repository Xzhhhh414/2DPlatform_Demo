namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ActiveLabelColor of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightThemesActiveLabelColor : Wrapper<Color, AccordionHighlightThemes>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightThemesActiveLabelColor"/> class.
		/// </summary>
		public AccordionHighlightThemesActiveLabelColor()
		{
			Name = nameof(AccordionHighlightThemes.ActiveLabelColor);
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlightThemes widget)
		{
			return widget.ActiveLabelColor;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlightThemes widget, Color value)
		{
			widget.ActiveLabelColor = value;
		}
	}
}