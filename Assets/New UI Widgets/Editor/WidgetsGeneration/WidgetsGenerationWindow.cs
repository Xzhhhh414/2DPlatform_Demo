#if UNITY_EDITOR
namespace UIWidgets.WidgetGeneration
{
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Widgets generator window.
	/// </summary>
	public class WidgetsGenerationWindow : EditorWindow
	{
		/// <summary>
		/// Show window.
		/// </summary>
		[MenuItem("Window/New UI Widgets/Widgets Generator")]
		public static void Open()
		{
			Open(null, null);
		}

		/// <summary>
		/// Show window.
		/// </summary>
		/// <param name="script">Script.</param>
		/// <param name="info">Class info.</param>
		public static void Open(MonoScript script, ClassInfo info)
		{
			var window = GetWindow<WidgetsGenerationWindow>("Widgets Generator");
			window.minSize = new Vector2(520, 200);
			window.currentScript = script;
			window.info = info;
		}

		readonly GUIStyle styleLabel = new GUIStyle();

		readonly GUIStyle styleHeader = new GUIStyle();

		readonly GUILayoutOption[] scrollOptions = new GUILayoutOption[] { GUILayout.Height(150) };

		readonly GUILayoutOption[] errorOptions = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.MaxHeight(150) };

		readonly GUIContent toggleLabel = new GUIContent(string.Empty, "Use in widgets");

		readonly GUILayoutOption[] toggleHeaderOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(30) };

		readonly GUILayoutOption[] toggleOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(20) };

		readonly GUILayoutOption[] fieldOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(200) };

		readonly GUILayoutOption[] autocompleteOptions = new GUILayoutOption[] { GUILayout.Width(150) };

		readonly GUILayoutOption[] autocompleteButtonOptions = new GUILayoutOption[] { GUILayout.Width(40) };

		MonoScript previousScript;

		MonoScript currentScript;

		string previousType;

		string currentType;

		Vector2 scrollPosition;

		ClassInfo info;

		/// <summary>
		/// Set styles.
		/// </summary>
		protected virtual void SetStyles()
		{
			styleLabel.margin = new RectOffset(4, 4, 2, 2);
			styleLabel.richText = true;

			styleHeader.margin = new RectOffset(4, 4, 0, 0);
			styleHeader.fontStyle = FontStyle.Bold;
		}

		/// <summary>
		/// Draw GUI.
		/// </summary>
		protected virtual void OnGUI()
		{
			SetStyles();

			GUILayout.Label("Widgets Generator", EditorStyles.boldLabel);
			currentScript = EditorGUILayout.ObjectField("Data Script", currentScript, typeof(MonoScript), false, new GUILayoutOption[] { }) as MonoScript;

			if ((previousScript != currentScript) || (info == null))
			{
				info = new ClassInfo(currentScript);
				previousScript = currentScript;

				previousType = info.FullTypeName;
				currentType = info.FullTypeName;
			}

			currentType = EditorGUILayout.TextField("Data Type", currentType);

			if (previousType != currentType)
			{
				info = new ClassInfo(currentType);
				previousType = currentType;
			}

			if (!info.IsValid)
			{
				ShowErrors();
				return;
			}
			else
			{
				ShowFields();
			}

			var button_label = "Generate Widgets";

			if (info.IsUnityObject)
			{
				GUILayout.Label("<b>Warning:</b>", styleLabel);
				GUILayout.Label("Class is derived from Unity.Object.\nUsing it as a data class can be a bad practice and lead to future problems.", styleLabel);
				button_label = "Continue Generation";
			}

			if (info.Fields.Count == 0)
			{
				GUILayout.Label("<b>Cannot generate widgets since all fields deselected.</b>", styleLabel);
				return;
			}

			if (GUILayout.Button(button_label))
			{
				var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(currentScript));
				var gen = new ScriptsGenerator(info, path);
				gen.Generate();
				Close();
			}
		}

		void ShowFields()
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, scrollOptions);

			GUILayout.Space(16f);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Use", styleHeader, toggleHeaderOptions);
			GUILayout.Label("Field", styleHeader, fieldOptions);
			GUILayout.Label("Autocomplete", styleHeader, autocompleteOptions);

			EditorGUILayout.EndHorizontal();

			foreach (var field in info.AllFields)
			{
				EditorGUILayout.BeginHorizontal();

				ShowField(field);

				EditorGUILayout.BeginVertical();
				GUILayout.Space(20f);
				EditorGUILayout.EndVertical();

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndScrollView();
		}

		void ShowField(ClassField field)
		{
			var allowed = info.Fields.Contains(field);

			GUILayout.Space(5f);
			if (allowed != EditorGUILayout.Toggle(toggleLabel, allowed, toggleOptions))
			{
				if (allowed)
				{
					info.Fields.Remove(field);
				}
				else
				{
					info.Fields.Add(field);
				}
			}

			GUILayout.Space(1f);
			GUILayout.Label(field.FieldName, fieldOptions);

			if (field.AllowAutocomplete)
			{
				if (field.FieldName == info.AutocompleteField)
				{
					EditorGUILayout.LabelField("Used", autocompleteOptions);
				}
				else if (GUILayout.Button("Use", autocompleteButtonOptions))
				{
					info.AutocompleteField = field.FieldName;
				}
			}
		}

		void ShowErrors()
		{
			GUILayout.Label("<b>Errors:</b>", styleLabel);
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, scrollOptions);
			foreach (var error in info.Errors)
			{
				GUILayout.Label(error, styleLabel, errorOptions);
			}

			EditorGUILayout.EndScrollView();
		}
	}
}
#endif