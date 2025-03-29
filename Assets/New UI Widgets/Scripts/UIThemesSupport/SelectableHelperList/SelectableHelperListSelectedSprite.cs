namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the selected sprite of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListSelectedSprite : SelectableHelperListSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListSelectedSprite"/> class.
		/// </summary>
		public SelectableHelperListSelectedSprite()
		{
			Name = nameof(SelectableHelperList.SpriteState.selectedSprite);
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