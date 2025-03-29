namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the active text color of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightActiveTextColor : Wrapper<Color, AccordionHighlight>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightActiveTextColor"/> class.
		/// </summary>
		public AccordionHighlightActiveTextColor()
		{
			Name = "ActiveTextColor";
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlight widget)
		{
			return widget.ActiveToggleText.Color;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlight widget, Color value)
		{
			widget.ActiveToggleText.Color = value;
		}

		/// <inheritdoc/>
		protected override bool Active(AccordionHighlight widget)
		{
			return widget.ActiveToggleText.ChangeColor;
		}
	}
}