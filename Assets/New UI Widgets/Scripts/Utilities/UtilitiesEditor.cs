namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
#if UIWIDGETS_TMPRO_SUPPORT && UIWIDGETS_TMPRO_4_0_OR_NEWER
	using FontAsset = UnityEngine.TextCore.Text.FontAsset;
#elif UIWIDGETS_TMPRO_SUPPORT
	using FontAsset = TMPro.TMP_FontAsset;
#else
	using FontAsset = UnityEngine.ScriptableObject;
#endif

	/// <summary>
	/// Editor functions.
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
				if (type.DeclaringType != null)
				{
					return string.Format("{0}.{1}", GetFriendlyTypeName(type.DeclaringType), friendly_name);
				}

				return string.IsNullOrEmpty(type.Namespace) ? friendly_name : string.Format("{0}.{1}", type.Namespace, friendly_name);
			}

			var backtick_index = friendly_name.IndexOf('`');
			if (backtick_index > 0)
			{
				friendly_name = friendly_name.Remove(backtick_index);
			}

			var sb = new StringBuilder();
			if (type.DeclaringType != null)
			{
				sb.Append(GetFriendlyTypeName(type.DeclaringType));
				sb.Append(".");
			}
			else if (!string.IsNullOrEmpty(type.Namespace))
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
		/// Serialize object with BinaryFormatter to base64 string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <returns>Serialized string.</returns>
		public static string Serialize(object obj)
		{
			var formatter = new BinaryFormatter();
			using var ms = new MemoryStream();

			formatter.Serialize(ms, obj);
			return Convert.ToBase64String(ms.ToArray());
		}

		/// <summary>
		/// De-serialize object with BinaryFormatter from base64 string.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <param name="encoded">Serialized string.</param>
		/// <returns>De-serialized object.</returns>
		public static T Deserialize<T>(string encoded)
		{
			var serializer = new BinaryFormatter();
			var ms = new MemoryStream();

			var bytes = Convert.FromBase64String(encoded);
			ms.Write(bytes, 0, bytes.Length);
			ms.Seek(0, SeekOrigin.Begin);

			return (T)serializer.Deserialize(ms);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Path to the selected asset.
		/// </summary>
		/// <returns>Path.</returns>
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
		/// Get default font.
		/// </summary>
		/// <returns>Default font.</returns>
		public static Font GetDefaultFont()
		{
#if UNITY_2022_2_OR_NEWER
			var font = "LegacyRuntime.ttf";
#else
			var font = "Arial.ttf";
#endif
			return Resources.GetBuiltinResource<Font>(font);
		}

		/// <summary>
		/// Get default TMPro font.
		/// </summary>
		/// <returns>Default TMPro font.</returns>
		public static FontAsset GetDefaultTMProFont()
		{
			var paths = new string[]
			{
				"Fonts & Materials/LiberationSans SDF",
				"TextMeshPro/Resources/Fonts & Materials/LiberationSans SDF",
				"Fonts & Materials/ARIAL SDF",
				"TextMeshPro/Resources/Fonts & Materials/ARIAL SDF",
			};

			foreach (var path in paths)
			{
				var font = Resources.Load<FontAsset>(path);
				if (font != null)
				{
					return font;
				}
			}

			return null;
		}

		/// <summary>
		/// Apply modified properties.
		/// </summary>
		/// <param name="serialized">Serialized object.</param>
		/// <returns>true if serialized object has modified properties; otherwise false.</returns>
		public static bool ApplyModifiedProperties(SerializedObject serialized)
		{
#if UNITY_2018_3_OR_NEWER
			var is_modified = serialized.hasModifiedProperties;
#else
			var is_modified = true;
#endif
			if (is_modified)
			{
				serialized.ApplyModifiedProperties();
			}

			return is_modified;
		}

		/// <summary>
		/// Create gameobject.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="undo">Support editor undo.</param>
		/// <returns>Created gameobject.</returns>
		[Obsolete("Replaced with Widgets.CreateGameObject()")]
		public static GameObject CreateGameObject(GameObject prefab, bool undo = true)
		{
			return null;
		}

		/// <summary>
		/// Returns the asset object of type T with the specified GUID.
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <returns>Asset with the specified GUID.</returns>
		/// <typeparam name="T">Asset type.</typeparam>
		public static T LoadAssetWithGUID<T>(string guid)
			where T : UnityEngine.Object
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogWarning("Path not found for the GUID: " + guid);
				return null;
			}

			return Compatibility.LoadAssetAtPath<T>(path);
		}

		/// <summary>
		/// Find assets by specified search.
		/// </summary>
		/// <typeparam name="T">Assets type.</typeparam>
		/// <param name="search">Search string.</param>
		/// <returns>Assets list.</returns>
		public static List<T> GetAssets<T>(string search)
			where T : UnityEngine.Object
		{
			var guids = AssetDatabase.FindAssets(search);

			var result = new List<T>(guids.Length);
			foreach (var guid in guids)
			{
				var asset = LoadAssetWithGUID<T>(guid);
				if (asset != null)
				{
					result.Add(asset);
				}
			}

			return result;
		}

		/// <summary>
		/// Get asset by label.
		/// </summary>
		/// <typeparam name="T">Asset type.</typeparam>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset.</returns>
		public static T GetAsset<T>(string label)
			where T : UnityEngine.Object
		{
			var path = GetAssetPath(label + "Asset");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return Compatibility.LoadAssetAtPath<T>(path);
		}

		/// <summary>
		/// Get asset path by label.
		/// </summary>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset path.</returns>
		public static string GetAssetPath(string label)
		{
			var key = "l:Uiwidgets" + label;
			var guids = AssetDatabase.FindAssets(key);
			if (guids.Length == 0)
			{
				Debug.LogWarning("Label not found: " + label);
				return null;
			}

			return AssetDatabase.GUIDToAssetPath(guids[0]);
		}

		/// <summary>
		/// Get prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		public static GameObject GetPrefab(string label)
		{
			var path = GetAssetPath(label + "Prefab");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return Compatibility.LoadAssetAtPath<GameObject>(path);
		}

		/// <summary>
		/// Get generated prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		public static GameObject GetGeneratedPrefab(string label)
		{
			return GetPrefab("Generated" + label);
		}

		/// <summary>
		/// Set prefabs label.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		public static void SetPrefabLabel(UnityEngine.Object prefab)
		{
			AssetDatabase.SetLabels(prefab, new[] { "UiwidgetsGenerated" + prefab.name + "Prefab", });
		}

		/// <summary>
		/// Create widget template from asset specified by label.
		/// </summary>
		/// <param name="templateLabel">Template label.</param>
		/// <returns>Widget template.</returns>
		[Obsolete("Replaced with Widgets.CreateTemplateFromAsset().")]
		public static GameObject CreateWidgetTemplateFromAsset(string templateLabel)
		{
			return null;
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="widget">Widget name.</param>
		/// <param name="applyStyle">Apply style to created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <returns>Created GameObject.</returns>
		[Obsolete("Replaced with Widgets.CreateFromAsset()")]
		public static GameObject CreateWidgetFromAsset(string widget, bool applyStyle = true, Action<GameObject> converter = null)
		{
			return null;
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="prefab">Widget name.</param>
		/// <param name="applyStyle">Apply style or attach theme to the created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <returns>Created GameObject.</returns>
		[Obsolete("Replaced with Widgets.CreateFromPrefab()")]
		public static GameObject CreateWidgetFromPrefab(GameObject prefab, bool applyStyle = true, Action<GameObject> converter = null)
		{
			return null;
		}

		/// <summary>
		/// Replace Close button callback on Cancel instead of the Hide for the Dialog components in the specified GameObject.
		/// </summary>
		/// <param name="go">GameObject.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		[Obsolete("Replaced with Widgets.FixDialogCloseButton()")]
		public static void FixDialogCloseButton(GameObject go)
		{
		}

		/// <summary>
		/// Gets the canvas transform.
		/// </summary>
		/// <returns>The canvas transform.</returns>
		[Obsolete("Replaced with Widgets.GetCanvasTransform()")]
		public static Transform GetCanvasTransform()
		{
			return null;
		}
#endif
	}
}