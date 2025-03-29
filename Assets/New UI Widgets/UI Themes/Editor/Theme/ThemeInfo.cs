#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;

	/// <summary>
	/// Theme info.
	/// </summary>
	public class ThemeInfo
	{
		/// <summary>
		/// Property.
		/// </summary>
		public class Property
		{
			/// <summary>
			/// Name.
			/// </summary>
			public readonly string Name;

			/// <summary>
			/// Value type.
			/// </summary>
			public Type ValueType => attr.ValueType;

			readonly Func<Theme, Theme.IValuesWrapper> getWrapper;

			/// <summary>
			/// Get values wrapper.
			/// </summary>
			public Func<Theme, Theme.IValuesWrapper> GetWrapper => getWrapper;

			readonly UIThemes.PropertyGroupAttribute attr;

			private Property(string name, Func<Theme, Theme.IValuesWrapper> getWrapper, UIThemes.PropertyGroupAttribute attr)
			{
				Name = name;
				this.getWrapper = getWrapper;
				this.attr = attr;
			}

			/// <summary>
			/// Create field view.
			/// </summary>
			/// <param name="theme">Theme.</param>
			/// <returns>Field view.</returns>
			public FieldViewBase CreateFieldView(Theme theme)
			{
				return attr.Constructor.Invoke(new object[] { attr.UndoName, getWrapper(theme) }) as FieldViewBase;
			}

			/// <summary>
			/// Extract value type from the type of value wrapper.
			/// </summary>
			/// <param name="valuesWrapperType">Type of values wrapper.</param>
			/// <returns>Value type.</returns>
			protected static Type PropertyValueType(Type valuesWrapperType)
			{
				while (valuesWrapperType != null)
				{
					if (valuesWrapperType.IsGenericType)
					{
						var args = valuesWrapperType.GenericTypeArguments;
						if (args.Length == 1 && typeof(Theme.ValuesWrapper<>).MakeGenericType(args) == valuesWrapperType)
						{
							return args[0];
						}
					}

					valuesWrapperType = valuesWrapperType.BaseType;
				}

				return null;
			}

			/// <summary>
			/// Create property.
			/// </summary>
			/// <param name="property">Property info.</param>
			/// <returns>Property.</returns>
			public static Property Create(PropertyInfo property)
			{
				var attrs = property.GetCustomAttributes(typeof(UIThemes.PropertyGroupAttribute), true);
				foreach (var attr in attrs)
				{
					var vp = attr as UIThemes.PropertyGroupAttribute;
					if (vp.ValueType == null)
					{
						Debug.LogErrorFormat(
							"{0}: fieldView value of [ThemeProperty] of {1} should be inherited from FieldView<TValue>.",
							property.DeclaringType,
							property.Name);
						return null;
					}

					if (vp.Constructor == null)
					{
						Debug.LogErrorFormat(
							"{0}: fieldView value of [ThemeProperty] of {1} should have public constuctor with arguments (string, Theme.ValuesWrapper<TValue>).",
							property.DeclaringType,
							property.Name);
						return null;
					}

					var property_value_type = PropertyValueType(property.PropertyType);
					if (vp.ValueType != property_value_type)
					{
						Debug.LogErrorFormat(
							"{0}: Property {1} and fieldView values type does not match: {2} != {3}.",
							property.DeclaringType,
							property.Name,
							property_value_type,
							vp.ValueType);
						return null;
					}

					return new Property(property.Name, t => property.GetValue(t) as Theme.IValuesWrapper, vp);
				}

				return null;
			}
		}

		readonly List<Property> properties = new List<Property>();

		/// <summary>
		/// Properties.
		/// </summary>
		public IReadOnlyList<Property> Properties => properties;

		private ThemeInfo(Type type)
		{
			LoadProperties(type);
		}

		/// <summary>
		/// Get property by name.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <returns>Property.</returns>
		public virtual Property GetProperty(string name)
		{
			foreach (var p in Properties)
			{
				if (p.Name == name)
				{
					return p;
				}
			}

			return null;
		}

		void LoadProperties(Type type)
		{
			if (type.BaseType != null)
			{
				LoadProperties(type.BaseType);
			}

			foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
			{
				var vp = Property.Create(p);
				if (vp != null)
				{
					properties.Add(vp);
				}
			}
		}

		/// <summary>
		/// Get options count.
		/// </summary>
		/// <param name="theme">Theme.</param>
		/// <returns>Options count</returns>
		public Dictionary<string, int> OptionsCount(Theme theme)
		{
			var result = new Dictionary<string, int>();
			foreach (var p in properties)
			{
				if (!theme.IsActiveProperty(p.Name))
				{
					continue;
				}

				var wrapper = p.GetWrapper(theme);
				result[p.Name] = wrapper.Options.Count;
			}

			return result;
		}

		[DomainReloadExclude]
		static readonly Dictionary<Type, ThemeInfo> Cache = new Dictionary<Type, ThemeInfo>();

		/// <summary>
		/// Get theme information for the specified theme.
		/// </summary>
		/// <param name="theme">Theme.</param>
		/// <returns>Theme information.</returns>
		public static ThemeInfo Get(Theme theme)
		{
			var type = theme.GetType();

			if (Cache.TryGetValue(type, out var info))
			{
				return info;
			}

			info = new ThemeInfo(type);
			Cache[type] = info;

			return info;
		}
	}
}
#endif