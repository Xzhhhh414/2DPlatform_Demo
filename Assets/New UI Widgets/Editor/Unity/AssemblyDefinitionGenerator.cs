#if UNITY_EDITOR
namespace UIWidgets
{
	using System.Collections.Generic;
	using System.IO;
	using UIWidgets.Pool;
	using UnityEditor;

	/// <summary>
	/// Assembly definitions generator.
	/// </summary>
	public class AssemblyDefinitionGenerator
	{
		readonly struct AssemblyDefinition
		{
			public readonly string Name;

			public readonly string References;

			public readonly bool Editor;

			public AssemblyDefinition(string name, bool editor, IReadOnlyList<string> references)
			{
				Name = name;
				Editor = editor;
				References = references.Count > 0
					? '"' + string.Join("\", \"", references) + '"'
					: string.Empty;
			}

			public readonly void Save(string folder)
			{
				var path = folder + Path.DirectorySeparatorChar + Name + ".asmdef";
				if (File.Exists(path))
				{
					return;
				}

				File.WriteAllText(path, Content());
			}

			readonly string Content()
			{
				var platforms = Editor
					? "\"includePlatforms\": [\"Editor\"], \"excludePlatforms\": [],"
					: string.Empty;

				return string.Format(ReferenceGUID.AsmdefTemplate, Name, References, platforms);
			}
		}

		/// <summary>
		/// Assembly definitions created.
		/// </summary>
		public static bool IsCreated => !typeof(AssemblyDefinitionGenerator).Module.Name.Contains("Assembly-CSharp");

		/// <summary>
		/// Create assembly definitions.
		/// </summary>
		public static void Create()
		{
			if (IsCreated)
			{
				return;
			}

			if (string.IsNullOrEmpty(ReferenceGUID.ScriptsFolder)
				|| string.IsNullOrEmpty(ReferenceGUID.EditorFolder)
				|| string.IsNullOrEmpty(ReferenceGUID.AsmdefTemplateFile))
			{
				UnityEngine.Debug.LogError("[New UI Widgets] Cannot create assemblies: missing scripts folder or editor folder or assembly definition template.");
				return;
			}

			UIThemes.Editor.AssemblyDefinitionGenerator.Create();

			using var _ = ListPool<string>.Get(out var assemblies);
			ProjectSettings.GetAssemblies(assemblies);
			var main = new AssemblyDefinition("UIWidgets", false, assemblies);

			assemblies.Add("UIWidgets");
			var samples = new AssemblyDefinition("UIWidgets.Samples", false, assemblies);

			assemblies.Add("UIThemes.Editor");
			var editor = new AssemblyDefinition("UIWidgets.Editor", true, assemblies);

			main.Save(ReferenceGUID.ScriptsFolder);
			editor.Save(ReferenceGUID.EditorFolder);

			if (!string.IsNullOrEmpty(ReferenceGUID.SamplesFolder))
			{
				samples.Save(ReferenceGUID.SamplesFolder);
			}
		}

		/// <summary>
		/// Delete assembly definitions.
		/// </summary>
		public static void Delete()
		{
			Delete(ReferenceGUID.ScriptsFolder);
			Delete(ReferenceGUID.SamplesFolder);
			Delete(ReferenceGUID.EditorFolder);

			UIThemes.Editor.AssemblyDefinitionGenerator.Delete();

			AssetDatabase.Refresh();
		}

		static void Delete(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			var files = Directory.GetFiles(path, "*.asmdef", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				AssetDatabase.DeleteAsset(file);
			}
		}
	}
}
#endif