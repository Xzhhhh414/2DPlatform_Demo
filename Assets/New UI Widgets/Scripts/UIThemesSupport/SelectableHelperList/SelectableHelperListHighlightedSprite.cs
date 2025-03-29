namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the highlighted sprite of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListHighlightedSprite : SelectableHelperListSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListHighlightedSprite"/> class.
		/// </summary>
		public SelectableHelperListHighlightedSprite()
		{
			Name = nameof(SelectableHelperList.SpriteState.highlightedSprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(SpriteState sprites)
		{
			return sprites.highlightedSprite;
		}

		/// <inheritdoc/>
		protected override void Set(ref SpriteState sprites, Sprite value)
		{
			sprites.highlightedSprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SpriteState sprites)
		{
			return sprites.highlightedSprite != null;
		}
	}
}