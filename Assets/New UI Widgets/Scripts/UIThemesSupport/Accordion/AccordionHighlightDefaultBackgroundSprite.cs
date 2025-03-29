namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the default background sprite of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightDefaultBackgroundSprite : Wrapper<Sprite, AccordionHighlight>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightDefaultBackgroundSprite"/> class.
		/// </summary>
		public AccordionHighlightDefaultBackgroundSprite()
		{
			Name = "DefaultBackgroundSprite";
		}

		/// <inheritdoc/>
		protected override Sprite Get(AccordionHighlight widget)
		{
			return widget.DefaultToggleBackground.Sprite;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlight widget, Sprite value)
		{
			widget.DefaultToggleBackground.Sprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(AccordionHighlight widget)
		{
			return widget.DefaultToggleBackground.Sprite != null;
		}
	}
}