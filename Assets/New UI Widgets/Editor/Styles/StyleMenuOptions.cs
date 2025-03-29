#if UNITY_EDITOR
namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using System.IO;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class StyleMenuOptions
	{
		/// <summary>
		/// Creates the style.
		/// </summary>
		#if UIWIDGETS_LEGACY_STYLE
		[MenuItem("Assets/Create/New UI Widgets/Style", false)]
		#endif
		public static void CreateStyle()
		{
			var path = UtilitiesEditor.SelectedAssetPath() + Path.DirectorySeparatorChar + "New UI Widgets Style.asset";
			var style = UtilitiesEditor.CreateScriptableObjectAsset<Style>(path, false);

			style.SetDefaultValues();
			EditorUtility.SetDirty(style);
			AssetDatabase.SaveAssets();

			Selection.activeObject = style;
		}

		/// <summary>
		/// Apply the default style.
		/// </summary>
		#if UIWIDGETS_LEGACY_STYLE
		[MenuItem("GameObject/UI/New UI Widgets/Apply Default Style", false, 10)]
		#endif
		public static void ApplyDefaultStyle()
		{
			var style = PrefabsMenu.Instance.DefaultStyle;
			if (style == null)
			{
				Debug.LogWarning("Default style not found.");
				return;
			}

			var target = Selection.activeGameObject;
			if (target == null)
			{
				return;
			}

			Undo.RegisterFullObjectHierarchyUndo(target, "Apply Style");
			style.ApplyTo(target);
			RecordPrefabInstanceModifications(target);
		}

		/// <summary>
		/// Record prefab instance modifications.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RecordPrefabInstanceModifications(GameObject target)
		{
			#if UNITY_2018_3_OR_NEWER
			if (PrefabUtility.IsPartOfAnyPrefab(target))
			#endif
			{
				PrefabUtility.RecordPrefabInstancePropertyModifications(target);

				using var _ = ListPool<Component>.Get(out var temp);

				target.GetComponents(temp);

				foreach (var c in temp)
				{
					PrefabUtility.RecordPrefabInstancePropertyModifications(c);
				}
			}

			var t = target.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				RecordPrefabInstanceModifications(t.GetChild(i).gameObject);
			}
		}

		/// <summary>
		/// Check is selected object is not null.
		/// </summary>
		/// <returns><c>true</c>, if selected object is not null, <c>false</c> otherwise.</returns>
		#if UIWIDGETS_LEGACY_STYLE
		[MenuItem("GameObject/UI/New UI Widgets/Apply Default Style", true, 10)]
		#endif
		public static bool CanApplyStyle()
		{
			return Selection.activeGameObject != null;
		}

		/// <summary>
		/// Update the default style.
		/// </summary>
		#if UIWIDGETS_LEGACY_STYLE
		[MenuItem("GameObject/UI/New UI Widgets/Update Default Style", false, 10)]
		#endif
		public static void UpdateDefaultStyle()
		{
			var style = PrefabsMenu.Instance.DefaultStyle;
			if (style == null)
			{
				Debug.LogWarning("Default style not found.");
				return;
			}

			var target = Selection.activeGameObject;
			if (target == null)
			{
				return;
			}

			Undo.RecordObject(style, "Update Style");
			style.GetFrom(target);
			EditorUtility.SetDirty(style);
		}

		/// <summary>
		/// Check is selected object is not null.
		/// </summary>
		/// <returns><c>true</c>, if selected object is not null, <c>false</c> otherwise.</returns>
		#if UIWIDGETS_LEGACY_STYLE
		[MenuItem("GameObject/UI/New UI Widgets/Update Default Style", true, 10)]
		#endif
		public static bool CanUpdateStyle()
		{
			return Selection.activeGameObject != null;
		}
	}
}
#endif