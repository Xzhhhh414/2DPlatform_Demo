namespace UIThemes.Wrappers
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Theme property interface.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	public interface IWrapper<TValue>
	{
		/// <summary>
		/// Component type.
		/// </summary>
		Type Type
		{
			get;
		}

		/// <summary>
		/// Property name.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Value.</returns>
		TValue Get(Component component);

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="value">Value.</param>
		/// <param name="comparer">Value comparer.</param>
		/// <returns>true if value was changed; otherwise false.</returns>
		bool Set(Component component, TValue value, IEqualityComparer<TValue> comparer);

		/// <summary>
		/// Check is property active.
		/// If false then the property will not be available to the ThemeTarget list.
		/// Example: ``Selectable`` sprites properties should be available only if ``Selectable.transition`` is ``SpriteSwap``.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if property active; otherwise false.</returns>
		bool Active(Component component);

		/// <summary>
		/// If true then try to find or create value in options (only when using menu "Attach Theme").
		/// If false then the ThemeTarget option will be None.
		/// Example: if Image component sprite is null then it should not be controlled by ThemeTarget by default.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if should attach value; otherwise false.</returns>
		bool ShouldAttachValue(Component component);
	}
}