namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ModalSprite of SplitButton.
	/// </summary>
	public class SplitButtonModalSprite : Wrapper<Sprite, SplitButton>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SplitButtonModalSprite"/> class.
		/// </summary>
		public SplitButtonModalSprite()
		{
			Name = nameof(SplitButton.ModalSprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(SplitButton widget)
		{
			return widget.ModalSprite;
		}

		/// <inheritdoc/>
		protected override void Set(SplitButton widget, Sprite value)
		{
			widget.ModalSprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SplitButton widget)
		{
			return widget.ModalSprite != null;
		}
	}
}