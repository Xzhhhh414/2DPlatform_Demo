namespace UIThemes.Wrappers
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;

	/// <summary>
	/// Theme wrapper for the reflected fields/properties.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	public class Wrapper<TValue> : IWrapper<TValue>, IWrapperValidation
	{
		delegate TValue Getter(Component component);

		delegate void Setter(Component component, TValue value);

		readonly Getter get;

		readonly Setter set;

		/// <summary>
		/// Component type.
		/// </summary>
		public Type Type
		{
			get;
			protected set;
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
		/// Is property/field type is same as TValue?
		/// </summary>
		public Func<Component, bool> IsValidType
		{
			get;
			private set;
		}

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Value.</returns>
		public TValue Get(Component component) => get(component);

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="value">Value.</param>
		/// <param name="comparer">Comparer.</param>
		/// <returns>true if value was changed; otherwise false.</returns>
		public bool Set(Component component, TValue value, IEqualityComparer<TValue> comparer)
		{
			if (comparer.Equals(Get(component), value))
			{
				return false;
			}

			set(component, value);
			return true;
		}

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
		public virtual bool ShouldAttachValue(Component component) => true;

		/// <summary>
		/// Initializes a new instance of the <see cref="Wrapper{TValue}"/> class.
		/// </summary>
		/// <param name="field">Field information.</param>
		public Wrapper(FieldInfo field)
		{
			Type = field.DeclaringType;
			Name = field.Name;

			ReflectionWrappersRegistry.Add(Type, Name);

			IsValidType = component => field.GetValue(component).GetType() == typeof(TValue);
			get = component => (TValue)field.GetValue(component);
			set = (component, value) => field.SetValue(component, value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Wrapper{TValue}"/> class.
		/// </summary>
		/// <param name="property">Property information.</param>
		public Wrapper(PropertyInfo property)
		{
			Type = property.DeclaringType;
			Name = property.Name;

			ReflectionWrappersRegistry.Add(Type, Name);

			IsValidType = component => property.GetValue(component).GetType() == typeof(TValue);
			get = component => (TValue)property.GetValue(component);
			set = (component, value) => property.SetValue(component, value);
		}

		/// <summary>
		/// Create theme property.
		/// </summary>
		/// <param name="type">Component type.</param>
		/// <param name="property">Property name.</param>
		/// <returns>Property wrapper.</returns>
		public static Wrapper<TValue> Create(Type type, string property)
		{
			var field_info = type.GetField(property);
			if (field_info != null)
			{
				return new Wrapper<TValue>(field_info);
			}

			var property_info = type.GetProperty(property);
			if (property_info != null)
			{
				return new Wrapper<TValue>(property_info);
			}

			return null;
		}
	}
}