namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the pressed sprite of SelectableHelper.
	/// </summary>
	public class SelectableHelperPressedSprite : SelectableHelperSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperPressedSprite"/> class.
		/// </summary>
		public SelectableHelperPressedSprite()
		{
			Name = nameof(SelectableHelper.SpriteState.pressedSprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(SpriteState sprites)
		{
			return sprites.pressedSprite;
		}

		/// <inheritdoc/>
		protected override void Set(ref SpriteState sprites, Sprite value)
		{
			sprites.pressedSprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SpriteState sprites)
		{
			return sprites.pressedSprite != null;
		}
	}
}