namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ActiveBackgroundSprite of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightThemesActiveBackgroundSprite : Wrapper<Sprite, AccordionHighlightThemes>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightThemesActiveBackgroundSprite"/> class.
		/// </summary>
		public AccordionHighlightThemesActiveBackgroundSprite()
		{
			Name = nameof(AccordionHighlightThemes.ActiveBackgroundSprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(AccordionHighlightThemes widget)
		{
			return widget.ActiveBackgroundSprite;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlightThemes widget, Sprite value)
		{
			widget.ActiveBackgroundSprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(AccordionHighlightThemes widget)
		{
			return widget.ActiveBackgroundSprite != null;
		}
	}
}