namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the active background sprite of AccordionHighlight.
	/// </summary>
	public class AccordionHighlightActiveBackgroundSprite : Wrapper<Sprite, AccordionHighlight>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccordionHighlightActiveBackgroundSprite"/> class.
		/// </summary>
		public AccordionHighlightActiveBackgroundSprite()
		{
			Name = "ActiveBackgroundSprite";
		}

		/// <inheritdoc/>
		protected override Sprite Get(AccordionHighlight widget)
		{
			return widget.ActiveToggleBackground.Sprite;
		}

		/// <inheritdoc/>
		protected override void Set(AccordionHighlight widget, Sprite value)
		{
			widget.ActiveToggleBackground.Sprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(AccordionHighlight widget)
		{
			return widget.ActiveToggleBackground.Sprite != null;
		}
	}
}