#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEngine;

	/// <summary>
	/// Theme editor window.
	/// </summary>
	public partial class ThemeEditor : EditorWindow
	{
		/// <summary>
		/// Theme.
		/// </summary>
		protected Theme Theme;

		/// <summary>
		/// Select theme view.
		/// </summary>
		protected SelectBlockView SelectView;

		/// <summary>
		/// Edit theme values view.
		/// </summary>
		protected EditBlockView EditView;

		/// <summary>
		/// Warning view.
		/// </summary>
		protected ThemeWarningBlockView WarningBlockView;

		/// <summary>
		/// Open theme editor window.
		/// </summary>
		/// <param name="instanceID">Asset instance ID.</param>
		/// <param name="line">Line.</param>
		/// <returns>true if asset is theme; otherwise false.</returns>
		[OnOpenAsset(100)]
		public static bool OpenAsset(int instanceID, int line)
		{
			var theme = EditorUtility.InstanceIDToObject(instanceID) as Theme;
			if (theme == null)
			{
				return false;
			}

			Open(theme);
			return true;
		}

		/// <summary>
		/// Open theme editor window.
		/// </summary>
		[MenuItem("Window/UI Themes/Editor")]
		public static void Open()
		{
			Open(null);
		}

		/// <summary>
		/// Open theme editor window.
		/// </summary>
		/// <param name="theme">Theme.</param>
		public static void Open(Theme theme)
		{
			var window = GetWindow<ThemeEditor>();
			window.minSize = new Vector2(800, 500);
			window.titleContent = new GUIContent("UITheme");
			window.SetTheme(theme);
		}

		/// <summary>
		/// Refresh window.
		/// </summary>
		public static void RefreshWindow()
		{
			foreach (var window in Resources.FindObjectsOfTypeAll<ThemeEditor>())
			{
				window.Refresh();
			}
		}

		/// <summary>
		/// Create GUI.
		/// </summary>
		public virtual void CreateGUI()
		{
			if (SelectView != null)
			{
				return;
			}

			foreach (var style in ReferencesGUIDs.GetStyleSheets())
			{
				rootVisualElement.styleSheets.Add(style);
			}

			if (EditorGUIUtility.isProSkin)
			{
				rootVisualElement.AddToClassList("theme-dark-skin");
			}

			SelectView = new SelectBlockView();
			rootVisualElement.Add(SelectView.Block);
			SelectView.OnThemeChanged += SetTheme;

			EditView = new EditBlockView();
			rootVisualElement.Add(EditView.Block);

			WarningBlockView = new ThemeWarningBlockView();
			rootVisualElement.Add(WarningBlockView.Block);

			ThemeChanged();
		}

		/// <summary>
		/// Refresh.
		/// </summary>
		public virtual void Refresh()
		{
			EditView?.Refresh();
		}

		/// <summary>
		/// Refresh.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		protected virtual void Refresh(VariationId variationId)
		{
			Refresh();
		}

		/// <summary>
		/// Process theme changed.
		/// </summary>
		protected virtual void ThemeChanged()
		{
			if (SelectView == null)
			{
				return;
			}

			var warning = Theme == ReferencesGUIDs.DefaultTheme;
			SelectView.Visible = !warning && (Theme == null);
			EditView.Visible = !warning && (Theme != null);
			WarningBlockView.Visible = warning;

			EditView.Theme = Theme;
		}

		/// <summary>
		/// Set theme.
		/// </summary>
		/// <param name="theme">Theme.</param>
		protected virtual void SetTheme(Theme theme)
		{
			if (Theme.ValidateVariations(theme))
			{
				EditorUtility.SetDirty(theme);
				AssetDatabase.SaveAssets();
			}

			if (Theme != null)
			{
				Theme.OnChange -= Refresh;
			}

			Theme = theme;

			if (Theme != null)
			{
				Theme.OnChange += Refresh;
			}

			if (Theme != null)
			{
				titleContent = new GUIContent("UI Theme: " + theme.name);
				Theme.ClearCache();
			}
			else
			{
				titleContent = new GUIContent("UI Theme");
			}

			ThemeChanged();
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			#if !UNITY_2020_3_OR_NEWER
			CreateGUI();
			#endif

			if (Theme != null)
			{
				Theme.OnChange -= Refresh;
				Theme.OnChange += Refresh;
			}

			Undo.undoRedoPerformed += HandleUndo;
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			if (Theme != null)
			{
				Theme.OnChange -= Refresh;
			}

			Undo.undoRedoPerformed -= HandleUndo;

			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// Handle undo.
		/// </summary>
		protected virtual void HandleUndo()
		{
			if (Theme != null)
			{
				Theme.ClearCache();
			}

			Refresh();

			UtilitiesEditor.RefreshTargets();
			ThemeTargetInspector.RefreshWindow();
		}
	}
}
#endif