#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Serialization;
#if UIWIDGETS_TMPRO_SUPPORT && UIWIDGETS_TMPRO_4_0_OR_NEWER
	using FontAsset = UnityEngine.TextCore.Text.FontAsset;
#elif UIWIDGETS_TMPRO_SUPPORT
	using FontAsset = TMPro.TMP_FontAsset;
#else
	using FontAsset = UnityEngine.ScriptableObject;
#endif

	/// <summary>
	/// Widgets references.
	/// </summary>
	[Serializable]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/project-settings.html")]
	public class WidgetsReferences : ScriptableObject
	{
		[SerializeField]
		bool assemblyDefinitions = true;

		/// <summary>
		/// Assembly definitions.
		/// </summary>
		public bool AssemblyDefinitions
		{
#if UIWIDGETS_ASMDEF_DISABLED
			get => false;
#else
			get => assemblyDefinitions;
#endif

			set
			{
				if (assemblyDefinitions != value)
				{
					assemblyDefinitions = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("instantiatePrefabs")]
		bool instantiateWidgets;

		/// <summary>
		/// Instantiate widgets.
		/// </summary>
		public bool InstantiateWidgets
		{
			get => instantiateWidgets;

			set
			{
				if (instantiateWidgets != value)
				{
					instantiateWidgets = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		bool attachTheme = false;

		/// <summary>
		/// Attach default to the widgets created from the menu.
		/// </summary>
		public bool AttachTheme
		{
			get => attachTheme;

			set
			{
				if (attachTheme != value)
				{
					attachTheme = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		bool useWhiteSprite = false;

		/// <summary>
		/// Sets white sprite for the Image components without sprite.
		/// Prevents rare bugs when such Images are displayed as black.
		/// </summary>
		public bool UseWhiteSprite
		{
			get => useWhiteSprite;

			set
			{
				if (useWhiteSprite != value)
				{
					useWhiteSprite = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		PrefabsMenu current;

		/// <summary>
		/// Current theme.
		/// </summary>
		public PrefabsMenu Current
		{
			get => current;

			set
			{
				if (!ReferenceEquals(current, value))
				{
					current = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		FontAsset defaultFont;

		/// <summary>
		/// Default font.
		/// </summary>
		public FontAsset DefaultFont
		{
			get => defaultFont;

			set
			{
				if (!ReferenceEquals(defaultFont, value))
				{
					defaultFont = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		/// <summary>
		/// Is widgets references exists?
		/// </summary>
		public static bool InstanceExists
		{
			get
			{
				var refs = Resources.FindObjectsOfTypeAll<WidgetsReferences>();
				if (refs.Length > 0)
				{
					return true;
				}

				var guids = AssetDatabase.FindAssets("t:" + typeof(WidgetsReferences).FullName);
				if (guids.Length > 0)
				{
					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Widgets references.
		/// </summary>
		public static WidgetsReferences Instance
		{
			get
			{
				var guids = AssetDatabase.FindAssets("t:" + typeof(WidgetsReferences).FullName);
				if (guids.Length > 0)
				{
					var path = AssetDatabase.GUIDToAssetPath(guids[0]);
					return AssetDatabase.LoadAssetAtPath<WidgetsReferences>(path);
				}

				if (File.Exists(DefaultPath))
				{
					return AssetDatabase.LoadAssetAtPath<WidgetsReferences>(DefaultPath);
				}

				return Create();
			}
		}

		static string DefaultPath => Path.Combine(ReferenceGUID.EditorFolder, "Widgets References.asset");

		/// <summary>
		/// Create references.
		/// </summary>
		/// <returns>Created instance.</returns>
		static WidgetsReferences Create()
		{
			var folder = ReferenceGUID.EditorFolder;
			if (string.IsNullOrEmpty(folder))
			{
				return null;
			}

			return UtilitiesEditor.CreateScriptableObjectAsset<WidgetsReferences>(DefaultPath);
		}
	}
}
#endif