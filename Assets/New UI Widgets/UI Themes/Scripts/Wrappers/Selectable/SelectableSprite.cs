namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the theme property for sprite of Selectable.
	/// </summary>
	public abstract class SelectableSprite : Wrapper<Sprite, Selectable>
	{
		/// <inheritdoc/>
		protected override Sprite Get(Selectable widget) => Get(widget.spriteState);

		/// <summary>
		/// Get sprite.
		/// </summary>
		/// <param name="sprites">Sprites.</param>
		/// <returns>Sprite.</returns>
		protected abstract Sprite Get(SpriteState sprites);

		/// <inheritdoc/>
		protected override void Set(Selectable widget, Sprite value)
		{
			var sprites = widget.spriteState;
			Set(ref sprites, value);
			widget.spriteState = sprites;
		}

		/// <summary>
		/// Set sprite.
		/// </summary>
		/// <param name="sprites">Sprites.</param>
		/// <param name="value">Value.</param>
		protected abstract void Set(ref SpriteState sprites, Sprite value);

		/// <inheritdoc/>
		protected override bool Active(Selectable widget) => widget.transition == Selectable.Transition.SpriteSwap;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Selectable widget) => ShouldAttachValue(widget.spriteState);

		/// <summary>
		/// Should attach value, only for the menu "Attach Theme".
		/// </summary>
		/// <param name="sprites">Sprites</param>
		/// <returns>true if should attach value; otherwise false.</returns>
		protected virtual bool ShouldAttachValue(SpriteState sprites) => true;
	}
}