#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// Reflection wrappers window.
	/// </summary>
	public partial class ReflectionWrappersWindow : EditorWindow
	{
		/// <summary>
		/// Root block.
		/// </summary>
		protected VisualElement RootBlock;

		/// <summary>
		/// Genetator block.
		/// </summary>
		protected VisualElement GeneratorBlock;

		/// <summary>
		/// Input for the folder to write wrappers files.
		/// </summary>
		protected TextField WrappersFolder;

		/// <summary>
		/// Input for the wrappers namespace.
		/// </summary>
		protected TextField WrappersNamespace;

		/// <summary>
		/// Generate wrappers button.
		/// </summary>
		protected Button WrappersGenerate;

		/// <summary>
		/// Show reflection wrappers.
		/// </summary>
		[MenuItem("Window/UI Themes/Reflection Wrappers")]
		public static void Open()
		{
			var window = GetWindow<ReflectionWrappersWindow>();
			window.minSize = new Vector2(500, 600);
			window.titleContent = new GUIContent("Reflection Wrappers");
		}

		/// <summary>
		/// Create GUI.
		/// </summary>
		public virtual void CreateGUI()
		{
			if (RootBlock != null)
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

			RootBlock = new VisualElement();
			rootVisualElement.Add(RootBlock);

			Refresh();
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			#if !UNITY_2020_3_OR_NEWER
			CreateGUI();
			#endif

			ReflectionWrappersRegistry.OnChanged += Refresh;
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			ReflectionWrappersRegistry.OnChanged -= Refresh;
		}

		/// <summary>
		/// Refresh.
		/// </summary>
		protected virtual void Refresh()
		{
			if (RootBlock == null)
			{
				CreateGUI();
				return;
			}

			RootBlock.Clear();

			var wrappers = ReflectionWrappersRegistry.All();

			ShowWrappers(wrappers);

			GeneratorBlock ??= CreateGeneratorBlock();

			if (wrappers.Count > 0)
			{
				RootBlock.Add(GeneratorBlock);
			}
		}

		/// <summary>
		/// Generate wrappers scripts.
		/// </summary>
		protected virtual void Generate()
		{
			ThemesReferences.Instance.WrappersFolder = WrappersFolder.value;
			ThemesReferences.Instance.WrappersNamespace = WrappersNamespace.value;

			var gen = new WrappersGenerator(
				ThemesReferences.Instance.WrappersFolder,
				ThemesReferences.Instance.WrappersNamespace,
				ReflectionWrappersRegistry.All());
			gen.Run();
		}

		/// <summary>
		/// Select folder.
		/// </summary>
		protected virtual void SelectFolder()
		{
			var result = Utilities.SelectAssetsPath(WrappersFolder.value, "Select a directory for wrappers scripts");
			if (string.IsNullOrEmpty(result))
			{
				return;
			}

			WrappersFolder.value = result;
			ValidateFolder(result);
		}

		/// <summary>
		/// Show wrappers.
		/// </summary>
		/// <param name="wrappers">Wrappers.</param>
		protected virtual void ShowWrappers(IReadOnlyDictionary<Type, IReadOnlyCollection<string>> wrappers)
		{
			var scroll = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
			scroll.AddToClassList("wrappers-scroll");

			if (wrappers.Count == 0)
			{
				var none = new Label("There are no wrappers created with reflection.");
				none.AddToClassList("wrappers-none");
				scroll.Add(none);
			}
			else
			{
				var info = new Label("Types and fields / properties with wrappers created with reflection:");
				info.AddToClassList("wrappers-info");
				scroll.Add(info);
			}

			foreach (var values in wrappers)
			{
				var block = ShowTypeProperties(values.Key, values.Value);
				scroll.Add(block);
			}

			RootBlock.Add(scroll);
		}

		/// <summary>
		/// Validate folder.
		/// </summary>
		/// <param name="folder">Folder.</param>
		protected virtual void ValidateFolder(string folder)
		{
			ValidateInput(folder, WrappersNamespace.value);
		}

		/// <summary>
		/// Validate wrappers namespace.
		/// </summary>
		/// <param name="wrappersNamespace">Namespace.</param>
		protected virtual void ValidateNamespace(string wrappersNamespace)
		{
			ValidateInput(WrappersFolder.value, wrappersNamespace);
		}

		/// <summary>
		/// Validate input.
		/// </summary>
		/// <param name="folder">Folder.</param>
		/// <param name="wrappersNamespace">Wrappers namespace.</param>
		protected virtual void ValidateInput(string folder, string wrappersNamespace)
		{
			var valid_folder = System.IO.Directory.Exists(folder);
			WrappersFolder.EnableInClassList("wrappers-generate-input-error", !valid_folder);

			var valid_namespace = !string.IsNullOrEmpty(wrappersNamespace);
			WrappersNamespace.EnableInClassList("wrappers-generate-input-error", !valid_namespace);

			WrappersGenerate.SetEnabled(valid_folder && valid_namespace);
		}

		/// <summary>
		/// Show generator block.
		/// </summary>
		/// <returns>Block.</returns>
		protected virtual VisualElement CreateGeneratorBlock()
		{
			var block = new VisualElement();
			block.AddToClassList("wrappers-generate");

			var title = new Label("Wrappers Generator");
			title.AddToClassList("wrappers-generate-title");
			block.Add(title);

			var folder_block = new VisualElement();
			folder_block.AddToClassList("wrappers-generate-folder");
			WrappersFolder = new TextField("Wrappers Folder")
			{
				value = ThemesReferences.Instance.WrappersFolder,
			};
			WrappersFolder.RegisterValueChangedCallback(x => ValidateFolder(x.newValue));
			WrappersFolder.AddToClassList("wrappers-generate-input");
			folder_block.Add(WrappersFolder);

			var select = new Button(SelectFolder)
			{
				text = "Select a path",
			};
			folder_block.Add(select);

			block.Add(folder_block);

			WrappersNamespace = new TextField("Wrappers Namespace")
			{
				value = ThemesReferences.Instance.WrappersNamespace,
			};
			WrappersFolder.AddToClassList("wrappers-generate-input");
			WrappersNamespace.RegisterValueChangedCallback(x => ValidateNamespace(x.newValue));
			block.Add(WrappersNamespace);

			WrappersGenerate = new Button(Generate)
			{
				text = "Generate Wrappers",
			};
			block.Add(WrappersGenerate);

			ValidateInput(WrappersFolder.value, WrappersNamespace.value);

			return block;
		}

		/// <summary>
		/// Show type properties.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="properties">Properties.</param>
		/// <returns>Visual element.</returns>
		protected virtual VisualElement ShowTypeProperties(Type type, IReadOnlyCollection<string> properties)
		{
			var block = new VisualElement();
			block.AddToClassList("wrappers-block");

			var type_name = new Label(UtilitiesEditor.GetFriendlyTypeName(type));
			type_name.AddToClassList("wrappers-block-type");
			block.Add(type_name);
			foreach (var property in properties)
			{
				var label = new Label("- " + property);
				label.AddToClassList("wrappers-block-type-property");
				block.Add(label);
			}

			return block;
		}
	}
}
#endif