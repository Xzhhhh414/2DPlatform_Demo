namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultLabelColor of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightThemesDefaultLabelColor : Wrapper<Color, AccordionHighlightThemes>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightThemesDefaultLabelColor"/> class.
		/// </summary>
		public AccordionHighlightThemesDefaultLabelColor()
		{
			Name = nameof(AccordionHighlightThemes.DefaultLabelColor);
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlightThemes widget)
		{
			return widget.DefaultLabelColor;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlightThemes widget, Color value)
		{
			widget.DefaultLabelColor = value;
		}
	}
}