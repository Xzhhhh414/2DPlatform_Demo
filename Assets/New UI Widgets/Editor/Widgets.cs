#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using UIWidgets.Pool;
	using UnityEditor;
	using UnityEditor.Events;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Widgets.
	/// </summary>
	public static class Widgets
	{
		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="widget">Widget name.</param>
		/// <param name="applyStyle">Apply style to created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <returns>Created GameObject.</returns>
		[Obsolete("Replaced with CreateFromPrefab() method.")]
		public static GameObject CreateFromAsset(string widget, bool applyStyle = true, Action<GameObject> converter = null)
		{
			var go = CreateObjectFromAsset(widget + "Prefab");

			if (go != null)
			{
				converter?.Invoke(go);

				if (applyStyle)
				{
					#if UIWIDGETS_LEGACY_STYLE
					var style = PrefabsMenu.Instance.DefaultStyle;
					if (style != null)
					{
						style.ApplyTo(go);
					}
					#else
					UIThemes.Editor.ThemeAttach.AttachDefaultTheme(go, createOptions: false, out var _);
					#endif
				}
			}

			Upgrade(go);

			FixDialogCloseButton(go);

			return go;
		}

		static void SetWhiteSprite(GameObject go)
		{
			var white_sprite = ReferenceGUID.WhiteSprite;
			if (white_sprite == null)
			{
				return;
			}

			using var _ = ListPool<Image>.Get(out var images);
			go.GetComponentsInChildren(true, images);

			foreach (var image in images)
			{
				if (image.sprite == null)
				{
					image.sprite = white_sprite;
				}
			}
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="prefab">Widget name.</param>
		/// <returns>Created GameObject.</returns>
		public static GameObject CreateFromPrefab(GameObject prefab)
		{
			return CreateFromPrefab(prefab, true, ConverterTMPro.Widget2TMPro, WidgetsReferences.Instance.InstantiateWidgets);
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="prefab">Widget name.</param>
		/// <param name="applyStyle">Apply style or attach theme to the created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <param name="instantiateAsPrefab">If true instantiate game object as prefab reference; otherwise as copy of prefab.</param>
		/// <returns>Created GameObject.</returns>
		public static GameObject CreateFromPrefab(GameObject prefab, bool applyStyle, Action<GameObject> converter = null, bool instantiateAsPrefab = false)
		{
			var go = CreateGameObject(prefab, true, instantiateAsPrefab);

			if (go == null)
			{
				return null;
			}

			Selection.activeObject = go;

			if (WidgetsReferences.Instance.UseWhiteSprite)
			{
				SetWhiteSprite(go);
			}

			converter?.Invoke(go);

			if (applyStyle)
			{
				#if UIWIDGETS_LEGACY_STYLE
				var style = PrefabsMenu.Instance.DefaultStyle;
				if (style != null)
				{
					style.ApplyTo(go);
				}
				#else
				if (WidgetsReferences.Instance.AttachTheme)
				{
					UIThemes.Editor.ThemeAttach.AttachDefaultTheme(go, createOptions: false, out var _);
				}
				#endif
			}

			Upgrade(go);
			FixDialogCloseButton(go);

			return go;
		}

		static void Upgrade(GameObject go)
		{
			using var _ = ListPool<IUpgradeable>.Get(out var upgrades);
			go.GetComponentsInChildren(true, upgrades);
			for (int i = 0; i < upgrades.Count; i++)
			{
				upgrades[i].Upgrade();
			}
		}

		/// <summary>
		/// Replace Close button callback on Cancel instead of the Hide for the Dialog components in the specified GameObject.
		/// </summary>
		/// <param name="go">GameObject.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public static void FixDialogCloseButton(GameObject go)
		{
			var dialogs = go.GetComponentsInChildren<Dialog>(true);

			foreach (var dialog in dialogs)
			{
				var button_go = dialog.transform.Find("Header/CloseButton");
				if (button_go == null)
				{
					continue;
				}

				if (!button_go.TryGetComponent<Button>(out var button))
				{
					continue;
				}

				if (IsEventCallMethod(button.onClick, dialog, "Hide"))
				{
					UnityEventTools.RemovePersistentListener(button.onClick, dialog.Hide);
					UnityEventTools.AddPersistentListener(button.onClick, dialog.Cancel);
				}
			}
		}

		static bool IsEventCallMethod(UnityEvent ev, MonoBehaviour target, string method)
		{
			var n = ev.GetPersistentEventCount();
			for (int i = 0; i < n; i++)
			{
				if ((ev.GetPersistentMethodName(i) == method) && (ev.GetPersistentTarget(i) == target))
				{
					return true;
				}
			}

			return false;
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
		/// Creates the object by path to asset prefab.
		/// </summary>
		/// <returns>The created object.</returns>
		/// <param name="path">Path.</param>
		static GameObject CreateObject(string path)
		{
			var prefab = Compatibility.LoadAssetAtPath<GameObject>(path);
			if (Utilities.IsNull(prefab))
			{
				throw new ArgumentException(string.Format("Prefab not found at path {0}.", path));
			}

			return CreateGameObject(prefab);
		}

		/// <summary>
		/// Creates the object from asset.
		/// </summary>
		/// <returns>The object from asset.</returns>
		/// <param name="key">Search string.</param>
		static GameObject CreateObjectFromAsset(string key)
		{
			var path = GetAssetPath(key);
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			var go = CreateObject(path);

			Selection.activeObject = go;

			return go;
		}

		/// <summary>
		/// Create gameobject.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="undo">Support editor undo.</param>
		/// <param name="instantiateAsPrefab">Instantiate gameobject as prefab reference or copy of prefab.</param>
		/// <returns>Created gameobject.</returns>
		public static GameObject CreateGameObject(GameObject prefab, bool undo = true, bool instantiateAsPrefab = false)
		{
			var instance = instantiateAsPrefab
				? PrefabUtility.InstantiatePrefab(prefab) as GameObject
				: Compatibility.Instantiate(prefab);

			if (undo)
			{
				Undo.RegisterCreatedObjectUndo(instance, "Create " + prefab.name);
			}

			var go_parent = Selection.activeTransform;
			if ((go_parent == null) || (!(go_parent is RectTransform)))
			{
				go_parent = GetCanvasTransform();
			}

			if (go_parent != null)
			{
				if (undo)
				{
					Undo.SetTransformParent(instance.transform, go_parent, "Create " + prefab.name);
				}
				else
				{
					instance.transform.SetParent(go_parent, false);
				}
			}

			instance.name = prefab.name;

			var rectTransform = instance.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.anchoredPosition = new Vector2(0, 0);

				Utilities.FixInstantiated(prefab, instance);
			}

			return instance;
		}

		/// <summary>
		/// Gets the canvas transform.
		/// </summary>
		/// <returns>The canvas transform.</returns>
		public static Transform GetCanvasTransform()
		{
			var canvas = (Selection.activeGameObject != null) ? Selection.activeGameObject.GetComponentInParent<Canvas>() : null;
			if (Utilities.IsNull(canvas))
			{
				canvas = Compatibility.FindFirstObjectOfType<Canvas>();
			}

			if (canvas != null)
			{
				return canvas.transform;
			}

			var canvasGO = new GameObject("Canvas")
			{
				layer = LayerMask.NameToLayer("UI"),
			};
			canvas = canvasGO.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler>();
			canvasGO.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(canvasGO, "Create " + canvasGO.name);

			if (Compatibility.FindFirstObjectOfType<EventSystem>() == null)
			{
				Compatibility.CreateEventSystem();
			}

			return canvasGO.transform;
		}
	}
}
#endif