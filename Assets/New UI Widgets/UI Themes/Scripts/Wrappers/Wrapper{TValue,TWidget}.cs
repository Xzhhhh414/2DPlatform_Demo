namespace UIThemes.Wrappers
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Base class for the widgets property wrappers.
	/// </summary>
	/// <typeparam name="TValue">Type of property value.</typeparam>
	/// <typeparam name="TWidget">Type of widget.</typeparam>
	public abstract class Wrapper<TValue, TWidget> : IWrapper<TValue>
		where TWidget : Component
	{
		/// <summary>
		/// Widget type.
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(TWidget);
			}
		}

		/// <summary>
		/// Property name.
		/// </summary>
		public string Name
		{
			get;
			protected set;
		}

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Value.</returns>
		public TValue Get(Component component)
		{
			return Get(component as TWidget);
		}

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="widget">Widget.</param>
		/// <returns>Value.</returns>
		protected abstract TValue Get(TWidget widget);

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="value">Value.</param>
		/// <param name="comparer">Value comparer.</param>
		/// <returns>true if value was changed; otherwise false.</returns>
		public bool Set(Component component, TValue value, IEqualityComparer<TValue> comparer)
		{
			if (comparer.Equals(Get(component), value))
			{
				return false;
			}

			Set(component as TWidget, value);
			return true;
		}

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="widget">Widget.</param>
		/// <param name="value">Value.</param>
		protected abstract void Set(TWidget widget, TValue value);

		/// <summary>
		/// Check is property active, like Selectable sprites should be available only if Selectable.transition is SpriteSwap.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if property active; otherwise false.</returns>
		public bool Active(Component component)
		{
			return Active(component as TWidget);
		}

		/// <summary>
		/// Check is property active.
		/// </summary>
		/// <param name="widget">Widget.</param>
		/// <returns>true if property active; otherwise false.</returns>
		protected virtual bool Active(TWidget widget)
		{
			return true;
		}

		/// <summary>
		/// Should attach value, only for the menu "Attach Theme".
		/// Like sprite should be not null for the Image component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if should attach value; otherwise false.</returns>
		public virtual bool ShouldAttachValue(Component component)
		{
			return ShouldAttachValue(component as TWidget);
		}

		/// <summary>
		/// Should attach value, only for the menu "Attach Theme".
		/// </summary>
		/// <param name="widget">Widget.</param>
		/// <returns>true if should attach value; otherwise false.</returns>
		protected virtual bool ShouldAttachValue(TWidget widget)
		{
			return true;
		}
	}
}