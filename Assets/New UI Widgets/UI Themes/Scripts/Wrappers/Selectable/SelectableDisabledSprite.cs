namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the disabled sprite of Selectable.
	/// </summary>
	public class SelectableDisabledSprite : SelectableSprite
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableDisabledSprite"/> class.
		/// </summary>
		public SelectableDisabledSprite() => Name = nameof(Selectable.spriteState.disabledSprite);

		/// <inheritdoc/>
		protected override Sprite Get(SpriteState sprites) => sprites.disabledSprite;

		/// <inheritdoc/>
		protected override void Set(ref SpriteState sprites, Sprite value) => sprites.disabledSprite = value;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SpriteState sprites) => sprites.disabledSprite != null;
	}
}