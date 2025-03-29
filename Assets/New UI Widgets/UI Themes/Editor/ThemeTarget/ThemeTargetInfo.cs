#if UNITY_EDITOR
namespace UIThemes
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Target information for the Theme.
	/// </summary>
	public class ThemeTargetInfo
	{
		/// <summary>
		/// Field.
		/// </summary>
		public class Field
		{
			/// <summary>
			/// Target name.
			/// </summary>
			public readonly string TargetName;

			/// <summary>
			/// Theme property name.
			/// </summary>
			public readonly string ThemePropertyName;

			/// <summary>
			/// Declared type.
			/// </summary>
			public readonly Type DeclaredType;

			readonly Func<ThemeTargetBase, List<Target>> getValue;

			readonly Predicate<Target> HasMissing = x => x.MissingComponent;

			private Field(
				string targetName,
				string themePropertyName,
				Func<ThemeTargetBase, List<Target>> getValue,
				Type declaredType)
			{
				TargetName = targetName;
				ThemePropertyName = themePropertyName;
				this.getValue = getValue;
				DeclaredType = declaredType;
			}

			/// <summary>
			/// Get targets.
			/// </summary>
			/// <param name="themeTargetBase">Theme target.</param>
			/// <returns>Targets.</returns>
			public IReadOnlyList<Target> GetTargets(ThemeTargetBase themeTargetBase)
			{
				return getValue(themeTargetBase);
			}

			/// <summary>
			/// Clear targets.
			/// </summary>
			/// <param name="themeTargetBase">Theme target.</param>
			public void ClearTargets(ThemeTargetBase themeTargetBase)
			{
				getValue(themeTargetBase).Clear();
			}

			/// <summary>
			/// Remove missing targets.
			/// </summary>
			/// <param name="themeTargetBase">Theme target.</param>
			public void RemoveMissingTargets(ThemeTargetBase themeTargetBase)
			{
				getValue(themeTargetBase)?.RemoveAll(HasMissing);
			}

			/// <summary>
			/// Get values.
			/// </summary>
			/// <param name="theme">Theme.</param>
			/// <returns>Values wrapper.</returns>
			public Theme.IValuesWrapper GetValues(Theme theme)
			{
				var property = theme.GetType().GetProperty(ThemePropertyName);
				if (property == null)
				{
					return null;
				}

				return property.GetValue(theme) as Theme.IValuesWrapper;
			}

			/// <summary>
			/// Get options.
			/// </summary>
			/// <param name="theme">Theme.</param>
			/// <returns>Options.</returns>
			public IReadOnlyList<Option> GetOptions(Theme theme)
			{
				var property = theme.GetType().GetProperty(ThemePropertyName);
				if (property == null)
				{
					return null;
				}

				if (!(property.GetValue(theme) is Theme.IValuesWrapper wrapper))
				{
					return null;
				}

				return wrapper.Options;
			}

			/// <summary>
			/// Create field information.
			/// </summary>
			/// <param name="field">Field.</param>
			/// <returns>Field information.</returns>
			public static Field Create(FieldInfo field)
			{
				var attrs = field.GetCustomAttributes(typeof(ThemePropertyAttribute), true);
				foreach (var attr in attrs)
				{
					var vp = attr as ThemePropertyAttribute;
					return new Field(field.Name, vp.ThemeProperty, t => field.GetValue(t) as List<Target>, field.DeclaringType);
				}

				return null;
			}
		}

		readonly List<Field> fields = new List<Field>();

		/// <summary>
		/// Fields.
		/// </summary>
		public IReadOnlyList<Field> Fields => fields;

		private ThemeTargetInfo(Type type)
		{
			LoadFields(type);
		}

		/// <summary>
		/// Find field by property name.
		/// </summary>
		/// <param name="themePropertyName">Property name.</param>
		/// <returns>Field.</returns>
		public Field FindField(string themePropertyName)
		{
			foreach (var field in Fields)
			{
				if (field.ThemePropertyName == themePropertyName)
				{
					return field;
				}
			}

			return null;
		}

		/// <summary>
		/// Check if target uses property with specified option ID.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="themePropertyName">Property name.</param>
		/// <param name="optionId">Option ID.</param>
		/// <returns>true if target uses property with specified option ID; otherwise false.</returns>
		public bool UseOption(ThemeTargetBase target, string themePropertyName, OptionId optionId)
		{
			var property = FindField(themePropertyName);
			if (property == null)
			{
				return false;
			}

			foreach (var t in property.GetTargets(target))
			{
				if (t.OptionId == optionId)
				{
					return true;
				}
			}

			return false;
		}

		void LoadFields(Type type)
		{
			if (type.BaseType != null)
			{
				LoadFields(type.BaseType);
			}

			foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
			{
				var info = Field.Create(field);
				if (info != null)
				{
					fields.Add(info);
				}
			}
		}

		[DomainReloadExclude]
		static readonly Dictionary<Type, ThemeTargetInfo> Cache = new Dictionary<Type, ThemeTargetInfo>();

		/// <summary>
		/// Get target information for the specified target.
		/// </summary>
		/// <param name="themeTarget">Target.</param>
		/// <returns>Target information.</returns>
		public static ThemeTargetInfo Get(ThemeTargetBase themeTarget)
		{
			var type = themeTarget.GetType();

			if (Cache.TryGetValue(type, out var info))
			{
				return info;
			}

			info = new ThemeTargetInfo(type);
			Cache[type] = info;

			return info;
		}
	}
}
#endif