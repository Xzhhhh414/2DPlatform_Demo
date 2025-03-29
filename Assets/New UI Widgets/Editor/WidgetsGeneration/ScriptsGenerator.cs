#if UNITY_EDITOR
namespace UIWidgets.WidgetGeneration
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UIWidgets.Extensions;
	using UIWidgets.Pool;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Widget scripts generator.
	/// </summary>
	public class ScriptsGenerator : IFormattable
	{
		/// <summary>
		/// Script data.
		/// </summary>
		public class ScriptData
		{
			/// <summary>
			/// Type.
			/// </summary>
			public readonly string Type;

			/// <summary>
			/// Class name.
			/// </summary>
			public readonly string ClassName;

			/// <summary>
			/// Template.
			/// </summary>
			public readonly TextAsset Template;

			/// <summary>
			/// Can create.
			/// </summary>
			public readonly bool CanCreate;

			/// <summary>
			/// Path.
			/// </summary>
			public readonly string Path;

			/// <summary>
			/// Initializes a new instance of the <see cref="ScriptData"/> class.
			/// </summary>
			/// <param name="type">Type.</param>
			/// <param name="classname">Class name.</param>
			/// <param name="template">Template.</param>
			/// <param name="path">Path.</param>
			/// <param name="canCreate">Can script be created?</param>
			public ScriptData(string type, string classname, TextAsset template, string path, bool canCreate)
			{
				Type = type;
				ClassName = classname;
				Template = template;
				Path = path;
				CanCreate = canCreate;
			}
		}

		/// <summary>
		/// Class info.
		/// </summary>
		public ClassInfo Info;

		/// <summary>
		/// Path to save created files.
		/// </summary>
		protected string SavePath;

		/// <summary>
		/// Path to save created editor scripts.
		/// </summary>
		protected string EditorSavePath;

		/// <summary>
		/// Path to save created widgets scripts.
		/// </summary>
		protected string ScriptSavePath;

		/// <summary>
		/// Path to save created prefab.
		/// </summary>
		protected string PrefabSavePath;

		/// <summary>
		/// Scripts templates.
		/// </summary>
		public readonly Dictionary<string, ScriptData> Templates = new Dictionary<string, ScriptData>();

		/// <summary>
		/// Script templates values.
		/// </summary>
		protected Dictionary<string, string> TemplateValues;

		/// <summary>
		/// Script editor templates.
		/// </summary>
		protected HashSet<string> EditorTemplates = new HashSet<string>()
		{
			"MenuOptions",
			"PrefabGenerator",
			"PrefabGeneratorAutocomplete",
			"PrefabGeneratorTable",
			"PrefabGeneratorScene",
		};

		/// <summary>
		/// Prefabs.
		/// </summary>
		protected List<string> Prefabs = new List<string>()
		{
			"ListView",
			"DragInfo",
			"Combobox",
			"ComboboxMultiselect",
			"Table",
			"TileView",
			"TreeView",
			"TreeGraph",
			"PickerListView",
			"PickerTreeView",
			"Autocomplete",
			"AutoCombobox",
			"Tooltip",
		};

		/// <summary>
		/// Allow autocomplete script and widget.
		/// </summary>
		protected bool AllowAutocomplete;

		/// <summary>
		/// Allow table widget.
		/// </summary>
		protected bool AllowTable;

		/// <summary>
		/// Allow test item generation.
		/// </summary>
		protected bool AllowItem;

		/// <summary>
		/// String builder.
		/// </summary>
		protected StringBuilder StrBuilder = new StringBuilder();

		/// <summary>
		/// Add script data.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="template">Template.</param>
		/// <param name="canCreate">Can script be created?</param>
		protected void AddScriptData(string type, TextAsset template, bool canCreate = true)
		{
			var classname = type + Info.ShortTypeName;
			var dir = EditorTemplates.Contains(type) ? EditorSavePath : ScriptSavePath;
			var path = dir + Path.DirectorySeparatorChar + classname + ".cs";

			var data = new ScriptData(type, classname, template, path, canCreate);
			Templates[type] = data;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptsGenerator"/> class.
		/// </summary>
		/// <param name="info">Class info.</param>
		/// <param name="path">Path to save created files.</param>
		public ScriptsGenerator(ClassInfo info, string path)
		{
			Info = info;
			SavePath = path + Path.DirectorySeparatorChar + "Widgets" + info.ShortTypeName;
			EditorSavePath = SavePath + Path.DirectorySeparatorChar + "Editor";
			ScriptSavePath = SavePath + Path.DirectorySeparatorChar + "Scripts";
			PrefabSavePath = SavePath + Path.DirectorySeparatorChar + "Prefabs";

			var templates = ScriptsTemplates.Instance;

			AllowAutocomplete = !string.IsNullOrEmpty(Info.AutocompleteField);
			AllowTable = Info.TextFields.Count > 0;
			AllowItem = Info.ParameterlessConstructor;

			// collections
			AddScriptData("Autocomplete", templates.Autocomplete, AllowAutocomplete);
			AddScriptData("AutoCombobox", templates.AutoCombobox, AllowAutocomplete);
			AddScriptData("Combobox", templates.Combobox);
			AddScriptData("ListView", templates.ListView);
			AddScriptData("ListViewComponent", templates.ListViewComponent);
			AddScriptData("TreeGraph", templates.TreeGraph);
			AddScriptData("TreeGraphComponent", templates.TreeGraphComponent);
			AddScriptData("TreeView", templates.TreeView);
			AddScriptData("TreeViewComponent", templates.TreeViewComponent);

			// collections support
			AddScriptData("Comparers", templates.Comparers);
			AddScriptData("ListViewDragSupport", templates.ListViewDragSupport);
			AddScriptData("ListViewDropSupport", templates.ListViewDropSupport);
			AddScriptData("TreeViewDropSupport", templates.TreeViewDropSupport);
			AddScriptData("TreeViewNodeDragSupport", templates.TreeViewNodeDragSupport);
			AddScriptData("TreeViewNodeDropSupport", templates.TreeViewNodeDropSupport);
			AddScriptData("Tooltip", templates.Tooltip);
			AddScriptData("TooltipViewer", templates.TooltipViewer);

			// dialogs
			AddScriptData("PickerListView", templates.PickerListView);
			AddScriptData("PickerTreeView", templates.PickerTreeView);

			// test
			AddScriptData("Test", templates.Test);
			AddScriptData("TestItem", AllowItem ? templates.TestItem : templates.TestItemOff);

			// test
			AddScriptData("MenuOptions", templates.MenuOptions);

			// generators
			AddScriptData("PrefabGenerator", templates.PrefabGenerator);
			AddScriptData("PrefabGeneratorAutocomplete", AllowAutocomplete ? templates.PrefabGeneratorAutocomplete : templates.PrefabGeneratorAutocompleteOff);
			AddScriptData("PrefabGeneratorTable", AllowTable ? templates.PrefabGeneratorTable : templates.PrefabGeneratorTableOff);
			AddScriptData("PrefabGeneratorScene", templates.PrefabGeneratorScene);
		}

		void SetAvailableScripts()
		{
			foreach (var template in Templates)
			{
				if (template.Value.CanCreate)
				{
					Info.Scripts[template.Key] = !File.Exists(template.Value.Path);
				}
			}
		}

		void SetAvailablePrefabs()
		{
			foreach (var prefab in Prefabs)
			{
				if (CanCreateWidget(prefab))
				{
					Info.Prefabs[prefab] = !File.Exists(Prefab2Filename(prefab));
				}
			}
		}

		void SetAvailableScenes()
		{
			Info.Scenes["TestScene"] = !File.Exists(Scene2Filename(Info.ShortTypeName));
		}

		void SetTemplateValues()
		{
			var label_listview = Info.ParameterlessConstructor
				? "The left ListView and TileView display the same list.\\r\\nYou can Drag-and-Drop items between ListView, TileView and TreeView."
				: "Test data is not available because of a data type\\r\\ndoes not have a parameterless constructor.";
			TemplateValues = new Dictionary<string, string>()
			{
				{ "WidgetsNamespace", Info.WidgetsNamespace },
				{ "SourceClassShortName", Info.ShortTypeName },
				{ "SourceClass", Info.FullTypeName },
				{ "AutocompleteField", Info.AutocompleteField },
				{ "ComparersEnum", "ComparersFields" + Info.ShortTypeName },
				{ "Info", UtilitiesEditor.Serialize(Info) },
				{ "Path", UtilitiesEditor.Serialize(SavePath) },
				{ "TextType", UtilitiesEditor.GetFriendlyTypeName(Info.TextFieldType) },
				{ "AutocompleteInput", UtilitiesEditor.GetFriendlyTypeName(Info.InputField) },
				{ "AutocompleteText", UtilitiesEditor.GetFriendlyTypeName(Info.InputText) },
				{ "PrefabsMenuGUID", Info.PrefabsMenuGUID },
				{ "LabelListView", label_listview },
			};
		}

		/// <summary>
		/// Generate files.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public void Generate()
		{
			SetAvailableScripts();
			SetAvailablePrefabs();
			SetAvailableScenes();

			OverwriteRequestWindow.Open(this, GenerateScripts);
		}

		void GenerateScripts()
		{
			Directory.CreateDirectory(EditorSavePath);
			Directory.CreateDirectory(ScriptSavePath);
			Directory.CreateDirectory(PrefabSavePath);

			if (Info.Scenes["TestScene"])
			{
				if (!Compatibility.SceneSave())
				{
					EditorUtility.DisplayDialog("Widget Generation", "Please save scene to continue.", "OK");
					return;
				}

				Compatibility.SceneNew();
			}

			var menu = ScriptableObject.CreateInstance<PrefabsMenuGenerated>();

			StrBuilder.Append(PrefabSavePath);
			StrBuilder.Append(Path.DirectorySeparatorChar);
			StrBuilder.Append("PrefabsMenu");
			StrBuilder.Append(Info.ShortTypeName);
			StrBuilder.Append(".asset");
			var menu_path = StrBuilder.ToString();
#if CSHARP_7_3_OR_NEWER
			StrBuilder.Clear();
#else
			StrBuilder = new StringBuilder();
#endif

			AssetDatabase.CreateAsset(menu, menu_path);
			Info.PrefabsMenuGUID = AssetDatabase.AssetPathToGUID(menu_path);

			SetTemplateValues();

			try
			{
				var progress = 0;
				ProgressbarUpdate(progress);

				foreach (var template in Templates)
				{
					CreateScript(template.Value);

					progress++;
					ProgressbarUpdate(progress);
				}
			}
			catch (Exception)
			{
				EditorUtility.ClearProgressBar();
				throw;
			}

			AssetDatabase.Refresh();
		}

		void ProgressbarUpdate(int progress)
		{
			if (progress < Templates.Count)
			{
				EditorUtility.DisplayProgressBar("Widget Generation", "Step 1. Creating scripts.", progress / (float)Templates.Count);
			}
			else
			{
				EditorUtility.ClearProgressBar();
			}
		}

		/// <summary>
		/// Is widget can be created?
		/// </summary>
		/// <param name="name">Widget name.</param>
		/// <returns>True if widget can be created; otherwise false.</returns>
		protected virtual bool CanCreateWidget(string name)
		{
			if (name == "Autocomplete")
			{
				return AllowAutocomplete;
			}

			if (name == "AutoCombobox")
			{
				return AllowAutocomplete;
			}

			if (name == "Table")
			{
				return AllowTable;
			}

			return true;
		}

		/// <summary>
		/// Get prefab filename by widget name.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Filename.</returns>
		public string Prefab2Filename(string type)
		{
			StrBuilder.Append(PrefabSavePath);
			StrBuilder.Append(Path.DirectorySeparatorChar);
			StrBuilder.Append(type);
			StrBuilder.Append(Info.ShortTypeName);
			StrBuilder.Append(".prefab");

			var filename = StrBuilder.ToString();
#if CSHARP_7_3_OR_NEWER
			StrBuilder.Clear();
#else
			StrBuilder = new StringBuilder();
#endif

			return filename;
		}

		/// <summary>
		/// Get prefab filename by widget name.
		/// </summary>
		/// <param name="scene">Scene.</param>
		/// <returns>Filename.</returns>
		public string Scene2Filename(string scene)
		{
			return SavePath + Path.DirectorySeparatorChar + scene + ".unity";
		}

		/// <summary>
		/// Create script by the specified template.
		/// </summary>
		/// <param name="data">Script data.</param>
		protected virtual void CreateScript(ScriptData data)
		{
			if (!Info.Scripts.TryGetValue(data.Type, out var can_create))
			{
				return;
			}

			if (!can_create)
			{
				return;
			}

			var code = string.Format(data.Template.text, this);

			File.WriteAllText(data.Path, code);
		}

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value.</param>
		/// <returns>The value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (TemplateValues.ContainsKey(format))
			{
				return TemplateValues[format];
			}

			var cls = "Class";
			if (format.EndsWith(cls))
			{
				var key = format.Substring(0, format.Length - cls.Length);
				if (Templates.ContainsKey(key))
				{
					return Templates[key].ClassName;
				}
			}

			var pos = format.IndexOf("@");
			if (pos != -1)
			{
				return ToStringList(format, formatProvider);
			}

			throw new ArgumentOutOfRangeException("Unsupported format: " + format);
		}

		readonly List<Type> TypesInt = new List<Type>()
		{
			typeof(decimal),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
		};

		readonly List<Type> TypesFloat = new List<Type>()
		{
			typeof(float),
			typeof(double),
		};

		/// <summary>
		/// Format fields list to list.
		/// </summary>
		/// <param name="predicate">Predicate to check if field should be included.</param>
		/// <param name="format">Format.</param>
		/// <param name="formatProvider">Format provider.</param>
		/// <param name="onlyFirst">Include only first field that match predicate.</param>
		/// <returns>The fields in the specified format.</returns>
		protected string FormatFields(Predicate<ClassField> predicate, string format, IFormatProvider formatProvider, bool onlyFirst = false)
		{
			using var _ = ListPool<ClassField>.Get(out var fields);

			foreach (var field in Info.Fields)
			{
				if (predicate(field))
				{
					fields.Add(field);
					if (onlyFirst)
					{
						break;
					}
				}
			}

			return fields.ToString(format, this, formatProvider);
		}

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value.</param>
		/// <returns>The value of the current instance in the specified format.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed.")]
		protected string ToStringList(string format, IFormatProvider formatProvider)
		{
			var template = format.Split(new[] { "@" }, 2, StringSplitOptions.None);
			template[1] = template[1].Replace("[", "{").Replace("]", "}");

			return template[0] switch
			{
				"IfTMProText" => Info.IsTMProText ? string.Format(template[1], this) : string.Empty,
				"!IfTMProText" => !Info.IsTMProText ? string.Format(template[1], this) : string.Empty,
				"IfTMProInputField" => Info.IsTMProInputField ? string.Format(template[1], this) : string.Empty,
				"!IfTMProInputField" => !Info.IsTMProInputField ? string.Format(template[1], this) : string.Empty,
				"IfAutocomplete" => AllowAutocomplete ? string.Format(template[1], this) : string.Empty,
				"!IfAutocomplete" => !AllowAutocomplete ? string.Format(template[1], this) : string.Empty,
				"IfTable" => AllowTable ? string.Format(template[1], this) : string.Empty,
				"!IfTable" => !AllowTable ? string.Format(template[1], this) : string.Empty,
				"Fields" => Info.Fields.ToString(template[1], this, formatProvider),
				"TextFields" => Info.TextFields.ToString(template[1], this, formatProvider),
				"TextFieldsComparableGeneric" => Info.TextFieldsComparableGeneric.ToString(template[1], this, formatProvider),
				"TextFieldsComparableNonGeneric" => Info.TextFieldsComparableNonGeneric.ToString(template[1], this, formatProvider),
				"TextFieldFirst" => Info.TextFieldFirst.ToString(template[1], this, formatProvider),
				"TableFieldFirst" => Info.TableFieldFirst.ToString(template[1], this, formatProvider),
				"ImageFields" => FormatFields(x => x.IsImage, template[1], formatProvider),
				"ImageFieldsNullable" => FormatFields(x => x.IsImage && x.IsNullable, template[1], formatProvider),
				"TreeViewFields" => FormatFields(x => x.WidgetFieldName != "TextAdapter" && x.WidgetFieldName != "Icon", template[1], formatProvider),
				"FieldsString" => FormatFields(x => x.FieldType == typeof(string), template[1], formatProvider),
				"FieldsStringFirst" => FormatFields(x => x.FieldType == typeof(string), template[1], formatProvider, true),
				"FieldsInt" => FormatFields(x => TypesInt.Contains(x.FieldType), template[1], formatProvider),
				"FieldsFloat" => FormatFields(x => TypesFloat.Contains(x.FieldType), template[1], formatProvider),
				"FieldsSprite" => FormatFields(x => x.FieldType == typeof(Sprite), template[1], formatProvider),
				"FieldsTexture2D" => FormatFields(x => x.FieldType == typeof(Texture2D), template[1], formatProvider),
				"FieldsColor" => FormatFields(x => (x.FieldType == typeof(Color)) || (x.FieldType == typeof(Color32)), template[1], formatProvider),
				_ => throw new ArgumentOutOfRangeException("Unsupported format: " + format),
			};
		}
	}
}
#endif