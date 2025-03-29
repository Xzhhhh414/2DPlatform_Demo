namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultBackgroundSprite of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightThemesDefaultBackgroundSprite : Wrapper<Sprite, AccordionHighlightThemes>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightThemesDefaultBackgroundSprite"/> class.
		/// </summary>
		public AccordionHighlightThemesDefaultBackgroundSprite()
		{
			Name = nameof(AccordionHighlightThemes.DefaultBackgroundSprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(AccordionHighlightThemes widget)
		{
			return widget.DefaultBackgroundSprite;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlightThemes widget, Sprite value)
		{
			widget.DefaultBackgroundSprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(AccordionHighlightThemes widget)
		{
			return widget.DefaultBackgroundSprite != null;
		}
	}
}