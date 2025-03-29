namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ActiveBackgroundColor of AccordionHighlight .
	/// </summary>
	public class AccordionHighlightThemesActiveBackgroundColor : Wrapper<Color, AccordionHighlightThemes>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightThemesActiveBackgroundColor"/> class.
		/// </summary>
		public AccordionHighlightThemesActiveBackgroundColor()
		{
			Name = nameof(AccordionHighlightThemes.ActiveBackgroundColor);
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlightThemes widget)
		{
			return widget.ActiveBackgroundColor;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlightThemes widget, Color value)
		{
			widget.ActiveBackgroundColor = value;
		}
	}
}