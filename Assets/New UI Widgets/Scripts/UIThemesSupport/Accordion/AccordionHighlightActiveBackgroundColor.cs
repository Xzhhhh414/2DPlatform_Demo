namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the active background color of AccordionHighlight .
	/// </summary>
	public class AccordionHighlightActiveBackgroundColor : Wrapper<Color, AccordionHighlight>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightActiveBackgroundColor"/> class.
		/// </summary>
		public AccordionHighlightActiveBackgroundColor()
		{
			Name = "ActiveBackgroundColor";
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlight widget)
		{
			return widget.ActiveToggleBackground.Color;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlight widget, Color value)
		{
			widget.ActiveToggleBackground.Color = value;
		}
	}
}