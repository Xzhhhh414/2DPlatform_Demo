#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Assembly definitions editor.
	/// </summary>
	[Obsolete("Unsupported and work incorrectly with different Unity versions.")]
	public static class AssemblyDefinitionsEditor
	{
#if UNITY_2018_1_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Reviewed.")]
		[Serializable]
		class AssemblyDefinition
		{
			/// <summary>
			/// Path.
			/// </summary>
			[NonSerialized]
			public string Path;

			/// <summary>
			/// Assembly name.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Compatibility with Unity class.")]
			public string name = string.Empty;

			/// <summary>
			/// References.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Compatibility with Unity class.")]
			public List<string> references = new List<string>();

			/// <summary>
			/// Optional references.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Compatibility with Unity class.")]
			public List<string> optionalUnityReferences = new List<string>();

			/// <summary>
			/// Include platforms.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Compatibility with Unity class.")]
			public List<string> includePlatforms = new List<string>();

			/// <summary>
			/// Exclude platforms.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Compatibility with Unity class.")]
			public List<string> excludePlatforms = new List<string>();

			/// <summary>
			/// Allow unsafe code.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Compatibility with Unity class.")]
			public bool allowUnsafeCode = false;
		}

		static readonly Dictionary<string, string[]> CachedGUIDs = new Dictionary<string, string[]>();

		#if UNITY_2019_3_OR_NEWER
		/// <summary>
		/// Reload support.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(CachedGUIDs))]
		static void StaticInit()
		{
			CachedGUIDs.Clear();
		}
		#endif

		static void Get(string search, List<AssemblyDefinition> output)
		{
			var guids = CachedGUIDs.ContainsKey(search)
				? CachedGUIDs[search]
				: AssetDatabase.FindAssets(search);

			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrEmpty(path))
				{
					continue;
				}

				var asset = Compatibility.LoadAssetAtPath<TextAsset>(path);
				if (asset != null)
				{
					var json = JsonUtility.FromJson<AssemblyDefinition>(asset.text);
					json.Path = path;

					output.Add(json);
				}
			}
		}
#endif

		/// <summary>
		/// Add reference to assemblies.
		/// </summary>
		/// <param name="search">Search string for assemblies.</param>
		/// <param name="reference">Reference.</param>
#if !UNITY_2018_1_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "search", Justification = "Reviewed.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "reference", Justification = "Reviewed.")]
#endif
		public static void Add(string search, string reference)
		{
#if UNITY_2018_1_OR_NEWER
			using var _ = ListPool<AssemblyDefinition>.Get(out var assemblies);
			Get(search, assemblies);

			foreach (var assembly in assemblies)
			{
				if (!assembly.references.Contains(reference))
				{
					assembly.references.Add(reference);
					File.WriteAllText(assembly.Path, JsonUtility.ToJson(assembly, true));
				}
			}
#endif
		}

		/// <summary>
		/// Check if matched assemblies contains specified reference.
		/// </summary>
		/// <param name="search">Search string for assemblies.</param>
		/// <param name="reference">Reference.</param>
		/// <returns>true if all assemblies contains specified reference; otherwise false</returns>
#if !UNITY_2018_1_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "search", Justification = "Reviewed.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "reference", Justification = "Reviewed.")]
#endif
		public static bool Contains(string search, string reference)
		{
#if UNITY_2018_1_OR_NEWER
			using var _ = ListPool<AssemblyDefinition>.Get(out var assemblies);
			Get(search, assemblies);

			foreach (var assembly in assemblies)
			{
				if (!assembly.references.Contains(reference))
				{
					return false;
				}
			}
#endif

			return true;
		}

		/// <summary>
		/// Remove reference from assemblies.
		/// </summary>
		/// <param name="search">Search string for assemblies.</param>
		/// <param name="reference">Reference.</param>
#if !UNITY_2018_1_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "search", Justification = "Reviewed")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "reference", Justification = "Reviewed")]
#endif
		public static void Remove(string search, string reference)
		{
#if UNITY_2018_1_OR_NEWER
			using var _ = ListPool<AssemblyDefinition>.Get(out var assemblies);
			Get(search, assemblies);

			foreach (var assembly in assemblies)
			{
				assembly.references.Remove(reference);
				File.WriteAllText(assembly.Path, JsonUtility.ToJson(assembly, true));
			}
#endif
		}
	}
}
#endif