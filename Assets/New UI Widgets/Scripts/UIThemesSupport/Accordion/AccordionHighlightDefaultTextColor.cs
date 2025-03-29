namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the default text color of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightDefaultTextColor : Wrapper<Color, AccordionHighlight>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightDefaultTextColor"/> class.
		/// </summary>
		public AccordionHighlightDefaultTextColor()
		{
			Name = "DefaultTextColor";
		}

		/// <inheritdoc/>
		protected override Color Get(AccordionHighlight widget)
		{
			return widget.DefaultToggleText.Color;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlight widget, Color value)
		{
			widget.DefaultToggleText.Color = value;
		}

		/// <inheritdoc/>
		protected override bool Active(AccordionHighlight widget)
		{
			return widget.DefaultToggleText.ChangeColor;
		}
	}
}