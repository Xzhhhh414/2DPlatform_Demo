namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultBackgroundColor of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightThemesDefaultBackgroundColor : Wrapper<Color, AccordionHighlightThemes>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightThemesDefaultBackgroundColor"/> class.
		/// </summary>
		public AccordionHighlightThemesDefaultBackgroundColor()
		{
			Name = nameof(AccordionHighlightThemes.DefaultBackgroundColor);
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlightThemes widget)
		{
			return widget.DefaultBackgroundColor;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlightThemes widget, Color value)
		{
			widget.DefaultBackgroundColor = value;
		}
	}
}