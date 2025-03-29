namespace UIThemes.Samples
{
	using System;
	using System.Collections.Generic;
	using UIThemes.Wrappers;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property wrapper for the rotated sprite.
	/// </summary>
	public class RotatedSpriteWrapper : IWrapper<RotatedSprite>
	{
		/// <summary>
		/// Component type.
		/// </summary>
		public Type Type => typeof(Image);

		/// <summary>
		/// Name.
		/// </summary>
		public string Name => "Sprite + Rotation";

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Value.</returns>
		public RotatedSprite Get(Component component) => new RotatedSprite(component as Image);

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="value">Value.</param>
		/// <param name="comparer">Value comparer.</param>
		/// <returns>true if value was changed; otherwise false.</returns>
		public bool Set(Component component, RotatedSprite value, IEqualityComparer<RotatedSprite> comparer) => value.Set(component as Image);

		/// <summary>
		/// Check is property active, like Selectable sprites should be available only if Selectable.transition is SpriteSwap.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if property active; otherwise false.</returns>
		public bool Active(Component component) => true;

		/// <summary>
		/// Should attach value, only for the menu "Attach Theme".
		/// Like sprite should be not null for the Image component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if should attach value; otherwise false.</returns>
		public bool ShouldAttachValue(Component component) => (component as Image).sprite != null;
	}
}