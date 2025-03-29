#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System.Collections.Generic;
	using System.IO;
	using UIThemes.Pool;
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

				return string.Format(ReferencesGUIDs.AsmdefTemplate, Name, References, platforms);
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

			if (string.IsNullOrEmpty(ReferencesGUIDs.ScriptsFolder)
				|| string.IsNullOrEmpty(ReferencesGUIDs.EditorFolder)
				|| string.IsNullOrEmpty(ReferencesGUIDs.AsmdefTemplateFile))
			{
				UnityEngine.Debug.LogError("[UI Themes] Cannot create assemblies: missing scripts folder or editor folder or assembly definition template.");
				return;
			}

			using var _ = ListPool<string>.Get(out var assemblies);
			ProjectSettings.GetAssemblies(assemblies);
			assemblies.Add("Unity.Addressables");
			assemblies.Add("Unity.ResourceManager");
			var main = new AssemblyDefinition("UIThemes", false, assemblies);

			assemblies.Clear();
			ProjectSettings.GetAssemblies(assemblies);
			assemblies.Add("UIThemes");
			assemblies.Add("Unity.Addressables.Editor");
			var editor = new AssemblyDefinition("UIThemes.Editor", true, assemblies);

			main.Save(ReferencesGUIDs.ScriptsFolder);
			editor.Save(ReferencesGUIDs.EditorFolder);

			if (!string.IsNullOrEmpty(ReferencesGUIDs.SamplesFolder))
			{
				var samples = new AssemblyDefinition("UIThemes.Samples", false, assemblies);
				samples.Save(ReferencesGUIDs.SamplesFolder);
			}
		}

		/// <summary>
		/// Delete assembly definitions.
		/// </summary>
		public static void Delete()
		{
			Delete(ReferencesGUIDs.ScriptsFolder);
			Delete(ReferencesGUIDs.SamplesFolder);
			Delete(ReferencesGUIDs.EditorFolder);

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