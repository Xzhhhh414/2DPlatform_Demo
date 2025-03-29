#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using UIThemes.Pool;
	using UIThemes.Wrappers;
	using UnityEditor;
	using UnityEditor.UIElements;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// ThemeTarget inspector.
	/// </summary>
	[CustomEditor(typeof(ThemeTargetBase), true)]
	public class ThemeTargetInspector : Editor
	{
		/// <summary>
		/// Options root.
		/// </summary>
		protected VisualElement OptionsRoot;

		/// <summary>
		/// Theme field.
		/// </summary>
		protected ObjectField ThemeField;

		/// <summary>
		/// Theme target.
		/// </summary>
		protected ThemeTargetBase ThemeTarget;

		/// <summary>
		/// Target information.
		/// </summary>
		protected ThemeTargetInfo TargetInfo;

		/// <summary>
		/// Create inspector GUI.
		/// </summary>
		/// <returns>Root element.</returns>
		public override VisualElement CreateInspectorGUI()
		{
			ThemeTarget = target as ThemeTargetBase;
			if (ThemeTarget != null)
			{
				TargetInfo = ThemeTargetInfo.Get(ThemeTarget);
				ThemeTarget.FindTargets();
				ThemeTarget.Refresh();
			}

			var root = new VisualElement();
			foreach (var style in ReferencesGUIDs.GetStyleSheets())
			{
				root.styleSheets.Add(style);
			}

			root.AddToClassList("theme-target");

			ThemeField = new ObjectField("Theme");
			ThemeField.AddToClassList("theme-target-theme");
			ThemeField.objectType = ThemeTarget.ThemeType;
			ThemeField.value = ThemeTarget.GetTheme();
			ThemeField.RegisterValueChangedCallback(ev => ChangeTheme(ev.newValue as Theme));
			root.Add(ThemeField);

			OptionsRoot = new VisualElement();
			OptionsRoot.AddToClassList("theme-target-options-root");

			Refresh();

			root.Add(OptionsRoot);

			return root;
		}

		/// <summary>
		/// Refresh all ThemeTarget inspector windows.
		/// </summary>
		public static void RefreshWindow()
		{
			foreach (var window in Resources.FindObjectsOfTypeAll<ThemeTargetInspector>())
			{
				window.Refresh();
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Undo.undoRedoPerformed += HandleUndo;
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Undo.undoRedoPerformed -= HandleUndo;
		}

		/// <summary>
		/// Handle undo.
		/// </summary>
		protected virtual void HandleUndo()
		{
			Refresh();
		}

		/// <summary>
		/// Refresh.
		/// </summary>
		protected virtual void Refresh()
		{
			OptionsRoot?.Clear();

			if (ThemeTarget == null)
			{
				return;
			}

			var theme = ThemeTarget.GetTheme();
			ThemeField.value = theme;

			if ((theme == null) || (TargetInfo == null))
			{
				return;
			}

			var has_missing_components = false;

			foreach (var field in TargetInfo.Fields)
			{
				if (!theme.IsActiveProperty(field.ThemePropertyName))
				{
					continue;
				}

				var targets = field.GetTargets(ThemeTarget);
				if (targets == null)
				{
					Debug.LogErrorFormat(
						"ThemeTarget ({0}) does not have field \"{1}\".",
						ThemeTarget.GetType(),
						field.TargetName,
						ThemeTarget);
					continue;
				}

				var options = field.GetOptions(theme);
				if (options == null)
				{
					Debug.LogErrorFormat(
						"{0} does not have property \"{1}\" referenced by {2} with [TargetProperty] of {3}.",
						theme.GetType(),
						field.ThemePropertyName,
						ThemeTarget.GetType(),
						field.TargetName,
						ThemeTarget);
					continue;
				}

				has_missing_components |= HasMissingComponents(targets);

				using var _ = ListPool<Target>.Get(out var temp);

				GetActiveTargets(targets, field.GetValues(theme).ValueType, temp);
				AddOptionsView(ObjectNames.NicifyVariableName(field.TargetName), temp, options);
			}

			OptionsRoot.Add(CreateActions(has_missing_components));
		}

		/// <summary>
		/// Get active targets.
		/// </summary>
		/// <param name="targets">Targets.</param>
		/// <param name="valueType">Value type.</param>
		/// <param name="output">Active targets.</param>
		protected virtual void GetActiveTargets(IReadOnlyList<Target> targets, Type valueType, List<Target> output)
		{
			foreach (var target in targets)
			{
				var active = PropertyActiveGetter(valueType);
				if (active(target))
				{
					output.Add(target);
				}
			}
		}

		/// <summary>
		/// Properties cache.
		/// </summary>
		[DomainReloadExclude]
		protected static Dictionary<Type, Func<Target, bool>> PropertiesCache = new Dictionary<Type, Func<Target, bool>>();

		/// <summary>
		/// Get function to check if property is active.
		/// </summary>
		/// <param name="valueType">Value type.</param>
		/// <returns>Function to check if property is active.</returns>
		protected static Func<Target, bool> PropertyActiveGetter(Type valueType)
		{
			if (PropertiesCache.TryGetValue(valueType, out var result))
			{
				return result;
			}

			var type = typeof(PropertyWrappers<>).MakeGenericType(new[] { valueType });
			result = target =>
			{
				var get_method = type.GetMethod(nameof(PropertyWrappers<Color>.Get));
				var property = get_method.Invoke(null, new[] { target });
				if (property == null)
				{
					return false;
				}

				var property_method = property.GetType().GetMethod(nameof(IWrapper<Color>.Active));
				return (bool)property_method.Invoke(property, new[] { target.Component });
			};

			PropertiesCache[valueType] = result;
			return result;
		}

		/// <summary>
		/// Create action buttons.
		/// </summary>
		/// <param name="hasMissingComponents">Has missing components.</param>
		/// <returns>Root element.</returns>
		protected virtual VisualElement CreateActions(bool hasMissingComponents)
		{
			var actions = new VisualElement();
			actions.AddToClassList("theme-target-actions");

			if (hasMissingComponents)
			{
				actions.Add(Button(RemoveMissingComponents, "Remove Targets with Missing Components", "theme-target-action"));
			}

			actions.Add(Button(ApplyTheme, "Apply Theme (Active Variation)", "theme-target-action"));
			actions.Add(Button(UpdateTheme, "Update Theme Values (Active Variation)", "theme-target-action"));

			var active = new VisualElement();
			active.AddToClassList("theme-target-actions-active");
			actions.Add(active);

			active.Add(new Label("Active Variation Options"));
			active.Add(Button(FindOptionsActive, "Find", "theme-target-action"));
			active.Add(Button(RequireOptionsActive, "Find or Create", "theme-target-action"));

			var initial = new VisualElement();
			initial.AddToClassList("theme-target-actions-initial");
			actions.Add(initial);

			initial.Add(new Label("Initial Variation Options"));
			initial.Add(Button(FindOptionsInitial, "Find", "theme-target-action"));
			initial.Add(Button(RequireOptionsInitial, "Find or Create", "theme-target-action"));

			return actions;
		}

		/// <summary>
		/// Remove missing components.
		/// </summary>
		protected virtual void RemoveMissingComponents()
		{
			Undo.RecordObject(ThemeTarget, "Remove Targets with Missing Components");

			foreach (var field in TargetInfo.Fields)
			{
				field.RemoveMissingTargets(ThemeTarget);
			}

			AfterChanges();
		}

		/// <summary>
		/// Apply theme.
		/// </summary>
		protected virtual void ApplyTheme()
		{
			ThemeTarget.Refresh();
		}

		/// <summary>
		/// Update theme.
		/// </summary>
		protected virtual void UpdateTheme()
		{
			var theme = ThemeTarget.GetTheme();

			Undo.RecordObject(theme, "Update Theme by " + ThemeTarget.name);

			var processor = new ThemeAttach(theme);
			if (processor.UpdateTheme(ThemeTarget, theme.ActiveVariationId))
			{
				EditorUtility.SetDirty(theme);
			}
		}

		/// <summary>
		/// Detect options.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="create">Create options.</param>
		/// <param name="undo">Undo text.</param>
		protected virtual void DetectOptions(VariationId variationId, bool create, string undo)
		{
			Undo.RecordObject(ThemeTarget, undo);

			var theme = ThemeTarget.GetTheme();
			var processor = new ThemeAttach(theme);
			processor.AttachValues(ThemeTarget, create, variationId);

			AfterChanges();
		}

		/// <summary>
		/// Detect options in initial variation.
		/// </summary>
		protected virtual void FindOptionsInitial()
		{
			DetectOptions(ThemeTarget.GetTheme().InitialVariationId, false, "Detect Options (Initial Variation) for " + ThemeTarget.name);
		}

		/// <summary>
		/// Detect or add options in initial variation.
		/// </summary>
		protected virtual void RequireOptionsInitial()
		{
			DetectOptions(ThemeTarget.GetTheme().InitialVariationId, true, "Detect or Add Options (Initial Variation) for " + ThemeTarget.name);
		}

		/// <summary>
		/// Detect options in active variation.
		/// </summary>
		protected virtual void FindOptionsActive()
		{
			DetectOptions(ThemeTarget.GetTheme().ActiveVariationId, false, "Detect Options (Active Variation) for " + ThemeTarget.name);
		}

		/// <summary>
		/// Detect or add options in active variation.
		/// </summary>
		protected virtual void RequireOptionsActive()
		{
			DetectOptions(ThemeTarget.GetTheme().ActiveVariationId, true, "Detect or Add Options (Active Variation) for " + ThemeTarget.name);
		}

		/// <summary>
		/// Create button.
		/// </summary>
		/// <param name="action">Action on click.</param>
		/// <param name="label">Label.</param>
		/// <param name="className">Class name.</param>
		/// <returns>Button.</returns>
		protected virtual Button Button(Action action, string label, string className)
		{
			var button = new Button(action)
			{
				text = label,
			};
			button.AddToClassList(className);

			return button;
		}

		/// <summary>
		/// Add options view.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="targets">Targets.</param>
		/// <param name="options">Options.</param>
		public virtual void AddOptionsView(string label, IReadOnlyList<Target> targets, IReadOnlyList<Option> options)
		{
			var block = CreateOptionsView(label, targets, options);
			if (block == null)
			{
				return;
			}

			OptionsRoot.Add(block);
		}

		/// <summary>
		/// Create options view.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="targets">Targets.</param>
		/// <param name="options">Options.</param>
		/// <returns>Root element.</returns>
		protected virtual VisualElement CreateOptionsView(string label, IReadOnlyList<Target> targets, IReadOnlyList<Option> options)
		{
			if (targets.Count == 0)
			{
				return null;
			}

			var root = new VisualElement();
			root.AddToClassList("theme-target-options");

			var label_block = new Label(label);
			label_block.AddToClassList("theme-target-options-label");
			root.Add(label_block);

			foreach (var target in targets)
			{
				root.Add(TargetView(target, options));
			}

			return root;
		}

		/// <summary>
		/// Check is any target has missing component reference.
		/// </summary>
		/// <param name="targets">Targets.</param>
		/// <returns>true if any target has missing component reference; otherwise false.</returns>
		protected virtual bool HasMissingComponents(IReadOnlyList<Target> targets)
		{
			foreach (var target in targets)
			{
				if (target.MissingComponent)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Create target view.
		/// </summary>
		/// <param name="optionTarget">Target.</param>
		/// <param name="options">Options.</param>
		/// <returns>Root element.</returns>
		protected virtual VisualElement TargetView(Target optionTarget, IReadOnlyList<Option> options)
		{
			var root = new VisualElement();
			root.AddToClassList("theme-target-option");

			var enabled = new Toggle
			{
				value = optionTarget.Enabled,
			};
			enabled.AddToClassList("theme-target-option-enabled");
			enabled.RegisterValueChangedCallback(ev => ChangeEnabled(optionTarget, ev.newValue));
			enabled.SetEnabled(optionTarget.Owner == null);
			root.Add(enabled);

			var type = ObjectNames.NicifyVariableName(optionTarget.Component.GetType().Name);

			var component = new ObjectField
			{
				objectType = optionTarget.Component.GetType(),
				value = optionTarget.Component,
				tooltip = type,
			};
			component.AddToClassList("theme-target-option-component");
			component.SetEnabled(false);
			root.Add(component);

			var label = ObjectNames.NicifyVariableName(optionTarget.Property);
			var property = new TextField
			{
				value = optionTarget.Property,
				tooltip = label,
			};
			property.AddToClassList("theme-target-option-property");
			property.SetEnabled(false);
			root.Add(property);

			if (optionTarget.Owner != null)
			{
				var button = new Button(() => EditorGUIUtility.PingObject(optionTarget.Owner))
				{
					text = "Controlled by " + optionTarget.Owner.name,
				};
				button.AddToClassList("theme-target-option-id");
				root.Add(button);
			}
			else
			{
				var combobox = new ComboboxField(optionTarget.OptionId, options);
				combobox.AddToClassList("theme-target-option-id");
				combobox.RegisterValueChangedCallback(ev => ChangeOptionId(optionTarget, ev.newValue));
				combobox.SetEnabled(true);
				root.Add(combobox);
			}

			return root;
		}

		/// <summary>
		/// Change theme.
		/// </summary>
		/// <param name="theme">Theme.</param>
		protected virtual void ChangeTheme(Theme theme)
		{
			Undo.RecordObject(ThemeTarget, "UIThemeTarget Change Theme");

			ThemeTarget.SetTheme(theme);

			AfterChanges();
		}

		/// <summary>
		/// Change enabled.
		/// </summary>
		/// <param name="optionTarget">Target.</param>
		/// <param name="enabled">Enabled.</param>
		protected virtual void ChangeEnabled(Target optionTarget, bool enabled)
		{
			Undo.RecordObject(ThemeTarget, "UIThemeTarget Toggle Enabled");

			optionTarget.Enabled = enabled;

			AfterChanges();
		}

		/// <summary>
		/// Change option ID.
		/// </summary>
		/// <param name="optionTarget">Target.</param>
		/// <param name="id">Option ID.</param>
		protected virtual void ChangeOptionId(Target optionTarget, OptionId id)
		{
			Undo.RecordObject(ThemeTarget, "UIThemeTarget Change OptionId");

			optionTarget.OptionId = id;

			AfterChanges();
		}

		/// <summary>
		/// Process after changes was done.
		/// </summary>
		protected virtual void AfterChanges()
		{
			UtilitiesEditor.MarkDirty(ThemeTarget);
			Refresh();
			ThemeTarget.Refresh();
		}
	}
}
#endif