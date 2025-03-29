namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the default background color of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightDefaultBackgroundColor : Wrapper<Color, AccordionHighlight>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightDefaultBackgroundColor"/> class.
		/// </summary>
		public AccordionHighlightDefaultBackgroundColor()
		{
			Name = "DefaultBackgroundColor";
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlight widget)
		{
			return widget.DefaultToggleBackground.Color;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlight widget, Color value)
		{
			widget.DefaultToggleBackground.Color = value;
		}
	}
}