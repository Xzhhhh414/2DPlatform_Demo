#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UIThemes.Pool;
	using UIThemes.Wrappers;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Attach theme to the specified game object.
	/// </summary>
	public class ThemeAttach
	{
		readonly Theme theme;

		readonly bool showProgress;

		readonly Type targetType;

		ThemeTargetInfo targetInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThemeAttach"/> class.
		/// </summary>
		/// <param name="theme">Theme.</param>
		/// <param name="showProgress">Show progress.</param>
		public ThemeAttach(Theme theme, bool showProgress = true)
		{
			this.theme = theme;
			this.showProgress = showProgress;
			targetType = theme != null ? theme.GetTargetType() : null;
		}

		/// <summary>
		/// Attach theme to the root game object and nested one.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="createOptions">Create options.</param>
		public void Run(GameObject root, bool createOptions)
		{
			Run(new[] { root }, createOptions);
		}

		/// <summary>
		/// Attach theme to the root game object and nested one.
		/// </summary>
		/// <param name="roots">Root objects.</param>
		/// <param name="createOptions">Create options.</param>
		public void Run(IList<GameObject> roots, bool createOptions)
		{
			ThemeTargetBase.StaticInit(); // make sure to invoke static constructor

			ShowProgress("Step 1. Find GameObjects.", 0f);

			var go = new GameObject("Theme Temp");
			var target = go.AddComponent(targetType) as ThemeTargetBase;
			target.SetTheme(theme);

			using var _ = ListPool<GameObject>.Get(out var game_objects);
			try
			{
				targetInfo = ThemeTargetInfo.Get(target);

				for (var i = 0; i < roots.Count; i++)
				{
					GetGameObjects(roots[i].transform, game_objects);
				}

				Apply(game_objects, target, createOptions);
			}
			finally
			{
				ShowProgress("Step 3. Cleanup.", 1f);

				UnityEngine.Object.DestroyImmediate(go);

				EditorUtility.SetDirty(theme);
				AssetDatabase.SaveAssets();

				if (showProgress)
				{
					EditorUtility.ClearProgressBar();
				}
			}
		}

		/// <summary>
		/// Show progress.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="progress">Progress in range 0..1f.</param>
		protected virtual void ShowProgress(string message, float progress)
		{
			if (!showProgress)
			{
				return;
			}

			EditorUtility.DisplayProgressBar("Theme Attach", message, progress);
		}

		/// <summary>
		/// Get game objects in the hierarchy.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <param name="output">Output.</param>
		protected virtual void GetGameObjects(Transform transform, List<GameObject> output)
		{
			output.Add(transform.gameObject);

			for (int i = 0; i < transform.childCount; i++)
			{
				GetGameObjects(transform.GetChild(i), output);
			}
		}

		/// <summary>
		/// Apply theme.
		/// </summary>
		/// <param name="gameObjects">Game objects.</param>
		/// <param name="target">Target.</param>
		/// <param name="createOptions">Create options.</param>
		protected virtual void Apply(List<GameObject> gameObjects, ThemeTargetBase target, bool createOptions)
		{
			var total = gameObjects.Count.ToString();
			for (var i = 0; i < gameObjects.Count; i++)
			{
				var go = gameObjects[i];
				if (AlreadyAttached(go))
				{
					continue;
				}

				using var _ = ListPool<Component>.Get(out var components);

				go.GetComponents(components);

				if (ShouldAddThemeTarget(go, target, components))
				{
					var real_target = Undo.AddComponent(go, targetType) as ThemeTargetBase;
					real_target.SetTheme(theme);
					AttachValues(real_target, createOptions, false);

					UIThemes.Utilities.FindOwners(go, includeChildren: false);
					real_target.Refresh();
				}

				if (showProgress)
				{
					EditorUtility.DisplayProgressBar("Theme Attach", string.Format("Step 2. Creating ThemeTarget: {0} / {1}.", i, total), i / (float)gameObjects.Count);
				}
			}
		}

		/// <summary>
		/// Is game object has ThemeTarget with current theme?
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <returns>true if has ThemeTarget with current theme; otherwise false.</returns>
		protected virtual bool AlreadyAttached(GameObject go)
		{
			if (!go.TryGetComponent<ThemeTargetBase>(out var t))
			{
				return false;
			}

			if (t.GetTheme() == theme)
			{
				return true;
			}

			Undo.DestroyObjectImmediate(t);

			return false;
		}

		/// <summary>
		/// Should add ThemeTarget component.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="target">Target.</param>
		/// <param name="components">Components.</param>
		/// <returns>true if should add ThemeTarget component; otherwise false.</returns>
		protected virtual bool ShouldAddThemeTarget(GameObject go, ThemeTargetBase target, List<Component> components)
		{
			if (!HasTargetComponents(target, components))
			{
				return false;
			}

			return go.GetComponent(targetType) == null;
		}

		/// <summary>
		/// Has target components.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="components">Components.</param>
		/// <returns>true if components has any component with field or property to controlled by Theme; otherwise false.</returns>
		protected virtual bool HasTargetComponents(ThemeTargetBase target, List<Component> components)
		{
			ClearTargets(target);

			target.FindTargets(components);

			return TargetsCount(target) > 0;
		}

		/// <summary>
		/// Clear targets.
		/// </summary>
		/// <param name="target">Target.</param>
		protected virtual void ClearTargets(ThemeTargetBase target)
		{
			foreach (var field in targetInfo.Fields)
			{
				field.ClearTargets(target);
			}
		}

		/// <summary>
		/// Targets count.
		/// </summary>
		/// <param name="target">Targets.</param>
		/// <returns>Count.</returns>
		protected virtual int TargetsCount(ThemeTargetBase target)
		{
			var targets = 0;
			foreach (var field in targetInfo.Fields)
			{
				targets += field.GetTargets(target).Count;
			}

			return targets;
		}

		/// <summary>
		/// Attach values.
		/// </summary>
		/// <param name="themeTarget">Theme target.</param>
		/// <param name="createOptions">Create options.</param>
		/// <param name="refreshThemeWindow">Refresh theme window.</param>
		protected virtual void AttachValues(ThemeTargetBase themeTarget, bool createOptions, bool refreshThemeWindow = true)
		{
			AttachValues(themeTarget, createOptions, theme.InitialVariationId, refreshThemeWindow);
		}

		/// <summary>
		/// Attach values.
		/// </summary>
		/// <param name="themeTarget">Theme target.</param>
		/// <param name="createOptions">Create options.</param>
		/// <param name="variationId">Initial variation ID.</param>
		/// <param name="refreshThemeWindow">Refresh ThemeEditor window.</param>
		public virtual void AttachValues(ThemeTargetBase themeTarget, bool createOptions, VariationId variationId, bool refreshThemeWindow = true)
		{
			var theme = themeTarget.GetTheme();
			if (theme == null)
			{
				return;
			}

			var targetInfo = ThemeTargetInfo.Get(themeTarget);
			var theme_info = ThemeInfo.Get(theme);

			themeTarget.FindTargets();

			foreach (var field in targetInfo.Fields)
			{
				if (!theme.IsActiveProperty(field.ThemePropertyName))
				{
					continue;
				}

				var theme_property = theme_info.GetProperty(field.ThemePropertyName);
				if (theme_property == null)
				{
					continue;
				}

				var values = field.GetValues(theme);
				if (values == null)
				{
					continue;
				}

				var require_method = createOptions ? RequireOption(values) : FindOption(values);
				var getter = ValueGetter(theme_property);
				var should_attach_value = ShouldAttachValue(theme_property);

				var targets = field.GetTargets(themeTarget);
				foreach (var target in targets)
				{
					var value = getter(target);
					if (value == null)
					{
						continue;
					}

					var option_name = target.Component.name + "." + target.Property;
					target.OptionId = should_attach_value(target)
						? require_method(values, theme.InitialVariationId, value, option_name)
						: OptionId.None;
				}
			}

			if (createOptions && refreshThemeWindow)
			{
				ThemeEditor.RefreshWindow();
			}

			UtilitiesEditor.MarkDirty(themeTarget);
		}

		/// <summary>
		/// Update theme.
		/// </summary>
		/// <param name="themeTarget">Theme target.</param>
		/// <param name="variationId">Variation ID.</param>
		/// <returns>true if any theme value was changed; otherwise false.</returns>
		public virtual bool UpdateTheme(ThemeTargetBase themeTarget, VariationId variationId)
		{
			var theme = themeTarget.GetTheme();
			if (theme == null)
			{
				return false;
			}

			if (!theme.HasVariation(variationId))
			{
				return false;
			}

			targetInfo = ThemeTargetInfo.Get(themeTarget);

			var changed = false;
			using (var _ = theme.BeginUpdate())
			{
				var theme_info = ThemeInfo.Get(theme);
				foreach (var field in targetInfo.Fields)
				{
					if (!theme.IsActiveProperty(field.ThemePropertyName))
					{
						continue;
					}

					var theme_property = theme_info.GetProperty(field.ThemePropertyName);
					if (theme_property == null)
					{
						continue;
					}

					var values = field.GetValues(theme);
					if (values == null)
					{
						continue;
					}

					var set_method = SetValue(values);

					var targets = field.GetTargets(themeTarget);
					foreach (var target in targets)
					{
						if (!target.Active)
						{
							continue;
						}

						var getter = ValueGetter(theme_property);
						var value = getter(target);
						changed |= set_method(values, variationId, target.OptionId, value);
					}
				}
			}

			if (changed)
			{
				ThemeEditor.RefreshWindow();
			}

			return changed;
		}

		/// <summary>
		/// Method to set value.
		/// </summary>
		/// <param name="values">Values wrapper.</param>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <param name="value">Value.</param>
		/// <returns>true if value was added or changed; otherwise false.</returns>
		protected delegate bool SetValueMethod(Theme.IValuesWrapper values, VariationId variationId, OptionId optionId, object value);

		/// <summary>
		/// Cache of SetValueMethod.
		/// </summary>
		[DomainReloadExclude]
		protected static Dictionary<Type, SetValueMethod> SetValueCache = new Dictionary<Type, SetValueMethod>();

		/// <summary>
		/// Get method to set value.
		/// </summary>
		/// <param name="values">Values.</param>
		/// <returns>Method to set value.</returns>
		protected virtual SetValueMethod SetValue(Theme.IValuesWrapper values)
		{
			var type = values.GetType();
			if (SetValueCache.TryGetValue(type, out var set_value))
			{
				return set_value;
			}

			var method = type.GetMethod(nameof(Theme.ValuesWrapper<Color>.Set), BindingFlags.Public | BindingFlags.Instance);
			set_value = (values_wrapper, variation_id, option_id, value) => (bool)method.Invoke(values_wrapper, new[] { variation_id, option_id, value });
			SetValueCache[type] = set_value;

			return set_value;
		}

		/// <summary>
		/// Method to find option by value or add if option does not exists.
		/// </summary>
		/// <param name="values">Values.</param>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="value">Value.</param>
		/// <param name="optionName">Option name.</param>
		/// <returns>Option ID.</returns>
		protected delegate OptionId RequireOptionMethod(Theme.IValuesWrapper values, VariationId variationId, object value, string optionName);

		/// <summary>
		/// Cache of methods to find option by value or add if option does not exists.
		/// </summary>
		[DomainReloadExclude]
		protected static Dictionary<Type, RequireOptionMethod> RequireOptionCache = new Dictionary<Type, RequireOptionMethod>();

		/// <summary>
		/// Cache of methods to find option by value.
		/// </summary>
		[DomainReloadExclude]
		protected static Dictionary<Type, RequireOptionMethod> FindOptionCache = new Dictionary<Type, RequireOptionMethod>();

		/// <summary>
		/// Get method to find option by value or add if option does not exists.
		/// </summary>
		/// <param name="values">Values.</param>
		/// <returns>Method.</returns>
		protected virtual RequireOptionMethod RequireOption(Theme.IValuesWrapper values)
		{
			var type = values.GetType();
			if (RequireOptionCache.TryGetValue(type, out var require))
			{
				return require;
			}

			var method = type.GetMethod(nameof(Theme.ValuesWrapper<Color>.RequireOption), BindingFlags.Public | BindingFlags.Instance);
			require = (values_wrapper, variationId, value, option_name) => (OptionId)method.Invoke(values_wrapper, new[] { variationId, value, option_name });
			RequireOptionCache[type] = require;

			return require;
		}

		/// <summary>
		/// Get method to find option by value.
		/// </summary>
		/// <param name="values">Values.</param>
		/// <returns>Method.</returns>
		protected virtual RequireOptionMethod FindOption(Theme.IValuesWrapper values)
		{
			var type = values.GetType();
			if (FindOptionCache.TryGetValue(type, out var require))
			{
				return require;
			}

			var method = type.GetMethod(nameof(Theme.ValuesWrapper<Color>.FindOption), BindingFlags.Public | BindingFlags.Instance);
			require = (values_wrapper, variationId, value, option_name) => (OptionId)method.Invoke(values_wrapper, new[] { variationId, value });
			FindOptionCache[type] = require;

			return require;
		}

		/// <summary>
		/// Method to get property value.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Property value.</returns>
		protected delegate object PropertyGetterMethod(Target target);

		/// <summary>
		/// Cache of methods to get property value.
		/// </summary>
		[DomainReloadExclude]
		protected static Dictionary<Type, PropertyGetterMethod> PropertyGetterCache = new Dictionary<Type, PropertyGetterMethod>();

		/// <summary>
		/// Get method to get property value.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>Method.</returns>
		protected virtual PropertyGetterMethod ValueGetter(ThemeInfo.Property property)
		{
			if (PropertyGetterCache.TryGetValue(property.ValueType, out var getter))
			{
				return getter;
			}

			var type = typeof(PropertyWrappers<>).MakeGenericType(new[] { property.ValueType });
			var method = type.GetMethod(nameof(PropertyWrappers<Color>.Get), BindingFlags.Public | BindingFlags.Static);
			getter = target =>
			{
				var type_property = method.Invoke(null, new[] { target });
				if (type_property == null)
				{
					return null;
				}

				var value_getter = type_property.GetType().GetMethod(nameof(IWrapper<Color>.Get), BindingFlags.Public | BindingFlags.Instance);
				return value_getter.Invoke(type_property, new[] { target.Component });
			};
			PropertyGetterCache[property.ValueType] = getter;

			return getter;
		}

		/// <summary>
		/// Method to check if should attach property value.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if value was changed; otherwise false.</returns>
		protected delegate bool PropertyAttachValueMethod(Target target);

		/// <summary>
		/// Cache of methods to check if should attach property value.
		/// </summary>
		[DomainReloadExclude]
		protected static Dictionary<Type, PropertyAttachValueMethod> PropertyAttachValueCache = new Dictionary<Type, PropertyAttachValueMethod>();

		/// <summary>
		/// Get method to check if should attach property value.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>Method.</returns>
		protected virtual PropertyAttachValueMethod ShouldAttachValue(ThemeInfo.Property property)
		{
			if (PropertyAttachValueCache.TryGetValue(property.ValueType, out var getter))
			{
				return getter;
			}

			var type = typeof(PropertyWrappers<>).MakeGenericType(new[] { property.ValueType });
			var method = type.GetMethod(nameof(PropertyWrappers<Color>.Get), BindingFlags.Public | BindingFlags.Static);
			getter = target =>
			{
				var type_property = method.Invoke(null, new[] { target });
				if (type_property == null)
				{
					return false;
				}

				var value_getter = type_property.GetType().GetMethod(nameof(IWrapper<Color>.ShouldAttachValue), BindingFlags.Public | BindingFlags.Instance);
				return (bool)value_getter.Invoke(type_property, new[] { target.Component });
			};
			PropertyAttachValueCache[property.ValueType] = getter;

			return getter;
		}

		/// <summary>
		/// Attach theme to the specified game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="theme">Theme.</param>
		/// <param name="createOptions">Create options.</param>
		/// <param name="showProgress">Show progress.</param>
		public static void Attach(GameObject go, Theme theme, bool createOptions, bool showProgress = true)
		{
			var processor = new ThemeAttach(theme, showProgress);
			processor.Run(go, createOptions);
		}

		/// <summary>
		/// Attach clone of the theme to the specified game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="baseTheme">Theme to clone.</param>
		/// <param name="path">Path.</param>
		/// <param name="createOptions">Create options.</param>
		/// <param name="showProgress">Show progress.</param>
		public static void AttachClone(GameObject go, Theme baseTheme, string path, bool createOptions = true, bool showProgress = true)
		{
			if (baseTheme == null)
			{
				return;
			}

			path = AssetDatabase.GenerateUniqueAssetPath(path);
			var base_path = AssetDatabase.GetAssetPath(baseTheme);
			if (!AssetDatabase.CopyAsset(base_path, path))
			{
				Debug.LogError("Failed to copy theme: " + base_path + " -> " + path);
			}

			var theme = AssetDatabase.LoadAssetAtPath<Theme>(path);
			var processor = new ThemeAttach(theme, showProgress);
			processor.Run(go, createOptions);
		}

		/// <summary>
		/// Attach default theme to the specified game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="createOptions">Create options.</param>
		/// <param name="error">Error.</param>
		/// <returns>true if theme was attached; otherwise false.</returns>
		public static bool AttachDefaultTheme(GameObject go, bool createOptions, out string error)
		{
			var themes = ThemesReferences.Instance;
			if ((themes == null) || (themes.Current == null))
			{
				error = "Current theme is not specified.";
				return false;
			}

			var theme = themes.Current;
			if (!theme.HasVariation(theme.InitialVariationId))
			{
				error = "Default variation is not specified for the current theme.";
				return false;
			}

			Attach(go, theme, createOptions, showProgress: false);

			error = string.Empty;
			return true;
		}

		/// <summary>
		/// Add or replace ThemeTarget with the specified theme.
		/// </summary>
		/// <param name="theme">Theme.</param>
		/// <param name="createOptions">Create options.</param>
		public static void ReplaceWith(Theme theme, bool createOptions = true)
		{
			var processor = new ThemeAttach(theme, true);
			processor.Run(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects(), createOptions);
		}
	}
}
#endif