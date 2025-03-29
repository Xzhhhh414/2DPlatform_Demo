namespace UIThemes
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Property wrappers.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	public static class PropertyWrappers<TValue>
	{
		[DomainReloadExclude]
		static readonly Dictionary<Type, Dictionary<string, IWrapper<TValue>>> TypeReflectionWrappers = new Dictionary<Type, Dictionary<string, IWrapper<TValue>>>();

		[DomainReloadExclude]
		static readonly Dictionary<Type, Dictionary<string, IWrapper<TValue>>> TypeRegistryWrappers = new Dictionary<Type, Dictionary<string, IWrapper<TValue>>>();

		[DomainReloadExclude]
		static readonly Dictionary<Type, HashSet<string>> IgnoreProperties = new Dictionary<Type, HashSet<string>>();

		[DomainReloadExclude]
		static readonly HashSet<Type> ProcessedTypes = new HashSet<Type>();

		/// <summary>
		/// Is property ignored?
		/// </summary>
		/// <param name="type">Component type.</param>
		/// <param name="property">Property.</param>
		/// <returns>true if property ignored; otherwise false.</returns>
		public static bool Ignore(Type type, string property)
		{
			if (!IgnoreProperties.TryGetValue(type, out var properties))
			{
				return false;
			}

			return properties.Contains(property);
		}

		/// <summary>
		/// Add property to ignore.
		/// </summary>
		/// <param name="type">Component type.</param>
		/// <param name="property">Property.</param>
		public static void AddIgnore(Type type, string property)
		{
			if (!IgnoreProperties.TryGetValue(type, out var properties))
			{
				properties = new HashSet<string>();
				IgnoreProperties[type] = properties;
			}

			properties.Add(property);
		}

		/// <summary>
		/// Try get property wrapper.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="value">Property wrapper.</param>
		/// <returns>true if property wrapper exists; otherwise false.</returns>
		public static bool TryGetValue(Target target, out IWrapper<TValue> value)
		{
			return TryGetValue(target.Component.GetType(), target.Property, out value);
		}

		/// <summary>
		/// Try get property wrapper.
		/// </summary>
		/// <param name="type">Component type.</param>
		/// <param name="property">Component name.</param>
		/// <param name="value">Property wrapper.</param>
		/// <returns>true if property wrapper exists; otherwise false.</returns>
		public static bool TryGetValue(Type type, string property, out IWrapper<TValue> value)
		{
			value = null;
			foreach (var kv in TypeRegistryWrappers)
			{
				if (kv.Key.IsAssignableFrom(type) && kv.Value.TryGetValue(property, out value))
				{
					return true;
				}
			}

			foreach (var kv in TypeReflectionWrappers)
			{
				if (kv.Key.IsAssignableFrom(type) && kv.Value.TryGetValue(property, out value))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get property wrapper.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Property wrapper.</returns>
		public static IWrapper<TValue> Get(Target target)
		{
			if (TryGetValue(target, out var result))
			{
				return result;
			}

			var type = target.Component.GetType();
			var property = Wrapper<TValue>.Create(type, target.Property);
			if (property == null)
			{
				return null;
			}

			AddReflectionProperty(property);
			return property;
		}

		static bool HasRegisteredProperty(Type type, string property)
		{
			foreach (var kv in TypeRegistryWrappers)
			{
				if (kv.Key.IsAssignableFrom(type) && kv.Value.ContainsKey(property))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Has property wrapper?
		/// </summary>
		/// <param name="type">Component type.</param>
		/// <param name="property">Property name.</param>
		/// <returns>true if has property; otherwise false.</returns>
		public static bool Has(Type type, string property)
		{
			foreach (var kv in IgnoreProperties)
			{
				if (kv.Key.IsAssignableFrom(type) && kv.Value.Contains(property))
				{
					return false;
				}
			}

			foreach (var kv in TypeRegistryWrappers)
			{
				if (kv.Key.IsAssignableFrom(type) && kv.Value.ContainsKey(property))
				{
					return true;
				}
			}

			foreach (var kv in TypeReflectionWrappers)
			{
				if (kv.Key.IsAssignableFrom(type) && kv.Value.ContainsKey(property))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Add property wrapper.
		/// </summary>
		/// <param name="property">Property wrapper.</param>
		public static void Add(IWrapper<TValue> property)
		{
			if (!TypeRegistryWrappers.TryGetValue(property.Type, out var wrappers))
			{
				wrappers = new Dictionary<string, IWrapper<TValue>>();
				TypeRegistryWrappers[property.Type] = wrappers;
			}

			if (string.IsNullOrEmpty(property.Name))
			{
				Debug.LogWarning("Cannot add property without name: " + property.GetType());
				return;
			}

			wrappers[property.Name] = property;
		}

		static void AddReflectionProperty(IWrapper<TValue> property)
		{
			if (!TypeReflectionWrappers.TryGetValue(property.Type, out var wrappers))
			{
				wrappers = new Dictionary<string, IWrapper<TValue>>();
				TypeReflectionWrappers[property.Type] = wrappers;
			}

			wrappers[property.Name] = property;
		}

		/// <summary>
		/// Find and add properties wrappers.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="targets">Targets.</param>
		public static void Find(Component component, TargetsWrapper targets)
		{
			if (component == null)
			{
				return;
			}

			var original = component.GetType();
			var type = original;
			while (type != typeof(Component))
			{
				FindExistingWrappers(component, targets, type);

				type = type.BaseType;
			}

			FindOfType(component, targets, original);
		}

		/// <summary>
		/// Find and add properties wrappers.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="targets">Targets.</param>
		/// <param name="type">Type.</param>
		static void FindOfType(Component component, TargetsWrapper targets, Type type)
		{
			if (!ProcessedTypes.Add(type))
			{
				return;
			}

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
			foreach (var field in type.GetFields(flags))
			{
				if (field.FieldType != typeof(TValue))
				{
					continue;
				}

				if (Ignore(field.DeclaringType, field.Name))
				{
					continue;
				}

				if (field.GetCustomAttribute<ObsoleteAttribute>() != null)
				{
					continue;
				}

				if (HasRegisteredProperty(type, field.Name))
				{
					continue;
				}

				targets.Add(component, field.Name);
				AddReflectionProperty(new Wrapper<TValue>(field));
			}

			foreach (var property in type.GetProperties(flags))
			{
				if (property.PropertyType != typeof(TValue))
				{
					continue;
				}

				var getter = property.GetGetMethod(false);
				if (getter.GetBaseDefinition() != getter)
				{
					// ignore overridden properties because they are processed from the base type
					continue;
				}

				if (Ignore(property.DeclaringType, property.Name))
				{
					continue;
				}

				if (property.GetCustomAttribute<ObsoleteAttribute>() != null)
				{
					continue;
				}

				if (HasRegisteredProperty(type, property.Name))
				{
					continue;
				}

				targets.Add(component, property.Name);
				AddReflectionProperty(new Wrapper<TValue>(property));
			}

			if (type.BaseType != null)
			{
				FindOfType(component, targets, type.BaseType);
			}
		}

		static void FindExistingWrappers(Component component, TargetsWrapper targets, Type type)
		{
			if (TypeRegistryWrappers.TryGetValue(type, out var wrappers))
			{
				foreach (var property in wrappers.Keys)
				{
					targets.Add(component, property);
				}
			}

			if (TypeReflectionWrappers.TryGetValue(type, out wrappers))
			{
				foreach (var property in wrappers.Keys)
				{
					targets.Add(component, property);
				}
			}
		}

		/// <summary>
		/// Clear properties wrappers.
		/// </summary>
		public static void Clear()
		{
			TypeRegistryWrappers.Clear();
			TypeReflectionWrappers.Clear();
		}
	}
}