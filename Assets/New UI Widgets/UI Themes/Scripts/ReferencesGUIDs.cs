#if UNITY_EDITOR
namespace UIThemes
{
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// GUIDs references.
	/// </summary>
	public static class ReferencesGUIDs
	{
		const string ScriptsFolderGUID = "98f455afa744e774cad851ea8b50d793";

		/// <summary>
		/// Scripts folder.
		/// </summary>
		public static string ScriptsFolder => AssetDatabase.GUIDToAssetPath(ScriptsFolderGUID);

		const string EditorFolderGUID = "5a1c54369a068324cafa0a303422cc2d";

		/// <summary>
		/// Editor folder.
		/// </summary>
		public static string EditorFolder => AssetDatabase.GUIDToAssetPath(EditorFolderGUID);

		const string AsmdefTemplateGUID = "0ffd01842aa56fd4488a4408d86519ce";

		/// <summary>
		/// Assembly definition template file.
		/// </summary>
		public static string AsmdefTemplateFile => AssetDatabase.GUIDToAssetPath(AsmdefTemplateGUID);

		/// <summary>
		/// Assembly definition template.
		/// </summary>
		public static string AsmdefTemplate
		{
			get
			{
				var file = AsmdefTemplateFile;
				return string.IsNullOrEmpty(file) ? string.Empty : File.ReadAllText(file);
			}
		}

		const string SamplesFolderGUID = "6a8988fab0895a24f8e0dbba9d42f03b";

		/// <summary>
		/// Samples folder.
		/// </summary>
		public static string SamplesFolder => AssetDatabase.GUIDToAssetPath(SamplesFolderGUID);

		const string AssetsFolderGUID = "8a768ab325795d9418048129e781f26c";

		/// <summary>
		/// Assets folder.
		/// </summary>
		public static string AssetsFolder => AssetDatabase.GUIDToAssetPath(AssetsFolderGUID);

		const string DefaultThemeGUID =
#if UIWIDGETS_INSTALLED
			"117859af4570fec48b967908ecfe2c75";
#else
			"";
#endif

		/// <summary>
		/// Path to the default Theme.
		/// </summary>
		public static string DefaultThemePath => AssetDatabase.GUIDToAssetPath(DefaultThemeGUID);

		/// <summary>
		/// Default Theme.
		/// </summary>
		public static Theme DefaultTheme
		{
			get
			{
				var path = AssetDatabase.GUIDToAssetPath(DefaultThemeGUID);
				if (string.IsNullOrEmpty(path))
				{
					return null;
				}

				return AssetDatabase.LoadAssetAtPath<Theme>(path);
			}
		}

		const string WrapperTemplateGuid = "64e278826d934f04aa264638826693c5";

		/// <summary>
		/// Wrapper script template.
		/// </summary>
		public static string WrapperTemplate
		{
			get
			{
				var path = AssetDatabase.GUIDToAssetPath(WrapperTemplateGuid);
				if (string.IsNullOrEmpty(path))
				{
					return null;
				}

				return AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
			}
		}

		const string RegistryTemplateGuid = "cc4be8de7089c794ba3c3c9f0c21c29d";

		/// <summary>
		/// Registry script template.
		/// </summary>
		public static string RegistryTemplate
		{
			get
			{
				var path = AssetDatabase.GUIDToAssetPath(RegistryTemplateGuid);
				if (string.IsNullOrEmpty(path))
				{
					return null;
				}

				return AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
			}
		}

		const string EditorUSSGUID = "880339e54e64f8c4a944f703da1869d6";

		static readonly List<StyleSheet> Styles = new List<StyleSheet>();

		static StyleSheet defaultStyle;

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(defaultStyle), nameof(Styles))]
		static void StaticInit()
		{
			defaultStyle = null;
			Styles.Clear();
		}
#endif

		/// <summary>
		/// Add stylesheet.
		/// </summary>
		/// <param name="styleSheet">Stylesheet.</param>
		public static void AddStyleSheet(StyleSheet styleSheet)
		{
			if (Styles.Contains(styleSheet))
			{
				return;
			}

			Styles.Add(styleSheet);
		}

		/// <summary>
		/// Get stylesheets.
		/// </summary>
		/// <returns>Stylesheets.</returns>
		public static IReadOnlyCollection<StyleSheet> GetStyleSheets()
		{
			for (var i = Styles.Count - 1; i >= 0; i--)
			{
				if (Styles[i] == null)
				{
					Styles.RemoveAt(i);
				}
			}

			var style = GetDefaultStyleSheet();
			if (style != null)
			{
				Styles.Add(style);
			}

			return Styles;
		}

		static StyleSheet GetDefaultStyleSheet()
		{
			if (defaultStyle != null)
			{
				return defaultStyle;
			}

			var path = AssetDatabase.GUIDToAssetPath(EditorUSSGUID);
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			defaultStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
			return defaultStyle;
		}
	}
}
#endif