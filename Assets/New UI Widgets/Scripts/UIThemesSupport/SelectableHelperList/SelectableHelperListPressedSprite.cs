namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the pressed sprite of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListPressedSprite : SelectableHelperListSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListPressedSprite"/> class.
		/// </summary>
		public SelectableHelperListPressedSprite()
		{
			Name = nameof(SelectableHelperList.SpriteState.pressedSprite);
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