#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using UIThemes.Pool;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// Window to show ThemeTarget list.
	/// </summary>
	public class ThemeTargetsWindow : EditorWindow
	{
		/// <summary>
		/// Search options used in ThemeTargets.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		[Serializable]
		protected abstract class OptionsSearch<T>
		{
			[SerializeField]
			List<T> result = new List<T>();

			/// <summary>
			/// Result.
			/// </summary>
			public List<T> Result => result;

			/// <summary>
			/// Button to toggle search mode.
			/// </summary>
			public Button Button;

			/// <summary>
			/// Query.
			/// </summary>
			protected OptionQuery Query;

			/// <summary>
			/// Current progress.
			/// </summary>
			protected int currentProgress;

			/// <summary>
			/// Run search.
			/// </summary>
			/// <param name="query">Query.</param>
			public void Search(OptionQuery query)
			{
				Query = query;
				Reset();
				Search();
			}

			/// <summary>
			/// Run search.
			/// </summary>
			protected abstract void Search();

			/// <summary>
			/// Search options in the specified game objects.
			/// </summary>
			/// <param name="gameObjects">GameObjects.</param>
			/// <returns>ThemeTargets that match the query.</returns>
			protected virtual IEnumerable<ThemeTargetBase> Search(IList<GameObject> gameObjects)
			{
				var temp = ListPool<ThemeTargetBase>.Get();

				currentProgress = 0;
				for (var i = 0; i < gameObjects.Count; i++)
				{
					gameObjects[i].GetComponentsInChildren(true, temp);

					foreach (var target in temp)
					{
						if (Query.Match(target))
						{
							yield return target;
						}
					}

					temp.Clear();
					currentProgress += 1;
				}

				ListPool<ThemeTargetBase>.Release(temp);
			}

			/// <summary>
			/// Show progress.
			/// </summary>
			/// <param name="progress">Progress.</param>
			/// <param name="info">Info.</param>
			protected virtual void ProgressShow(float progress, string info = "")
			{
				EditorUtility.DisplayProgressBar("Search", info, progress);
			}

			/// <summary>
			/// Stop progress.
			/// </summary>
			protected virtual void ProgressStop()
			{
				EditorUtility.ClearProgressBar();
			}

			/// <summary>
			/// Reset.
			/// </summary>
			public virtual void Reset()
			{
				Result.Clear();
			}

			/// <summary>
			/// Get assets paths.
			/// </summary>
			/// <param name="output">Assets paths.</param>
			/// <param name="filter">Assets filter.</param>
			/// <param name="paths">Paths to search.</param>
			protected virtual void GetAssetsPaths(List<string> output, string filter, params string[] paths)
			{
				if (paths.Length == 0)
				{
					paths = new[] { "Assets" };
				}

				var guids = AssetDatabase.FindAssets(filter, paths);
				foreach (var guid in guids)
				{
					var path = AssetDatabase.GUIDToAssetPath(guid);
					if (output.Contains(path))
					{
						continue;
					}

					output.Add(path);
				}
			}
		}

		/// <summary>
		/// Search options used in ThemeTargets across open scenes.
		/// </summary>
		[Serializable]
		protected class OpenScenesSearch : OptionsSearch<ThemeTargetBase>
		{
			/// <inheritdoc/>
			protected override void Search()
			{
				using var _ = ListPool<GameObject>.Get(out var objects);
				UtilitiesEditor.GetHierarchyRootGameObjects(objects);

				var total = (float)objects.Count;
				foreach (var target in Search(objects))
				{
					Result.Add(target);
					ProgressShow(currentProgress / total);
				}

				ProgressStop();
			}
		}

		/// <summary>
		/// Search options used in ThemeTargets across all scenes.
		/// </summary>
		[Serializable]
		protected class AllScenesSearch : OptionsSearch<SceneReference>
		{
			/// <summary>
			/// Assets path.
			/// </summary>
			public string AssetsPath = null;

			/// <inheritdoc/>
			protected override void Search()
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

				using var _ = ListPool<string>.Get(out var paths);
				GetAssetsPaths(paths, "t:scene", AssetsPath);
				var total = (float)paths.Count;
				var objects = new List<string>();

				for (var i = 0; i < paths.Count; i++)
				{
					var scene_path = paths[i];
					ProgressShow(i / total, scene_path);

					var scene = EditorSceneManager.OpenScene(scene_path);
					if (!scene.IsValid())
					{
						Debug.LogWarning("Unable to open Scene: " + scene_path);
						continue;
					}

					if (!scene.isLoaded)
					{
						continue;
					}

					foreach (var target in Search(scene.GetRootGameObjects()))
					{
						objects.Add(target.name);
					}

					if (objects.Count > 0)
					{
						Result.Add(new SceneReference(scene_path, scene.name, objects));
						objects = new List<string>();
					}
				}

				ProgressStop();
			}
		}

		/// <summary>
		/// Search options used in ThemeTargets across all prefabs.
		/// </summary>
		[Serializable]
		protected class PrefabsSearch : OptionsSearch<PrefabReference>
		{
			/// <summary>
			/// Assets path.
			/// </summary>
			public string AssetsPath = null;

			/// <inheritdoc/>
			protected override void Search()
			{
				using var _ = ListPool<string>.Get(out var paths);
				GetAssetsPaths(paths, "t:prefab", AssetsPath);

				ProgressShow(0f);

				var total = (float)paths.Count;
				for (var i = 0; i < paths.Count; i++)
				{
					using var __ = ListPool<GameObject>.Get(out var temp);

					var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
					temp.Add(prefab);

					foreach (var target in Search(temp))
					{
						Result.Add(new PrefabReference(prefab, target));
					}

					ProgressShow((i + 1) / total);
				}

				ProgressStop();
			}
		}

		/// <summary>
		/// Reference to the scene.
		/// </summary>
		[Serializable]
		protected struct SceneReference
		{
			[SerializeField]
			string path;

			/// <summary>
			/// Path to the scene.
			/// </summary>
			public readonly string Path => path;

			[SerializeField]
			string name;

			/// <summary>
			/// Scene name.
			/// </summary>
			public readonly string Name => name;

			[SerializeField]
			List<string> objects;

			/// <summary>
			/// Names of found gameobjects.
			/// </summary>
			public readonly List<string> Objects => objects;

			/// <summary>
			/// Initializes a new instance of the <see cref="SceneReference"/> struct.
			/// </summary>
			/// <param name="path">Path to the scene.</param>
			/// <param name="name">Scene name.</param>
			/// <param name="objects">Names of found gameobjects.</param>
			public SceneReference(string path, string name, List<string> objects)
			{
				this.path = path;
				this.name = name;
				this.objects = objects;
			}
		}

		/// <summary>
		/// Reference to the prefab.
		/// </summary>
		[Serializable]
		protected struct PrefabReference
		{
			[SerializeField]
			GameObject prefab;

			/// <summary>
			/// Prefab.
			/// </summary>
			public readonly GameObject Prefab => prefab;

			[SerializeField]
			ThemeTargetBase themeTarget;

			/// <summary>
			/// ThemeTarget.
			/// </summary>
			public readonly ThemeTargetBase ThemeTarget => themeTarget;

			/// <summary>
			/// Initializes a new instance of the <see cref="PrefabReference"/> struct.
			/// </summary>
			/// <param name="prefab">Prefab.</param>
			/// <param name="themeTarget">ThemeTarget.</param>
			public PrefabReference(GameObject prefab, ThemeTargetBase themeTarget)
			{
				this.prefab = prefab;
				this.themeTarget = themeTarget;
			}
		}

		/// <summary>
		/// Option query.
		/// </summary>
		[Serializable]
		protected struct OptionQuery
		{
			[SerializeField]
			Theme theme;

			/// <summary>
			/// Theme.
			/// </summary>
			public readonly Theme Theme => theme;

			[SerializeField]
			string property;

			/// <summary>
			/// Property.
			/// </summary>
			public readonly string Property => property;

			[SerializeField]
			Option option;

			/// <summary>
			/// Option.
			/// </summary>
			public readonly Option Option => option;

			/// <summary>
			/// Is instance valid?
			/// </summary>
			public readonly bool Valid => option != null && !string.IsNullOrEmpty(property) && theme != null;

			/// <summary>
			/// Initializes a new instance of the <see cref="OptionQuery"/> struct.
			/// </summary>
			/// <param name="theme">Theme.</param>
			/// <param name="property">Property.</param>
			/// <param name="option">Option.</param>
			public OptionQuery(Theme theme, string property, Option option)
			{
				this.theme = theme;
				this.property = property;
				this.option = option;
			}

			/// <summary>
			/// Check if target uses specified option.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <returns>true if target match condition; otherwise false.</returns>
			public readonly bool Match(ThemeTargetBase target)
			{
				if (!UnityObjectComparer<Theme>.Instance.Equals(target.GetTheme(), theme))
				{
					return false;
				}

				return ThemeTargetInfo.Get(target).UseOption(target, property, option.Id);
			}
		}

		/// <summary>
		/// Seach mode.
		/// </summary>
		[Serializable]
		protected enum SearchMode
		{
			/// <summary>
			/// Open scenes.
			/// </summary>
			OpenScenes = 0,

			/// <summary>
			/// All scenes.
			/// </summary>
			AllScenes = 1,

			/// <summary>
			/// Prefabs.
			/// </summary>
			Prefabs = 2,
		}

		/// <summary>
		/// Query.
		/// </summary>
		[SerializeField]
		protected OptionQuery Query;

		/// <summary>
		/// Open scenes.
		/// </summary>
		[SerializeField]
		protected OpenScenesSearch OpenScenes = new OpenScenesSearch();

		/// <summary>
		/// All scenes.
		/// </summary>
		[SerializeField]
		protected AllScenesSearch AllScenes = new AllScenesSearch();

		/// <summary>
		/// Prefabs.
		/// </summary>
		[SerializeField]
		protected PrefabsSearch Prefabs = new PrefabsSearch();

		/// <summary>
		/// Search mode.
		/// </summary>
		[SerializeField]
		protected SearchMode Mode = SearchMode.OpenScenes;

		[SerializeField]
		string assetsPath = "Assets";

		/// <summary>
		/// Assets path.
		/// </summary>
		protected string AssetsPath
		{
			get => assetsPath;
			set => SetAssetsPath(value);
		}

		/// <summary>
		/// Root.
		/// </summary>
		protected VisualElement Root;

		/// <summary>
		/// Results block.
		/// </summary>
		protected ScrollView ResultsBlock;

		/// <summary>
		/// Button block.
		/// </summary>
		protected VisualElement ButtonsBlock;

		/// <summary>
		/// Assets path input.
		/// </summary>
		protected TextField AssetsPathInput;

		/// <summary>
		/// Open.
		/// </summary>
		/// <param name="theme">Theme.</param>
		/// <param name="property">Property.</param>
		/// <param name="option">Option.</param>
		public static void Open(Theme theme, string property, Option option)
		{
			var query = new OptionQuery(theme, property, option);
			var window = GetWindow<ThemeTargetsWindow>();
			window.Query = query;
			window.minSize = new Vector2(480, 550);
			window.titleContent = new GUIContent("Theme Targets Search");
			window.Search(SearchMode.OpenScenes);
			window.Refresh();
		}

		/// <summary>
		/// Reset search.
		/// </summary>
		protected virtual void SearchReset()
		{
			OpenScenes.Reset();
			AllScenes.Reset();
			Prefabs.Reset();
		}

		/// <summary>
		/// Search.
		/// </summary>
		/// <param name="mode">Search mode.</param>
		protected virtual void Search(SearchMode mode)
		{
			Mode = mode;
			SearchReset();

			switch (Mode)
			{
				case SearchMode.OpenScenes:
					OpenScenes.Search(Query);
					break;
				case SearchMode.AllScenes:
					AllScenes.Search(Query);
					break;
				case SearchMode.Prefabs:
					Prefabs.Search(Query);
					break;
			}

			if (ResultsBlock != null)
			{
				ShowResults();
				ToggleButtons();
			}
		}

		/// <summary>
		/// Create GUI.
		/// </summary>
		public virtual void CreateGUI()
		{
			if (Root != null)
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

			Root = new VisualElement();
			Root.AddToClassList("theme-targets-window");
			rootVisualElement.Add(Root);

			Refresh();
		}

		/// <summary>
		/// Set assets path.
		/// </summary>
		/// <param name="path">Path.</param>
		protected void SetAssetsPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = "Assets";
			}

			assetsPath = path;

			AllScenes.AssetsPath = path;
			Prefabs.AssetsPath = path;

			if (AssetsPathInput != null)
			{
				AssetsPathInput.value = assetsPath;
			}

			var exists = System.IO.Directory.Exists(path);
			AllScenes.Button?.SetEnabled(exists);
			Prefabs.Button?.SetEnabled(exists);
		}

		/// <summary>
		/// Request assets path.
		/// </summary>
		protected void RequestAssetsPath()
		{
			var result = Utilities.SelectAssetsPath(AssetsPath, "Select a directory for wrappers scripts");
			if (string.IsNullOrEmpty(result))
			{
				return;
			}

			AssetsPath = result;
		}

		/// <summary>
		/// Refresh.
		/// </summary>
		public virtual void Refresh()
		{
			if (!Query.Valid)
			{
				return;
			}

			Root.Clear();

			ShowQuery();

			ResultsBlock = new ScrollView(ScrollViewMode.Vertical);
			ResultsBlock.AddToClassList("theme-targets-list");
			Root.Add(ResultsBlock);

			ShowResults();

			ShowAssetsPath();
			ShowButtons();
		}

		/// <summary>
		/// Show assets path block.
		/// </summary>
		protected void ShowAssetsPath()
		{
			var block = new VisualElement();
			block.AddToClassList("theme-assets-path");

			AssetsPathInput = new TextField("Assets Path")
			{
				value = AssetsPath,
			};
			AssetsPathInput.RegisterValueChangedCallback(x => AssetsPath = x.newValue);
			block.Add(AssetsPathInput);

			var select = new Button(RequestAssetsPath)
			{
				text = "...",
			};
			block.Add(select);

			Root.Add(block);
		}

		/// <summary>
		/// Show buttons.
		/// </summary>
		protected void ShowButtons()
		{
			var buttons = new VisualElement();
			buttons.AddToClassList("theme-targets-buttons");

			OpenScenes.Button = new Button(() => Search(SearchMode.OpenScenes))
			{
				text = "Search on Open Scenes or Prefab",
			};
			buttons.Add(OpenScenes.Button);

			AllScenes.Button = new Button(() => Search(SearchMode.AllScenes))
			{
				text = "Search on All Scenes",
			};
			buttons.Add(AllScenes.Button);

			Prefabs.Button = new Button(() => Search(SearchMode.Prefabs))
			{
				text = "Search on Prefabs",
			};
			buttons.Add(Prefabs.Button);

			Root.Add(buttons);

			ToggleButtons();
		}

		/// <summary>
		/// Toggle buttons state.
		/// </summary>
		protected void ToggleButtons()
		{
			OpenScenes.Button.EnableInClassList("theme-targets-button-active", Mode == SearchMode.OpenScenes);
			AllScenes.Button.EnableInClassList("theme-targets-button-active", Mode == SearchMode.AllScenes);
			Prefabs.Button.EnableInClassList("theme-targets-button-active", Mode == SearchMode.Prefabs);
		}

		/// <summary>
		/// Show results.
		/// </summary>
		protected virtual void ShowResults()
		{
			ResultsBlock.Clear();

			switch (Mode)
			{
				case SearchMode.OpenScenes:
					ShowOpenScenes(ResultsBlock);
					break;
				case SearchMode.AllScenes:
					ShowAllScenes(ResultsBlock);
					break;
				case SearchMode.Prefabs:
					ShowPrefabs(ResultsBlock);
					break;
			}
		}

		/// <summary>
		/// Show open scenes.
		/// </summary>
		/// <param name="scroll">Scroll.</param>
		protected virtual void ShowOpenScenes(ScrollView scroll)
		{
			var k = 1;
			for (int i = 0; i < OpenScenes.Result.Count; i++)
			{
				var t = OpenScenes.Result[i];
				if (t == null)
				{
					continue;
				}

				var button = new Button(() => EditorGUIUtility.PingObject(t))
				{
					text = string.Format("{0}. {1}", k.ToString(), t.name),
				};
				button.AddToClassList("theme-targets-target");

				scroll.Add(button);
				k++;
			}
		}

		/// <summary>
		/// Show all scenes.
		/// </summary>
		/// <param name="scroll">Scroll.</param>
		protected virtual void ShowAllScenes(ScrollView scroll)
		{
			var k = 1;
			for (int i = 0; i < AllScenes.Result.Count; i++)
			{
				var t = AllScenes.Result[i];
				var button = new Button(() =>
				{
					var s = AssetDatabase.LoadAssetAtPath<SceneAsset>(t.Path);
					EditorGUIUtility.PingObject(s);
				})
				{
					text = string.Format("{0}. {1}", k.ToString(), t.Name),
				};
				button.AddToClassList("theme-targets-target");
				scroll.Add(button);

				var foldout = new Foldout()
				{
					value = false,
					text = string.Format("GameObject: {0}", t.Objects.Count.ToString()),
				};
				foreach (var go_name in t.Objects)
				{
					var label = new Label(go_name);
					label.AddToClassList("theme-targets-target-gameobject");
					foldout.contentContainer.Add(label);
				}

				scroll.Add(foldout);
				k++;
			}
		}

		/// <summary>
		/// Show prefabs.
		/// </summary>
		/// <param name="scroll">Scroll.</param>
		protected virtual void ShowPrefabs(ScrollView scroll)
		{
			var k = 1;
			for (int i = 0; i < Prefabs.Result.Count; i++)
			{
				var t = Prefabs.Result[i];
				if (t.Prefab == null)
				{
					continue;
				}

				var button = new Button(() => EditorGUIUtility.PingObject(t.Prefab))
				{
					text = string.Format("{0}. {1} / {2}", k.ToString(), t.Prefab.name, t.ThemeTarget.name),
				};
				button.AddToClassList("theme-targets-target");
				scroll.Add(button);
				k++;
			}
		}

		/// <summary>
		/// Show query.
		/// </summary>
		protected virtual void ShowQuery()
		{
			var label_template =
				#if UNITY_2021_2_OR_NEWER
				"{0}: <b>{1}</b>";
				#else
				"{0}: {1}";
				#endif

			var theme_name = new Label(string.Format(label_template, "Theme", Query.Theme.name));
			theme_name.AddToClassList("theme-targets-query-theme");
			Root.Add(theme_name);

			var property = new Label(string.Format(label_template, "Property Type", Query.Property));
			property.AddToClassList("theme-targets-query-property");
			Root.Add(property);

			var option_name = new Label(string.Format(label_template, "Option", Query.Option.Name));
			option_name.AddToClassList("theme-targets-query-option");
			Root.Add(option_name);
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			#if !UNITY_2020_3_OR_NEWER
			CreateGUI();
			#endif
		}
	}
}
#endif