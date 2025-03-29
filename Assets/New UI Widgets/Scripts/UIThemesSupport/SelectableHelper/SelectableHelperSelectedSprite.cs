namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the selected sprite of SelectableHelper.
	/// </summary>
	public class SelectableHelperSelectedSprite : SelectableHelperSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperSelectedSprite"/> class.
		/// </summary>
		public SelectableHelperSelectedSprite()
		{
			Name = nameof(SelectableHelper.SpriteState.selectedSprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(SpriteState sprites)
		{
			return sprites.selectedSprite;
		}

		/// <inheritdoc/>
		protected override void Set(ref SpriteState sprites, Sprite value)
		{
			sprites.selectedSprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SpriteState sprites)
		{
			return sprites.selectedSprite != null;
		}
	}
}