namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class of theme property for the sprites of SelectableHelper.
	/// </summary>
	public abstract class SelectableHelperSprite : Wrapper<Sprite, SelectableHelper>
	{
		/// <inheritdoc/>
		protected override Sprite Get(SelectableHelper widget)
		{
			return Get(widget.SpriteState);
		}

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="sprites">Sprites.</param>
		/// <returns>Value.</returns>
		protected abstract Sprite Get(SpriteState sprites);

		/// <inheritdoc/>
		protected override void Set(SelectableHelper widget, Sprite value)
		{
			var sprites = widget.SpriteState;
			Set(ref sprites, value);
			widget.SpriteState = sprites;
		}

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="sprites">Sprites.</param>
		/// <param name="value">Value.</param>
		protected abstract void Set(ref SpriteState sprites, Sprite value);

		/// <inheritdoc/>
		protected override bool Active(SelectableHelper widget)
		{
			return widget.Transition == Selectable.Transition.SpriteSwap;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelper widget)
		{
			return ShouldAttachValue(widget.SpriteState);
		}

		/// <summary>
		/// Should attach value, only for the menu "Attach Theme".
		/// </summary>
		/// <param name="sprites">Sprites.</param>
		/// <returns>true if should attach value; otherwise false.</returns>
		protected virtual bool ShouldAttachValue(SpriteState sprites)
		{
			return true;
		}
	}
}