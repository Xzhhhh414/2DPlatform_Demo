namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the highlighted sprite of Selectable.
	/// </summary>
	public class SelectableHighlightedSprite : SelectableSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHighlightedSprite"/> class.
		/// </summary>
		public SelectableHighlightedSprite() => Name = nameof(Selectable.spriteState.highlightedSprite);

		/// <inheritdoc/>
		protected override Sprite Get(SpriteState sprites) => sprites.highlightedSprite;

		/// <inheritdoc/>
		protected override void Set(ref SpriteState sprites, Sprite value) => sprites.highlightedSprite = value;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SpriteState sprites) => sprites.highlightedSprite != null;
	}
}