#if UNITY_EDITOR
namespace UIWidgets
{
	using System.IO;
	using UIWidgets.Attributes;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// GUID list.
	/// </summary>
	public static class ReferenceGUID
	{
		const string TMProStatusGUID = "8b1fbe80f7518be41872eadbd10d7067";

		/// <summary>
		/// File with TMPro support recompile status.
		/// </summary>
		public static string TMProStatus => AssetDatabase.GUIDToAssetPath(TMProStatusGUID);

		/// <summary>
		/// Directories and files to recompile when TMPro support enabled.
		/// </summary>
		[DomainReloadExclude]
		public static readonly string[] TMProSupport = new string[]
		{
			"91c23f731032c0649b92539754ca1c35", // Scripts\Converter\*
			"cd38b3043ce53d54b8f534f4de095a75", // Scripts\ThirdPartySupport\TMProSupport\*

			"92953f60ab285fd429ee7c0a7944c4b7", // Scripts\InputField\InputFieldAdapter.cs
			"98d84927b0690ab40afffcb089ec81a8", // Scripts\InputField\InputFieldExtendedAdapter.cs
			"84efaef737bf5184ba72224e49593615", // Scripts\InputField\InputFieldTMProProxy.cs
			"fef7da31ec25ead469faea1d77addefa", // Scripts\Spinner\SpinnerBase.cs
			"e6068bbda50527f419fe2c3a5af4d62b", // Scripts\Style\Style.cs
			"2483f16ccad27d64f8474c0f973c6a3c", // Scripts\Style\Fast\StyleFast.cs
			"6ce2adbf95e74fb46b112b2a9f122158", // Scripts\Style\Unity\StyleInputField.cs
			"1fa840be3ac537041af432e36e1da4b2", // Scripts\Style\Unity\StyleText.cs
			"52e6b3c9e0434ff43bb4f3722a0b0a57", // Scripts\Text\TextAdapter.cs
			"1752ed17982fef34c979ad689810dd7a", // Scripts\Text\TextTMPro.cs
			"0627250e80fa5bc4180d15dab4780e95", // Scripts\WidgetGeneration\Editor\ClassInfo.cs
			"91cef37d757e4b347aa34861c46f395b", // Scripts\WidgetGeneration\Editor\PrefabGenerator.cs
		};

		const string DataBindFolderGUID = "8c23c22a14c225149bd9bf7d5b69ae29";

		/// <summary>
		/// Foldes with DataBind support files.
		/// </summary>
		public static string DataBindFolder => AssetDatabase.GUIDToAssetPath(DataBindFolderGUID);

		const string DataBindStatusGUID = "3bcbe5da50ac7f24db9b4db908dcc110";

		/// <summary>
		/// File with DataBind support recompile status.
		/// </summary>
		public static string DataBindStatus => AssetDatabase.GUIDToAssetPath(DataBindStatusGUID);

		/// <summary>
		/// Directories and files to recompile when DataBind support enabled.
		/// </summary>
		[DomainReloadExclude]
		public static readonly string[] DataBindSupport = new string[]
		{
			"8c23c22a14c225149bd9bf7d5b69ae29", // Scripts\ThirdPartySupport\DataBindSupport\*
			"18f783f02c29bd3448684141f9d2ff3d", // Scripts\WidgetGeneration\DataBindSupport\*
			"91cef37d757e4b347aa34861c46f395b", // Scripts\WidgetGeneration\Editor\PrefabGenerator.cs
		};

		const string I2LocalizationStatusGUID = "8be3e66b21a4bf8489e41fda2bda8781";

		/// <summary>
		/// File with I2 Localization support recompile status.
		/// </summary>
		public static string I2LocalizationStatus => AssetDatabase.GUIDToAssetPath(I2LocalizationStatusGUID);

		/// <summary>
		/// Directories and files to recompile when I2 Localization support enabled.
		/// </summary>
		[DomainReloadExclude]
		public static readonly string[] I2LocalizationSupport = new string[]
		{
			"8d1e8758d46c0db419781b23554c9e48", // Scripts\Localization\Localization.cs
		};

		const string ScriptsFolderGUID = "1a0ae82418872774d8240b5bc4df7d06";

		/// <summary>
		/// Scripts folder.
		/// </summary>
		public static string ScriptsFolder => AssetDatabase.GUIDToAssetPath(ScriptsFolderGUID);

		const string EditorFolderGUID = "19ae3c8c02ecf5a49b87f66633cef292";

		/// <summary>
		/// Editor folder.
		/// </summary>
		public static string EditorFolder => AssetDatabase.GUIDToAssetPath(EditorFolderGUID);

		const string SamplesFolderGUID = "a1a3297bdcc2c4c4f87b613b2e928a9a";

		/// <summary>
		/// Samples folder.
		/// </summary>
		public static string SamplesFolder => AssetDatabase.GUIDToAssetPath(SamplesFolderGUID);

		const string AsmdefTemplateGUID = "1535ee6bbede3cf40a020c518c739227";

		/// <summary>
		/// Path to the assembly definition template.
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

		const string WhiteSpriteGUID = "b877c001bce9a1844aaee2cec5610ed6";

		/// <summary>
		/// White sprite.
		/// </summary>
		public static Sprite WhiteSprite
		{
			get
			{
				var file = AssetDatabase.GUIDToAssetPath(WhiteSpriteGUID);
				return string.IsNullOrEmpty(file) ? null : AssetDatabase.LoadAssetAtPath<Sprite>(file);
			}
		}

		/// <summary>
		/// Asset with references to the prefabs used by menu.
		/// </summary>
		public const string PrefabsMenu
#if UIWIDGETS_LEGACY_STYLE
			= "6a9100cba93b8194b974a6bf0e54197a";
#else
			= "ab14f43b49b4da74bb78683ba2149a67";
#endif

		/// <summary>
		/// Asset with references to the prefabs templates used by widget generation.
		/// </summary>
		public const string PrefabsTemplates
#if UIWIDGETS_LEGACY_STYLE
			= "6e48cc01edc8c384c8986c32d6343834";
#else
			= "9328bc1cc1f0fee489f98e97649ca3a6";
#endif

		/// <summary>
		/// Asset with references to the scripts templates used by widget generation.
		/// </summary>
		public const string ScriptsTemplates = "87c41e4b0b1f0b24aacf776a72df2626";

		/// <summary>
		/// Asset with references to the DataBind scripts templates used by DataBind support.
		/// </summary>
		public const string DataBindTemplates = "48d6be4465afc704fad81633397bf474";
	}
}
#endif