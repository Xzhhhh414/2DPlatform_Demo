#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class MenuOptions
	{
		/// <summary>
		/// Create theme.
		/// </summary>
		[MenuItem("Assets/Create/UI Themes/Theme", false)]
		public static void CreateTheme()
		{
			Theme theme;

			var default_theme = ReferencesGUIDs.DefaultTheme;
			var path = UtilitiesEditor.SelectedAssetPath() + Path.DirectorySeparatorChar + "UI Theme.asset";
			if (default_theme != null)
			{
				path = AssetDatabase.GenerateUniqueAssetPath(path);
				if (!AssetDatabase.CopyAsset(ReferencesGUIDs.DefaultThemePath, path))
				{
					Debug.LogError("Failed to copy theme: " + ReferencesGUIDs.DefaultThemePath);
				}

				theme = AssetDatabase.LoadAssetAtPath<Theme>(path);
			}
			else
			{
				theme = UtilitiesEditor.CreateScriptableObjectAsset<Theme>(path, false);
				theme.AddVariation("Initial Variation");
			}

			var refs = ThemesReferences.Instance;
			if ((refs != null) && (refs.Current == null))
			{
				refs.Current = theme;
			}

			AssetDatabase.SaveAssets();

			Selection.activeObject = theme;
			ThemeEditor.Open(theme);
		}

		static bool CanAttachTheme(out string error)
		{
			var target = Selection.activeGameObject;
			if (target == null)
			{
				error = "Gameobject is not selected.";
				return false;
			}

			var themes = ThemesReferences.Instance;
			if ((themes == null) || (themes.Current == null))
			{
				error = "Current theme is not specified.";
				return false;
			}

			var theme = themes.Current;
			if (!theme.HasVariation(theme.InitialVariationId))
			{
				error = "Initial variation is not specified for the current theme.";
				return false;
			}

			error = string.Empty;
			return true;
		}

		/// <summary>
		/// Can attach theme.
		/// </summary>
		/// <returns>true if can attach theme; otherwise false.</returns>
		[MenuItem("GameObject/UI/UI Themes/Attach Theme", true, 10)]
		public static bool CanAttachTheme()
		{
			return CanAttachTheme(out var _);
		}

		/// <summary>
		/// Attach theme.
		/// </summary>
		[MenuItem("GameObject/UI/UI Themes/Attach Theme", false, 10)]
		public static void AttachTheme()
		{
			if (!CanAttachTheme(out var error))
			{
				Debug.LogError(error);
				return;
			}

			var theme = ThemesReferences.Instance.Current;
			var target = Selection.activeGameObject;

			Undo.RegisterFullObjectHierarchyUndo(target, "Undo Theme Attach");
			ThemeAttach.Attach(target, theme, false);
			RecordPrefabInstanceModifications(target);

			ThemeEditor.RefreshWindow();

			CreateWrappers();
		}

		static void CreateWrappers()
		{
			var settings = ThemesReferences.Instance;
			if (!settings.ShouldGenerateWrappers)
			{
				return;
			}

			var gen = new WrappersGenerator(
				settings.WrappersFolder,
				settings.WrappersNamespace,
				ReflectionWrappersRegistry.All());
			gen.Run();
		}

		/// <summary>
		/// Can attach theme and create options.
		/// </summary>
		/// <returns>true if can attach theme and create options.</returns>
		[MenuItem("GameObject/UI/UI Themes/Attach Theme and Create Options", true, 20)]
		public static bool CanAttachThemeAndCreateOptions()
		{
			return CanAttachTheme(out var _);
		}

		/// <summary>
		/// Attach theme and create options.
		/// </summary>
		[MenuItem("GameObject/UI/UI Themes/Attach Theme and Create Options", false, 20)]
		public static void AttachThemeAndCreateOptions()
		{
			if (!CanAttachTheme(out var error))
			{
				Debug.LogError(error);
				return;
			}

			var theme = ThemesReferences.Instance.Current;
			var target = Selection.activeGameObject;

			var info = ThemeInfo.Get(theme);
			var options_start = info.OptionsCount(theme);

			Undo.RecordObject(theme, "Attach Theme and Create Options");
			Undo.RegisterFullObjectHierarchyUndo(target, "Attach Theme and Create Options");
			ThemeAttach.Attach(target, theme, true);
			RecordPrefabInstanceModifications(target);
			EditorUtility.SetDirty(theme);

			var options_end = info.OptionsCount(theme);
			var options_added = CalculateAddedOptions(options_start, options_end);
			var added = AddedOptions2String(options_added);
			if (!string.IsNullOrEmpty(added))
			{
				Debug.Log(added, theme);
			}

			ThemeEditor.RefreshWindow();

			CreateWrappers();
		}

		/// <summary>
		/// Can find options?
		/// </summary>
		/// <returns>true if can find options; otherwise false.</returns>
		[MenuItem("GameObject/UI/UI Themes/Find Options For All ThemeTargets", true, 23)]
		public static bool CanFindOptions()
		{
			return Selection.activeGameObject != null;
		}

		/// <summary>
		/// Find options.
		/// </summary>
		[MenuItem("GameObject/UI/UI Themes/Find Options For All ThemeTargets", false, 23)]
		public static void FindOptions()
		{
			if (!CanFindOptions())
			{
				return;
			}

			var attach = new ThemeAttach(null, false);
			var target = Selection.activeGameObject;

			Undo.RegisterFullObjectHierarchyUndo(target, "Find Options For All ThemeTargets");

			var targets = target.GetComponentsInChildren<ThemeTargetBase>(true);
			foreach (var t in targets)
			{
				var theme = t.GetTheme();
				if (theme == null)
				{
					continue;
				}

				attach.AttachValues(t, true, theme.InitialVariationId, false);
			}

			RecordPrefabInstanceModifications(target);

			ThemeTargetInspector.RefreshWindow();

			CreateWrappers();
		}

		/// <summary>
		/// Can find and create options?
		/// </summary>
		/// <returns>true if can find and create options; otherwise false.</returns>
		[MenuItem("GameObject/UI/UI Themes/Find And Create Options For All ThemeTargets", true, 26)]
		public static bool CanFindAndCreateOptions()
		{
			return Selection.activeGameObject != null;
		}

		/// <summary>
		/// Find and create options.
		/// </summary>
		[MenuItem("GameObject/UI/UI Themes/Find And Create Options For All ThemeTargets", false, 26)]
		public static void FindAndCreateOptions()
		{
			if (!CanFindAndCreateOptions())
			{
				return;
			}

			var attach = new ThemeAttach(null, false);
			var target = Selection.activeGameObject;

			Undo.RegisterFullObjectHierarchyUndo(target, "Find And Create Options For All ThemeTargets");

			var targets = target.GetComponentsInChildren<ThemeTargetBase>(true);
			foreach (var t in targets)
			{
				var theme = t.GetTheme();
				if (theme == null)
				{
					continue;
				}

				Undo.RecordObject(theme, "Find And Create Options For All ThemeTargets");

				attach.AttachValues(t, true, theme.InitialVariationId, false);

				EditorUtility.SetDirty(theme);
			}

			RecordPrefabInstanceModifications(target);

			ThemeEditor.RefreshWindow();
			ThemeTargetInspector.RefreshWindow();

			CreateWrappers();
		}

		static string AddedOptions2String(Dictionary<string, int> options)
		{
			if (options.Count == 0)
			{
				return string.Empty;
			}

			var total = "Added Options:";
			var i = 0;
			foreach (var item in options)
			{
				var template = i == 0 ? " {0} = {1}" : ", {0} = {1}";
				total += string.Format(template, item.Key, item.Value.ToString());
				i += 1;
			}

			return total;
		}

		static Dictionary<string, int> CalculateAddedOptions(Dictionary<string, int> start, Dictionary<string, int> end)
		{
			var result = new Dictionary<string, int>();
			foreach (var item in end)
			{
				if (!start.TryGetValue(item.Key, out var has))
				{
					has = 0;
				}

				var delta = item.Value - has;
				if (delta > 0)
				{
					result[item.Key] = delta;
				}
			}

			return result;
		}

		/// <summary>
		/// Remove ThemeTarget components.
		/// </summary>
		[MenuItem("GameObject/UI/UI Themes/Detach Theme", false, 30)]
		public static void RemoveThemeTargets()
		{
			var target = Selection.activeGameObject;
			if (target == null)
			{
				return;
			}

			Undo.RegisterFullObjectHierarchyUndo(target, "Undo Detach Theme");
			var temp = target.GetComponentsInChildren<ThemeTargetBase>(true);
			foreach (var t in temp)
			{
				UnityEngine.Object.DestroyImmediate(t);
			}
		}

		/// <summary>
		/// Remove ThemeTarget components with Default Theme.
		/// </summary>
		[MenuItem("GameObject/UI/UI Themes/Detach Default Theme", false, 30)]
		public static void RemoveThemeTargetsDefault()
		{
			var target = Selection.activeGameObject;
			if (target == null)
			{
				return;
			}

			var refs = ThemesReferences.Instance;
			if ((refs == null) || (refs.Current == null))
			{
				return;
			}

			var theme = refs.Current;
			Undo.RegisterFullObjectHierarchyUndo(target, "Undo Detach Default Theme");
			var temp = target.GetComponentsInChildren<ThemeTargetBase>(true);
			foreach (var t in temp)
			{
				if (UnityObjectComparer<Theme>.Instance.Equals(t.GetTheme(), theme))
				{
					UnityEngine.Object.DestroyImmediate(t);
				}
			}
		}

		[DomainReloadExclude]
		static readonly List<Component> Components = new List<Component>();

		/// <summary>
		/// Record prefab instance modifications.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RecordPrefabInstanceModifications(GameObject target)
		{
			if (PrefabUtility.IsPartOfAnyPrefab(target))
			{
				PrefabUtility.RecordPrefabInstancePropertyModifications(target);

				target.GetComponents(Components);

				foreach (var c in Components)
				{
					PrefabUtility.RecordPrefabInstancePropertyModifications(c);
				}

				Components.Clear();
			}

			var t = target.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				RecordPrefabInstanceModifications(t.GetChild(i).gameObject);
			}
		}
	}
}
#endif