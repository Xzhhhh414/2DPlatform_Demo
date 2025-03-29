namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the disable sprite of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListDisabledSprite : SelectableHelperListSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListDisabledSprite"/> class.
		/// </summary>
		public SelectableHelperListDisabledSprite()
		{
			Name = nameof(SelectableHelperList.SpriteState.disabledSprite);
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