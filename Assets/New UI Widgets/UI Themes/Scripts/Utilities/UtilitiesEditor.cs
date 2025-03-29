#if UNITY_EDITOR
namespace UIThemes
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UIThemes.Pool;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Utilities editor.
	/// </summary>
	public static class UtilitiesEditor
	{
		/// <summary>
		/// Get friendly name of the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Friendly name.</returns>
		public static string GetFriendlyTypeName(Type type)
		{
			var friendly_name = type.Name;

			if (!type.IsGenericType)
			{
				return string.IsNullOrEmpty(type.Namespace) ? friendly_name : string.Format("{0}.{1}", type.Namespace, friendly_name);
			}

			var backtick_index = friendly_name.IndexOf('`');
			if (backtick_index > 0)
			{
				friendly_name = friendly_name.Remove(backtick_index);
			}

			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(type.Namespace))
			{
				sb.Append(type.Namespace);
				sb.Append(".");
			}

			sb.Append(friendly_name);
			sb.Append('<');

			var type_parameters = type.GetGenericArguments();
			for (int i = 0; i < type_parameters.Length; ++i)
			{
				var type_param_name = GetFriendlyTypeName(type_parameters[i]);
				if (i > 0)
				{
					sb.Append(',');
				}

				sb.Append(type_param_name);
			}

			sb.Append('>');

			return sb.ToString();
		}

		[DomainReloadExclude]
		static readonly char[] ScriptingSymbolSeparator = new char[] { ';' };

		/// <summary>
		/// Get scripting define symbols.
		/// </summary>
		/// <param name="targetGroup">Target group.</param>
		/// <returns>Scripting define symbols.</returns>
		public static HashSet<string> GetScriptingDefineSymbols(BuildTargetGroup targetGroup)
		{
			#if UNITY_2023_1_OR_NEWER
			var build_target = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
			var symbols = PlayerSettings.GetScriptingDefineSymbols(build_target);
			#else
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
			#endif

			var result = new HashSet<string>();
			foreach (var symbol in symbols.Split(ScriptingSymbolSeparator))
			{
				if (symbol.Length > 0)
				{
					result.Add(symbol);
				}
			}

			return result;
		}

		/// <summary>
		/// Set scripting define symbols.
		/// </summary>
		/// <param name="targetGroup">Target group.</param>
		/// <param name="symbols">Symbols.</param>
		public static void SetScriptingDefineSymbols(BuildTargetGroup targetGroup, HashSet<string> symbols)
		{
			var arr = new string[symbols.Count];
			symbols.CopyTo(arr);
			var str = string.Join(ScriptingSymbolSeparator[0].ToString(), arr);

			#if UNITY_2023_1_OR_NEWER
			var build_target = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
			PlayerSettings.SetScriptingDefineSymbols(build_target, str);
			#else
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, str);
			#endif
		}

		/// <summary>
		/// Get type by full name.
		/// </summary>
		/// <param name="typename">Type name.</param>
		/// <returns>Type.</returns>
		public static Type GetType(string typename)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type[] types;
				try
				{
					types = assembly.GetTypes();
				}
				catch (System.Reflection.ReflectionTypeLoadException e)
				{
					types = e.Types;
				}

				foreach (var assembly_type in types)
				{
					if ((assembly_type != null) && (assembly_type.FullName == typename))
					{
						return assembly_type;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Get path to the selected asset.
		/// </summary>
		/// <returns>Path to the selected asset</returns>
		public static string SelectedAssetPath()
		{
			var folder = "Assets";
			if (Selection.activeObject != null)
			{
				folder = AssetDatabase.GetAssetPath(Selection.activeObject);
				if (!Directory.Exists(folder))
				{
					folder = Path.GetDirectoryName(folder);
				}
			}

			return folder;
		}

		/// <summary>
		/// Create ScriptableObject asset.
		/// </summary>
		/// <typeparam name="T">Type of ScriptableObject.</typeparam>
		/// <param name="path">Path.</param>
		/// <param name="save">Save.</param>
		/// <returns>ScriptableObject instance.</returns>
		public static T CreateScriptableObjectAsset<T>(string path, bool save = true)
			where T : ScriptableObject
		{
			var file = AssetDatabase.GenerateUniqueAssetPath(path);
			var asset = ScriptableObject.CreateInstance<T>();

			AssetDatabase.CreateAsset(asset, file);
			EditorUtility.SetDirty(asset);

			if (save)
			{
				AssetDatabase.SaveAssets();
			}

			return asset;
		}

		/// <summary>
		/// Mark specified component as dirty.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <param name="target">Component.</param>
		public static void MarkDirty<T>(T target)
			where T : Component
		{
			#if UNITY_2018_3_OR_NEWER
			if (PrefabUtility.IsPartOfAnyPrefab(target))
			#endif
			{
				PrefabUtility.RecordPrefabInstancePropertyModifications(target);
			}

			EditorUtility.SetDirty(target);
		}

		/// <summary>
		/// Refresh targets.
		/// Editor only.
		/// </summary>
		public static void RefreshTargets()
		{
			if (Application.isPlaying)
			{
				return;
			}

			var prefab_go = GetPrefabStageGameObject();
			if (prefab_go != null)
			{
				RefreshTarget(prefab_go);
			}

			using var _ = ListPool<GameObject>.Get(out var objects);
			Utilities.GetRootGameObjects(objects);
			foreach (var go in objects)
			{
				RefreshTarget(go);
			}
		}

		static void RefreshTarget(GameObject go)
		{
			using var _ = ListPool<IThemeTarget>.Get(out var targets);

			go.GetComponentsInChildren(true, targets);
			foreach (var t in targets)
			{
				t.Refresh();
			}
		}

		/// <summary>
		/// Get prefab stage gameobjects.
		/// </summary>
		/// <returns>Prefab gameobject.</returns>
		public static GameObject GetPrefabStageGameObject()
		{
#if UNITY_2021_2_OR_NEWER
			var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
			var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
			if ((stage != null) && (stage.prefabContentsRoot != null))
			{
				return stage.prefabContentsRoot;
			}

			return null;
		}

		/// <summary>
		/// Get root gameobjects at hierarchy.
		/// </summary>
		/// <param name="output">Output.</param>
		public static void GetHierarchyRootGameObjects(List<GameObject> output)
		{
			var prefab_go = GetPrefabStageGameObject();
			if (prefab_go != null)
			{
				output.Add(prefab_go);
			}
			else
			{
				Utilities.GetRootGameObjects(output);
			}
		}
	}
}
#endif