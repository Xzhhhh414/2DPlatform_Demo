namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the selected sprite of Selectable.
	/// </summary>
	public class SelectableSelectedSprite : SelectableSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableSelectedSprite"/> class.
		/// </summary>
		public SelectableSelectedSprite() => Name = nameof(Selectable.spriteState.selectedSprite);

		/// <inheritdoc/>
		protected override Sprite Get(SpriteState sprites) => sprites.selectedSprite;

		/// <inheritdoc/>
		protected override void Set(ref SpriteState sprites, Sprite value) => sprites.selectedSprite = value;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SpriteState sprites) => sprites.selectedSprite != null;
	}
}