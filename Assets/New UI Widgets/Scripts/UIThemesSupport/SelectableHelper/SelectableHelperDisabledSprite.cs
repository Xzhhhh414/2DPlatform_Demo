namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the disabled of SelectableHelper.
	/// </summary>
	public class SelectableHelperDisabledSprite : SelectableHelperSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperDisabledSprite"/> class.
		/// </summary>
		public SelectableHelperDisabledSprite()
		{
			Name = nameof(SelectableHelper.SpriteState.disabledSprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(SpriteState sprites)
		{
			return sprites.disabledSprite;
		}

		/// <inheritdoc/>
		protected override void Set(ref SpriteState sprites, Sprite value)
		{
			sprites.disabledSprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SpriteState sprites)
		{
			return sprites.disabledSprite != null;
		}
	}
}